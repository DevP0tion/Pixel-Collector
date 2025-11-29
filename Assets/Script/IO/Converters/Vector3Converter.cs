using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PixelCollector.IO.Converters
{
  public class Vector3Converter : JsonConverter<Vector3>
  {
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
      value.ToJValue().WriteTo(writer);
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var str = reader.Value?.ToString();
      
      if (reader.TokenType != JsonToken.String || string.IsNullOrEmpty(str)) return hasExistingValue ? existingValue : Vector3.zero;
      
      var values = str.Split(',');

      return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
    }
  }

  public static class Vector3ConverterExtension
  {
    public static JValue ToJValue(this Vector3 value)
      => new ($"{value.x},{value.y},{value.z}");

    public static Vector3 ToVector3(this JValue value)
    {
      if (value.Type != JTokenType.String) return Vector3.zero;
      
      var values = value.ToString().Split(',');
      return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
    }
  }
}