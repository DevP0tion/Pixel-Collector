using System.Linq;
using PixelCollector.Core;
using PixelCollector.Util;
using UnityEditor;
using UnityEngine;

namespace PixelCollector.Editor
{
  public class DebugSymbolEditor : EditorWindow
  {
    [InitializeOnLoadMethod]
    private static void Inititalize()
    {
      Defines.DynamicLoading = EditorPrefs.GetBool(nameof(Defines.DynamicLoading), Defines.DynamicLoading);
    }
    
    [MenuItem("Debug/Symbol Editor")]
    private static void ShowWindow()
    {
      var window = GetWindow<DebugSymbolEditor>();
      window.titleContent = new GUIContent("Symbol Editor");
      window.Show();
    }

    private string tempText;
    
    private void OnGUI()
    {
      var symbolList = DebugSymbol.Symbols.ToArray();

      foreach (var symbol in symbolList)
      {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(symbol.name);
        symbol.isActive = EditorGUILayout.Toggle(symbol);

        if (!symbol.dontDestroy && GUILayout.Button("X")) symbol.Release();
        
        EditorGUILayout.EndHorizontal();
      }
      
      tempText = EditorGUILayout.TextField(tempText);
      if (GUILayout.Button("디버그 심볼 추가"))
      {
        DebugSymbol.Get(tempText, true, false);
      }

      // 스테이지 동적 로딩
      {
        var dynamicLoading = EditorGUILayout.Toggle("스테이지 동적 로딩", Defines.DynamicLoading);
        
        Defines.DynamicLoading = dynamicLoading;
        EditorPrefs.SetBool(nameof(Defines.DynamicLoading), dynamicLoading);
      }
    }
  }
}