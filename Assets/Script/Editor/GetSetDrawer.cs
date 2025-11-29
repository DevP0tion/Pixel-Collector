#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using PixelCollector.Util;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GetSetAttribute))]
public sealed class GetSetDrawer : PropertyDrawer
{
  private static readonly Dictionary<(Type, string), PropertyInfo> _propertyCache = new();
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    var attribute = (GetSetAttribute)this.attribute;

    EditorGUI.BeginChangeCheck();
    EditorGUI.PropertyField(position, property, label);
    if (EditorGUI.EndChangeCheck())
    {
      var parent = GetParentObject(property.propertyPath, property.serializedObject.targetObject);
      var key = (parent.GetType(), attribute.name);

      if (!_propertyCache.TryGetValue(key, out var info))
      {
        info = key.Item1.GetProperty(attribute.name);
        _propertyCache[key] = info;
      }

      info?.SetValue(parent, fieldInfo.GetValue(parent));
    }
  }

  public static object GetParentObject(string path, object obj)
  {
    string[] parts = path.Split('.');
    for (int i = 0; i < parts.Length - 1; i++)
    {
      var type = obj.GetType();
      var field = type.GetField(parts[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      if (field == null) return null;
      obj = field.GetValue(obj);
    }
    return obj;
  }
}
#endif