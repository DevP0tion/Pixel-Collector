using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  ///   명령어 핸들러 타입 (Pixel-Server와 동일)
  ///   소켓과 인자를 받아 처리하고 CommandResponse를 반환합니다.
  /// </summary>
  public delegate CommandResponse CommandHandler(SocketIOUnity socket, CommandData args);

  /// <summary>
  ///   소켓 서버로부터 명령을 수신하고 처리하는 클래스입니다.
  ///   명령어 데이터 형식: {cmd: string, data: {}}
  ///   Pixel-Server의 SocketCommandHandler와 인터페이스를 맞춤
  /// </summary>
  public class SocketCommandHandler
  {
    /// <summary>
    ///   명령어 핸들러 딕셔너리입니다. 키는 명령어 문자열이며, 값은 소켓과 인자를 받아 응답을 반환하는 함수입니다.
    /// </summary>
    private readonly Dictionary<string, CommandHandler> commandHandlers = new();

    /// <summary>
    ///   소켓 인스턴스 (응답 전송용)
    /// </summary>
    private SocketIOUnity socket;

    /// <summary>
    ///   소켓 인스턴스를 설정합니다.
    /// </summary>
    /// <param name="socketInstance">소켓 인스턴스</param>
    public void SetSocket(SocketIOUnity socketInstance)
    {
      socket = socketInstance;
    }

    /// <summary>
    ///   새로운 명령어 핸들러를 등록합니다.
    /// </summary>
    /// <param name="command">명령어 문자열</param>
    /// <param name="handler">명령어 처리 함수 (소켓과 인자를 받아 응답을 반환)</param>
    public void RegisterCommand(string command, CommandHandler handler)
    {
      if (string.IsNullOrEmpty(command))
      {
        Debug.LogWarning("[SocketCommandHandler] 빈 명령어는 등록할 수 없습니다.");
        return;
      }

      commandHandlers[command] = handler;
    }

    /// <summary>
    ///   등록된 명령어 핸들러를 제거합니다.
    /// </summary>
    /// <param name="command">제거할 명령어 문자열</param>
    public void UnregisterCommand(string command)
    {
      if (string.IsNullOrEmpty(command)) return;

      commandHandlers.Remove(command);
    }

    /// <summary>
    ///   등록된 명령어가 있는지 확인합니다.
    /// </summary>
    /// <param name="command">확인할 명령어 문자열</param>
    /// <returns>등록 여부</returns>
    public bool HasCommand(string command)
    {
      if (string.IsNullOrEmpty(command)) return false;

      return commandHandlers.ContainsKey(command);
    }

    /// <summary>
    ///   소켓 서버로부터 명령어를 수신했을 때 호출됩니다.
    ///   명령어 데이터 형식: {cmd: string, data: {}}
    /// </summary>
    public void HandleCommand(SocketIOResponse response)
    {
      try
      {
        // var cmdRes = response.GetValue<CommandData>();
        var json = response.GetValue();

        ExecuteCommand(new CommandData 
          {
            cmd = json.GetProperty("cmd").GetString(), 
            data = JToken.Parse(json.GetProperty("data").GetRawText())
          }
        );
      }
      catch (Exception e)
      {
        Debug.LogError($"[SocketCommandHandler] 명령어 처리 중 오류 발생: {e.Message}");
        SendResponse(CommandResponse.ServerError($"명령어 처리 중 오류 발생: {e.Message}"));
      }
    }

    /// <summary>
    ///   명령어를 직접 실행합니다.
    /// </summary>
    /// <param name="cmd">실행할 명령어 데이터</param>
    public void ExecuteCommand(CommandData cmd)
    {
      if (string.IsNullOrEmpty(cmd.cmd))
      {
        Debug.LogWarning("[SocketCommandHandler] 명령어(cmd)가 비어있습니다.");
        SendResponse(CommandResponse.ServerError("명령어(cmd)가 비어있습니다."));
        return;
      }

      if (commandHandlers.TryGetValue(cmd.cmd, out var handler))
      {
        var response = handler.Invoke(socket, cmd);
        if (response != null) SendResponse(response);
      }
      else
      {
        Debug.LogWarning($"[SocketCommandHandler] 등록되지 않은 명령어입니다: {cmd.cmd}");
        SendResponse(CommandResponse.NotFound(cmd.cmd));
      }
    }

    /// <summary>
    ///   명령어 응답을 소켓 서버로 전송합니다.
    /// </summary>
    /// <param name="response">응답 데이터</param>
    public void SendResponse(CommandResponse response)
    {
      if (socket == null || !socket.Connected)
      {
        Debug.LogWarning("[SocketCommandHandler] 소켓이 연결되어 있지 않아 응답을 전송할 수 없습니다.");
        return;
      }

      socket.Emit("command:response", response.ToString());
    }

    /// <summary>
    ///   모든 명령어 핸들러를 제거합니다.
    /// </summary>
    public void ClearCommands()
    {
      commandHandlers.Clear();
    }

    /// <summary>
    ///   등록된 모든 명령어 목록을 반환합니다.
    /// </summary>
    public IEnumerable<string> GetRegisteredCommands()
    {
      return commandHandlers.Keys;
    }
  }
}