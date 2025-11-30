using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PixelCollector.Networking.Client;
using PixelCollector.Networking.Server;
using SocketIOClient;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelCollector
{
  /// <summary>
  /// 소켓 서버와 통신하여 명령을 수신하고 서버 상태를 전송하는 클래스입니다.
  /// </summary>
  public class ServerManagement : MonoBehaviour
  {
    [SerializeField] private NetServerManager serverManager;
    [SerializeField] private ushort socketPort = 7777;
    
    private SocketIOUnity socket;
    
    /// <summary>
    /// 명령어 처리를 담당하는 핸들러입니다.
    /// </summary>
    private readonly SocketCommandHandler commandHandler = new();
    
    private void Awake()
    { 
      InitializeSocket();
      RegisterDefaultCommands();
      // serverManager.StartServer();
    }
    
    /// <summary>
    /// 소켓 연결을 초기화하고 이벤트 핸들러를 등록합니다.
    /// </summary>
    private void InitializeSocket()
    {
      socket = new SocketIOUnity($"http://localhost:{socketPort}", new SocketIOOptions
      {
        Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
        Query = new Dictionary<string, string>
        {
          ["clientType"] = "unity"
        } 
      });
      
      socket.OnConnected += OnSocketConnected;
      socket.OnDisconnected += OnSocketDisconnected;
      
      // 명령어 수신 이벤트 핸들러 등록
      socket.On("command", commandHandler.HandleCommand);
      
      socket.Connect();
      Debug.Log(socket.Connected);
    }
    
    /// <summary>
    /// 기본 명령어들을 등록합니다.
    /// </summary>
    private void RegisterDefaultCommands()
    {
      // 상태 요청 명령어
      RegisterCommand("get_status", args => SendServerStatus());
    }
    
    /// <summary>
    /// 새로운 명령어 핸들러를 등록합니다.
    /// </summary>
    /// <param name="command">명령어 문자열</param>
    /// <param name="handler">명령어 처리 함수</param>
    public void RegisterCommand(string command, Action<object> handler)
    {
      commandHandler.RegisterCommand(command, handler);
    }
    
    /// <summary>
    /// 등록된 명령어 핸들러를 제거합니다.
    /// </summary>
    /// <param name="command">제거할 명령어 문자열</param>
    public void UnregisterCommand(string command)
    {
      commandHandler.UnregisterCommand(command);
    }
    
    /// <summary>
    /// 서버 상태를 소켓 서버로 전송합니다.
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
    /// 소켓 서버로 데이터를 전송합니다.
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
  }
}
