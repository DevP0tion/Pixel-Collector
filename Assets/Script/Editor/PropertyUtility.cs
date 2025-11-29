using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

  /// <summary>
  ///   대충 Chatgpt한테서 긁어와서 정리한 코드에요
  ///   유니티 에디터에서 값을 긁어오거나 설정할 때 사용용도로 구현했어요
  /// </summary>
  public static class PropertyUtility
  {
    public static object Get(this SerializedProperty property)
    {
      object obj = property.serializedObject.targetObject;
      var path = property.propertyPath.Replace(".Array.data[", "[").Split('.');

      foreach (var part in path)
        if (part.Contains("["))
        {
          var fieldName = part[..part.IndexOf("[")];
          var index = int.Parse(part[(part.IndexOf("[", StringComparison.Ordinal) + 1)..].Replace("]", ""));
          obj = GetFieldValue(obj, fieldName, index);
        }
        else
        {
          obj = GetFieldValue(obj, part);
        }

      return obj;
    }

    public static T Get<T>(this SerializedProperty property)
    {
      return (T)Get(property);
    }

    public static T Set<T>(this SerializedProperty property, T value)
    {
      object obj = property.serializedObject.targetObject;
      var path = property.propertyPath.Replace(".Array.data[", "[")
        .Split('.');

      for (var i = 0; i < path.Length - 1; i++)
      {
        var part = path[i];
        if (part.Contains("["))
        {
          var fieldName = part[..part.IndexOf("[")];
          var index = int.Parse(part[(part.IndexOf("[") + 1)..].Replace("]", ""));
          obj = GetFieldValue(obj, fieldName, index);
        }
        else
        {
          obj = GetFieldValue(obj, part);
        }
      }

      // 마지막 필드 설정
      var last = path[^1];
      if (last.Contains("["))
      {
        var fieldName = last[..last.IndexOf("[")];
        var index = int.Parse(last[(last.IndexOf("[") + 1)..].Replace("]", ""));
        SetListElementValue(obj, fieldName, index, value);
      }
      else
      {
        SetFieldValue(obj, last, value);
      }

      property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

      return value;
    }

    private static object GetFieldValue(object source, string name)
    {
      if (source == null)
        return null;

      var type = source.GetType();
      var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      return field?.GetValue(source);
    }

    private static object GetFieldValue(object source, string name, int index)
    {
      if (GetFieldValue(source, name) is not IEnumerable enumerable) return null;

      var enumerator = enumerable.GetEnumerator();
      using var enumerator1 = enumerator as IDisposable;
      for (var i = 0; i <= index; i++)
        if (!enumerator.MoveNext())
          return null;
      return enumerator.Current;
    }

    private static void SetFieldValue(object source, string name, object value)
    {
      if (source == null) return;

      var type = source.GetType();
      var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      if (field != null) field.SetValue(source, value);
    }

    private static void SetListElementValue(object source, string name, int index, object value)
    {
      var field = source.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      if (field == null) return;

      var list = field.GetValue(source) as IList;
      if (list != null && index >= 0 && index < list.Count) list[index] = value;
    }
  }
