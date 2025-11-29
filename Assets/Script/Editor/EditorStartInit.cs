using UnityEditor;
using UnityEditor.SceneManagement;

namespace PixelCollector.Editor
{
  public static class EditorStartInit
  {
    [MenuItem("Debug/시작 씬부터 시작")]
    public static void StartInitScene()
    {
      var pathOfFirstScene = EditorBuildSettings.scenes[0].path; // 씬 번호를 넣어주자.
      var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
      EditorSceneManager.playModeStartScene = sceneAsset;
      EditorApplication.isPlaying = true;
    }
    
    [MenuItem("Debug/현재 씬부터 시작")]
    public static void StartFromThisScene()
    {
      EditorSceneManager.playModeStartScene = null;
      EditorApplication.isPlaying = true;
    }
  }
}