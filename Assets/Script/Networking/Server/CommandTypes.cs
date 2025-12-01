using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  ///   명령어 데이터 형식 (Pixel-Server와 동일)
  ///   Command data format: {cmd: string, data: {}}
  /// </summary>
  [Serializable]
  public class CommandData
  {
    [JsonProperty("cmd")] public string cmd;

    [JsonProperty("data")] public JToken data;

    public CommandData()
    {
    }

    public CommandData(string cmd, JToken data = null)
    {
      this.cmd = cmd;
      this.data = data;
    }
  }

  /// <summary>
  ///   명령어 응답 형식 (Pixel-Server와 동일)
  /// </summary>
  [Serializable]
  public class CommandResponse
  {
    public CommandResponse()
    {
    }

    public CommandResponse(int code, string message, JToken data = null)
    {
      this.code = code;
      this.message = message;
      this.data = data;
    }

    [JsonProperty("code")]
    public int code;

    [JsonProperty("message")]
    public string message;

    [JsonProperty("data")] 
    public JToken data;

    /// <summary>
    ///   성공 응답을 생성합니다.
    /// </summary>
    public static CommandResponse Success(string message, JToken data = null)
    {
      return new CommandResponse(100, message, data);
    }

    public static CommandResponse Message(params string[] msg)
    {
      return new CommandResponse(101, string.Join('\n', msg), null);
    }

    /// <summary>
    ///   에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse Error(int code, string message, JObject data = null)
    {
      return new CommandResponse(code, message, data);
    }

    /// <summary>
    ///   등록되지 않은 명령어 에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse NotFound(string command)
    {
      return new CommandResponse(404, $"등록되지 않은 명령어입니다: {command}");
    }

    /// <summary>
    ///   서버 에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse ServerError(string message)
    {
      return new CommandResponse(500, message);
    }

    public override string ToString()
    {
      return new JObject
      {
        ["code"] = code,
        ["message"] = message,
        ["data"] = data.ToString(Formatting.None)
      }.ToString();
    }
  }
}