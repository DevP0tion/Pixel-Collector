using Newtonsoft.Json.Linq;

namespace PixelCollector.IO
{
  public interface IJsonSerializable
  {
    void LoadJson(JObject json);
    JObject ToJson();
  }
}