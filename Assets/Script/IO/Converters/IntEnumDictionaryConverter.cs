using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PixelCollector.IO.Converters
{
  public class IntEnumDictionaryConverter <V> : JsonConverter<Dictionary<int, V>> where V : Enum
  {
    public override void WriteJson(JsonWriter writer, Dictionary<int, V> dictionary, JsonSerializer serializer)
    {
      var data = new JObject();

      foreach (var (key, value) in dictionary)
      {
        data[key.ToString()] = Convert.ToInt32(value);
        // data[key.ToString()] = value.ToString();
      }
      
      data.WriteTo(writer);
    }

    public override Dictionary<int, V> ReadJson(JsonReader reader, Type objectType, Dictionary<int, V> existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var result = hasExistingValue && existingValue != null
        ? new Dictionary<int, V>(existingValue) // 기존 값을 복사해서 사용
        : new Dictionary<int, V>();
      
      var dictionaryData = new JObject();
      
      if (reader.TokenType == JsonToken.StartObject)
      {
        dictionaryData = JObject.Load(reader);
      }
      
      foreach (var (key, value) in dictionaryData)
      {
        // 문자열 키 → int 변환
        if (!int.TryParse(key, out var id) || 
            value is not { Type: JTokenType.Integer }) 
          continue;
        
        var valueInt = value.Get(0);
        
        // int 값 → Enum 변환
        result[id] = Enum.IsDefined(typeof(V), valueInt) ? (V) Enum.ToObject(typeof(V), valueInt) : default;
      }

      return result;
    }
  }
}