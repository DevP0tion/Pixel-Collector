using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PixelCollector.IO.Converters
{
  public class IntBoolDictionaryConverter : JsonConverter<Dictionary<int, bool>>
  {
    public override void WriteJson(JsonWriter writer, Dictionary<int, bool> dictionary, JsonSerializer serializer)
    {
      var data = new JObject();

      foreach (var (key, value) in dictionary)
      {
        data[key.ToString()] = value;
      }
      
      data.WriteTo(writer);
    }

    public override Dictionary<int, bool> ReadJson(JsonReader reader, Type objectType, Dictionary<int, bool> existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var result = hasExistingValue && existingValue != null
        ? new Dictionary<int, bool>(existingValue) // 기존 값을 복사해서 사용
        : new Dictionary<int, bool>();
      JObject json;
      
      try
      {
        // JSON을 JObject로 로드
        json = JObject.Load(reader);
      }
      catch (JsonReaderException)
      {
        return result;
      }
      
      foreach (var (key, value) in json)
      {
        // 문자열 키 → int 변환
        if (!int.TryParse(key, out var id)) continue;
        if (value is not { Type: JTokenType.Boolean }) continue;
        
        // 문자열 값 → Enum 변환
        
        result[id] = value.Get(false);
      }

      return result;
    }
  }
}