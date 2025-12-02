using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  ///   서버 명령어를 관리하고 구현하는 정적 클래스입니다.
  ///   ServerManagement에서 명령어 로직을 분리하여 유지보수성을 향상시킵니다.
  /// </summary>
  public static class CommandManager
  {
    /// <summary>
    ///   기본 명령어들을 핸들러에 등록합니다.
    /// </summary>
    /// <param name="handler">명령어를 등록할 핸들러</param>
    /// <param name="serverManager">서버 매니저 인스턴스 (서버 정보 조회용)</param>
    public static void RegisterDefaultCommands(SocketCommandHandler handler, NetServerManager serverManager)
    {
      if (handler == null)
      {
        Debug.LogError("[CommandManager] SocketCommandHandler가 null입니다.");
        return;
      }

      RegisterStatusCommand(handler);
      RegisterPingCommand(handler);
      RegisterHelpCommand(handler);
      RegisterServerInfoCommand(handler);
      RegisterServerScenesCommand(handler);
      RegisterServerPlayersCommand(handler, serverManager);
    }

    /// <summary>
    ///   상태 요청 명령어를 등록합니다.
    /// </summary>
    private static void RegisterStatusCommand(SocketCommandHandler handler)
    {
      handler.RegisterCommand("status", "서버 상태를 출력합니다.", (_, _) =>
      {
        var serverTime = DateTimeOffset.UtcNow.ToString("o");
        var uptime = Time.realtimeSinceStartup;

        var message = $"Status\n" +
                      $"online: true\n" +
                      $"serverTime: {serverTime}\n" +
                      $"uptime: {uptime:F2}s";

        var data = new JObject
        {
          ["online"] = true,
          ["serverTime"] = serverTime,
          ["uptime"] = uptime
        };

        return CommandResponse.Success(message, data);
      });
    }

    /// <summary>
    ///   Ping 테스트 명령어를 등록합니다.
    /// </summary>
    private static void RegisterPingCommand(SocketCommandHandler handler)
    {
      handler.RegisterCommand("ping", "핑 테스트", (_, _) =>
      {
        var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return CommandResponse.Success($"pong {timeStamp}", new JObject
        {
          ["timestamp"] = timeStamp
        });
      });
    }

    /// <summary>
    ///   도움말 명령어를 등록합니다.
    /// </summary>
    private static void RegisterHelpCommand(SocketCommandHandler handler)
    {
      handler.RegisterCommand("help", "명령어 목록을 출력합니다.", (_, _) =>
      {
        var commands = handler.GetRegisteredCommands().ToArray();
        var helpMessage = "사용 가능한 명령어:\n" + string.Join("\n", commands.Select(c => $"  {c}"));
        var result = new JArray();

        foreach (var cmd in commands) result.Add(cmd);

        return CommandResponse.Success(helpMessage, result);
      });
    }

    /// <summary>
    ///   서버 정보 명령어를 등록합니다.
    /// </summary>
    private static void RegisterServerInfoCommand(SocketCommandHandler handler)
    {
      handler.RegisterCommand("server:info", "서버 정보를 출력합니다.", (_, _) =>
      {
        var uptime = Time.realtimeSinceStartup;
        var systemMemory = SystemInfo.systemMemorySize;
        var graphicsDevice = SystemInfo.graphicsDeviceName;

        var message = $"Server Info\n" +
                      $"name: Pixel Collector Unity Server\n" +
                      $"version: {Application.version}\n" +
                      $"unityVersion: {Application.unityVersion}\n" +
                      $"platform: {Application.platform}\n" +
                      $"uptime: {uptime:F2}s\n" +
                      $"systemMemory: {systemMemory} MB\n" +
                      $"graphicsDevice: {graphicsDevice}";

        var data = new JObject
        {
          ["name"] = "Pixel Collector Unity Server",
          ["version"] = Application.version,
          ["unityVersion"] = Application.unityVersion,
          ["platform"] = Application.platform.ToString(),
          ["uptime"] = uptime,
          ["systemMemory"] = systemMemory,
          ["graphicsDevice"] = graphicsDevice
        };

        return CommandResponse.Success(message, data);
      });
    }

    /// <summary>
    ///   현재 활성화된 씬 목록 명령어를 등록합니다.
    /// </summary>
    private static void RegisterServerScenesCommand(SocketCommandHandler handler)
    {
      handler.RegisterCommand("server:scenes", "현재 활성화된 씬 목록을 출력합니다.", (_, _) =>
      {
        var scenes = new JArray();
        var sceneNames = new System.Collections.Generic.List<string>();

        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
          var scene = SceneManager.GetSceneAt(i);
          sceneNames.Add(scene.name);
          scenes.Add(new JObject
          {
            ["name"] = scene.name,
            ["path"] = scene.path,
            ["isLoaded"] = scene.isLoaded,
            ["buildIndex"] = scene.buildIndex
          });
        }

        var message = $"Active Scenes ({SceneManager.sceneCount}):\n" +
                      string.Join("\n", sceneNames.Select(name => $"  {name}"));

        return CommandResponse.Success(message, scenes);
      });
    }

    /// <summary>
    ///   현재 플레이어 목록 명령어를 등록합니다.
    /// </summary>
    private static void RegisterServerPlayersCommand(SocketCommandHandler handler, NetServerManager serverManager)
    {
      handler.RegisterCommand("server:players", "현재 접속한 플레이어 목록을 출력합니다.", (_, _) =>
      {
        if (serverManager == null)
        {
          return CommandResponse.ServerError("서버 매니저를 사용할 수 없습니다.");
        }

        var players = serverManager.players.Values;
        var playerArray = new JArray();
        var playerList = new System.Collections.Generic.List<string>();

        foreach (var player in players)
        {
          playerList.Add($"{player.username} ({player.conn.connectionId})");
          playerArray.Add(new JObject
          {
            ["username"] = player.username,
            ["ipAddress"] = player.conn.address,
            ["connectedAt"] = player.connectedAt.ToString("o")
          });
        }

        var message = $"Connected Players ({players.Count}):\n" +
                      string.Join("\n", playerList.Select(p => $"  {p}"));

        return CommandResponse.Success(message, playerArray);
      });
    }
  }
}

