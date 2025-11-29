using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PixelCollector.IO.Converters
{
  public class StringHashSetConverter : JsonConverter<HashSet<string>>
  {
    public override void WriteJson(JsonWriter writer, HashSet<string> value, JsonSerializer serializer)
    {
      var data = new JArray();

      foreach (var item in value)
        data.Add(item);
      
      data.WriteTo(writer);
    }

    public override HashSet<string> ReadJson(JsonReader reader, Type objectType, HashSet<string> existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var result = hasExistingValue && existingValue != null
        ? new HashSet<string>(existingValue) // 기존 값을 복사해서 사용
        : new HashSet<string>();
      JArray json;
      
      try
      {
        // JSON을 JObject로 로드
        json = JArray.Load(reader);
      }
      catch (JsonReaderException)
      {
        return result;
      }

      foreach (var item in json)
      {
        if (item is not { Type: JTokenType.String }) continue;
        
        var value = item.Get(string.Empty);
        
        if (string.IsNullOrEmpty(value)) continue;
        
        result.Add(value);
      }
      
      return result;
    }
  }
}