using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  /// 소켓 서버로부터 명령을 수신하고 처리하는 클래스입니다.
  /// 명령어 데이터 형식: {cmd: string, args: {}}
  /// </summary>
  public class SocketCommandHandler
  {
    /// <summary>
    /// 명령어 핸들러 딕셔너리입니다. 키는 명령어 문자열이며, 값은 인자를 받아 실행하는 Action입니다.
    /// </summary>
    private readonly Dictionary<string, Action<object>> commandHandlers = new();
    
    /// <summary>
    /// 새로운 명령어 핸들러를 등록합니다.
    /// </summary>
    /// <param name="command">명령어 문자열</param>
    /// <param name="handler">명령어 처리 함수</param>
    public void RegisterCommand(string command, Action<object> handler)
    {
      if (string.IsNullOrEmpty(command))
      {
        Debug.LogWarning("[SocketCommandHandler] 빈 명령어는 등록할 수 없습니다.");
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
      if (string.IsNullOrEmpty(command))
      {
        return;
      }
      
      commandHandlers.Remove(command);
    }
    
    /// <summary>
    /// 등록된 명령어가 있는지 확인합니다.
    /// </summary>
    /// <param name="command">확인할 명령어 문자열</param>
    /// <returns>등록 여부</returns>
    public bool HasCommand(string command)
    {
      if (string.IsNullOrEmpty(command))
      {
        return false;
      }
      
      return commandHandlers.ContainsKey(command);
    }
    
    /// <summary>
    /// 소켓 서버로부터 명령어를 수신했을 때 호출됩니다.
    /// 명령어 데이터 형식: {cmd: string, args: {}}
    /// </summary>
    public void HandleCommand(SocketIOResponse response)
    {
      try
      {
        var data = response.GetValue<JObject>();
        
        if (data == null)
        {
          Debug.LogWarning("[SocketCommandHandler] 수신한 명령어 데이터가 null입니다.");
          return;
        }
        
        var cmd = data["cmd"]?.ToString();
        var args = data["args"]?.ToObject<object>();
        
        ExecuteCommand(cmd, args);
      }
      catch (Exception e)
      {
        Debug.LogError($"[SocketCommandHandler] 명령어 처리 중 오류 발생: {e.Message}");
      }
    }
    
    /// <summary>
    /// 명령어를 직접 실행합니다.
    /// </summary>
    /// <param name="command">실행할 명령어</param>
    /// <param name="args">명령어 인자</param>
    public void ExecuteCommand(string command, object args)
    {
      if (string.IsNullOrEmpty(command))
      {
        Debug.LogWarning("[SocketCommandHandler] 명령어(cmd)가 비어있습니다.");
        return;
      }
      
      if (commandHandlers.TryGetValue(command, out var handler))
      {
        handler.Invoke(args);
      }
      else
      {
        Debug.LogWarning($"[SocketCommandHandler] 등록되지 않은 명령어입니다: {command}");
      }
    }
    
    /// <summary>
    /// 모든 명령어 핸들러를 제거합니다.
    /// </summary>
    public void ClearCommands()
    {
      commandHandlers.Clear();
    }
  }
}
