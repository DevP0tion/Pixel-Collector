using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PixelCollector.Networking.Client;
using SocketIOClient;
using UnityEngine;

namespace PixelCollector
{
  /// <summary>
  /// 소켓 서버와 통신하여 명령을 수신하고 서버 상태를 전송하는 클래스입니다.
  /// </summary>
  public class ServerManagement : MonoBehaviour
  {
    [SerializeField] private NetClientManager netClientManager;
    [SerializeField] private string socketServerUrl = "http://localhost:7777";
    
    private SocketIOUnity socket;
    
    /// <summary>
    /// 명령어 핸들러 딕셔너리입니다. 키는 명령어 문자열이며, 값은 인자를 받아 실행하는 Action입니다.
    /// </summary>
    private readonly Dictionary<string, Action<object>> commandHandlers = new();
    
    private void Awake()
    { 
      InitializeSocket();
      RegisterDefaultCommands();
      netClientManager.StartServer();
    }
    
    /// <summary>
    /// 소켓 연결을 초기화하고 이벤트 핸들러를 등록합니다.
    /// </summary>
    private void InitializeSocket()
    {
      socket = new SocketIOUnity(socketServerUrl, new SocketIOOptions
      {
        Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
      });
      
      socket.OnConnected += OnSocketConnected;
      socket.OnDisconnected += OnSocketDisconnected;
      
      // 명령어 수신 이벤트 핸들러 등록
      socket.On("command", OnCommandReceived);
      
      socket.Connect();
    }
    
    /// <summary>
    /// 기본 명령어들을 등록합니다.
    /// </summary>
    private void RegisterDefaultCommands()
    {
      // 상태 요청 명령어
      RegisterCommand("get_status", _ => SendServerStatus());
    }
    
    /// <summary>
    /// 새로운 명령어 핸들러를 등록합니다.
    /// </summary>
    /// <param name="command">명령어 문자열</param>
    /// <param name="handler">명령어 처리 함수</param>
    public void RegisterCommand(string command, Action<object> handler)
    {
      if (string.IsNullOrEmpty(command))
      {
        Debug.LogWarning("[ServerManagement] 빈 명령어는 등록할 수 없습니다.");
        return;
      }
      
      commandHandlers[command] = handler;
    }
    
    /// <summary>
    /// 등록된 명령어 핸들러를 제거합니다.
    /// </summary>
    /// <param name="command">제거할 명령어 문자열</param>
    public void UnregisterCommand(string command)
    {
      commandHandlers.Remove(command);
    }
    
    /// <summary>
    /// 소켓 서버로부터 명령어를 수신했을 때 호출됩니다.
    /// 명령어 데이터 형식: {cmd: string, args: {}}
    /// </summary>
    private void OnCommandReceived(SocketIOResponse response)
    {
      try
      {
        var data = response.GetValue<JObject>();
        
        if (data == null)
        {
          Debug.LogWarning("[ServerManagement] 수신한 명령어 데이터가 null입니다.");
          return;
        }
        
        var cmd = data["cmd"]?.ToString();
        var args = data["args"]?.ToObject<object>();
        
        if (string.IsNullOrEmpty(cmd))
        {
          Debug.LogWarning("[ServerManagement] 명령어(cmd)가 비어있습니다.");
          return;
        }
        
        if (commandHandlers.TryGetValue(cmd, out var handler))
        {
          handler?.Invoke(args);
        }
        else
        {
          Debug.LogWarning($"[ServerManagement] 등록되지 않은 명령어입니다: {cmd}");
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"[ServerManagement] 명령어 처리 중 오류 발생: {e.Message}");
      }
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
