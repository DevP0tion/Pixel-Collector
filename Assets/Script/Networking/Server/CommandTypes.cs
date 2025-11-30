using System;
using Newtonsoft.Json;

namespace PixelCollector.Networking.Server
{
  /// <summary>
  /// 명령어 데이터 형식 (Pixel-Server와 동일)
  /// Command data format: {cmd: string, args: {}}
  /// </summary>
  [Serializable]
  public class CommandData
  {
    [JsonProperty("cmd")]
    public string Cmd { get; set; }
    
    [JsonProperty("args")]
    public object Args { get; set; }
    
    public CommandData() { }
    
    public CommandData(string cmd, object args = null)
    {
      Cmd = cmd;
      Args = args;
    }
  }
  
  /// <summary>
  /// 명령어 응답 형식 (Pixel-Server와 동일)
  /// </summary>
  [Serializable]
  public class CommandResponse
  {
    [JsonProperty("code")]
    public int Code { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public object Data { get; set; }
    
    public CommandResponse() { }
    
    public CommandResponse(int code, string message, object data = null)
    {
      Code = code;
      Message = message;
      Data = data;
    }
    
    /// <summary>
    /// 성공 응답을 생성합니다.
    /// </summary>
    public static CommandResponse Success(string message, object data = null)
    {
      return new CommandResponse(100, message, data);
    }
    
    /// <summary>
    /// 에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse Error(int code, string message, object data = null)
    {
      return new CommandResponse(code, message, data);
    }
    
    /// <summary>
    /// 등록되지 않은 명령어 에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse NotFound(string command)
    {
      return new CommandResponse(404, $"등록되지 않은 명령어입니다: {command}");
    }
    
    /// <summary>
    /// 서버 에러 응답을 생성합니다.
    /// </summary>
    public static CommandResponse ServerError(string message)
    {
      return new CommandResponse(500, message);
    }
  }
}
