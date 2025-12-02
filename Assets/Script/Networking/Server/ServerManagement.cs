using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PixelCollector.Networking.Server;
using SocketIOClient;
using SocketIOClient.Transport;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  ///   소켓 서버와 통신하여 명령을 수신하고 서버 상태를 전송하는 클래스입니다.
  ///   Pixel-Server의 명령어 인터페이스에 맞춰 구현되었습니다.
  /// </summary>
  public class ServerManagement : MonoBehaviour
  {
    [SerializeField] private NetServerManager serverManager;
    [SerializeField] private ushort socketPort = 7777;

    /// <summary>
    ///   명령어 처리를 담당하는 핸들러입니다.
    /// </summary>
    private readonly SocketCommandHandler commandHandler = new();
    private SocketIOUnity socket;

    #region Unity Callback
    private void Awake()
    {
      if (Application.platform != RuntimePlatform.WindowsServer)
      {
        Debug.LogWarning("Server only works on Windows Server");
        Destroy(gameObject);
        return;
      }
      
      DontDestroyOnLoad(gameObject);
      RegisterDefaultCommands();
    }

    private void Start()
    {
      InitializeSocket((_, _) => serverManager.StartServer());
    }

    private void OnDestroy()
    {
      if (socket != null)
      {
        socket.OnConnected -= OnSocketConnected;
        socket.OnDisconnected -= OnSocketDisconnected;
        socket.Disconnect();
        socket.Dispose();
      }
    }
    
    #endregion

    #region SocketIO Initialization
    
    /// <summary>
    ///   소켓 연결을 초기화하고 이벤트 핸들러를 등록합니다.
    /// </summary>
    private void InitializeSocket(EventHandler afterConnect = null)
    {
      socket = new SocketIOUnity($"http://localhost:{socketPort}", new SocketIOOptions
      {
        Transport = TransportProtocol.WebSocket,
        Query = new Dictionary<string, string>
        {
          ["clientType"] = "unity"
        }
      });

      // 소켓 인스턴스를 명령어 핸들러에 설정
      commandHandler.SetSocket(socket);

      socket.OnConnected += OnSocketConnected;
      socket.OnDisconnected += OnSocketDisconnected;

      // Pixel-Server의 이벤트 이름에 맞춰 "unity:command" 이벤트 수신
      socket.On("unity:command", commandHandler.HandleCommand);

      socket.OnConnected += afterConnect;

      socket.Connect();
    }

    /// <summary>
    ///   기본 명령어들을 등록합니다.
    ///   CommandManager를 통해 명령어를 등록합니다.
    /// </summary>
    private void RegisterDefaultCommands()
    {
      CommandManager.RegisterDefaultCommands(commandHandler, serverManager);
    }

    /// <summary>
    ///   새로운 명령어 핸들러를 등록합니다.
    /// </summary>
    /// <param name="command">명령어 문자열</param>
    /// <param name="description">명령어 설명</param>
    /// <param name="handler">명령어 처리 함수 (소켓과 인자를 받아 응답을 반환)</param>
    public void RegisterCommand(string command, string description, CommandHandler handler)
    {
      commandHandler.RegisterCommand(command, description, handler);
    }

    /// <summary>
    ///   등록된 명령어 핸들러를 제거합니다.
    /// </summary>
    /// <param name="command">제거할 명령어 문자열</param>
    public void UnregisterCommand(string command)
    {
      commandHandler.UnregisterCommand(command);
    }

    /// <summary>
    ///   서버 상태를 소켓 서버로 전송합니다.
    /// </summary>
    public void SendServerStatus()
    {
      if (socket == null || !socket.Connected)
      {
        Debug.LogWarning("[ServerManagement] 소켓이 연결되어 있지 않습니다.");
        return;
      }

      var status = new JObject
      {
        ["online"] = true,
        ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
      };

      socket.Emit("server_status", status);
    }

    /// <summary>
    ///   명령어 응답을 전송합니다.
    /// </summary>
    /// <param name="response">응답 데이터</param>
    public void SendCommandResponse(CommandResponse response)
    {
      commandHandler.SendResponse(response);
    }

    /// <summary>
    ///   게임 로그를 웹 콘솔로 전송합니다.
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="level">로그 레벨 (info, warning, error)</param>
    public void SendGameLog(string message, string level = "info")
    {
      if (socket == null || !socket.Connected)
      {
        Debug.LogWarning("[ServerManagement] 소켓이 연결되어 있지 않습니다.");
        return;
      }

      socket.Emit("game:log", new
      {
        message,
        level,
        timestamp = DateTimeOffset.UtcNow.ToString("o")
      });
    }

    /// <summary>
    ///   게임 이벤트를 웹 콘솔로 전송합니다.
    /// </summary>
    /// <param name="eventType">이벤트 타입</param>
    /// <param name="data">이벤트 데이터</param>
    public void SendGameEvent(string eventType, object data)
    {
      if (socket == null || !socket.Connected)
      {
        Debug.LogWarning("[ServerManagement] 소켓이 연결되어 있지 않습니다.");
        return;
      }

      socket.Emit("game:event", new
      {
        type = eventType,
        data,
        timestamp = DateTimeOffset.UtcNow.ToString("o")
      });
    }

    /// <summary>
    ///   소켓 서버로 데이터를 전송합니다.
    /// </summary>
    /// <param name="eventName">이벤트 이름</param>
    /// <param name="data">전송할 데이터</param>
    public void SendToSocket(string eventName, object data)
    {
      if (socket == null || !socket.Connected)
      {
        Debug.LogWarning("[ServerManagement] 소켓이 연결되어 있지 않습니다.");
        return;
      }

      socket.Emit(eventName, data);
    }

    private void OnSocketConnected(object sender, EventArgs e)
    {
      Debug.Log("[ServerManagement] 소켓 서버에 연결되었습니다.");
      SendServerStatus();
    }

    private void OnSocketDisconnected(object sender, string reason)
    {
      Debug.Log($"[ServerManagement] 소켓 서버 연결이 해제되었습니다: {reason}");
    }
    
    #endregion
  }
}