using PixelCollector.Core;
using PixelCollector.Core.Manager;
using PixelCollector.Util.Extensions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
#pragma warning disable CS0612 // 형식 또는 멤버는 사용되지 않습니다.

public static class AddressableAssetImporter
{
  private const string InitAudioAssetName = "Assets/Asset Initializer/Init Audio Asset";
  private const string ConvertLinkName = "Assets/Asset Initializer/Convert Link";
  
  [MenuItem(InitAudioAssetName, true)]
  private static bool ValidateInitAudioAsset()
  {
    foreach (var obj in Selection.objects)
    {
      if(obj is AudioClip)
        return true;
    }
    
    return false;
  }

  /// <summary>
  /// 오디오 에셋의 어드레서블 주소 설정 자동화
  /// </summary>
  [MenuItem(InitAudioAssetName)]
  private static void InitAudioAsset()
  {
    // 어드레서블 설정 찾기
    var settings = AddressableAssetSettingsDefaultObject.Settings;
    if (settings == null)
    {
      Debug.LogError("AddressableAssetSettings 파일이 존재하지 않습니다. Addressables이 설정되지 않았을 수 있습니다.");
      return;
    }
    
    var group = settings.FindGroup("Audio Assets");
    if (group == null)
    {
      Debug.LogError("Audio Assets 어드레서블 그룹이 존재하지 않아요!");
      return;
    }
    
    foreach (var obj in Selection.objects)
    {
      if (obj is not AudioClip) continue;
      
      string path = AssetDatabase.GetAssetPath(obj),
        guid = AssetDatabase.AssetPathToGUID(path);
      var entry = settings.FindAssetEntry(guid);

      if (entry != null)
      {
        if (entry.parentGroup != group)
        {
          settings.RemoveAssetEntry(guid);
          entry = settings.CreateOrMoveEntry(guid, group);
        }
      }
      else
        entry = settings.CreateOrMoveEntry(guid, group);

      entry.SetAddress($"{AudioManager.Label}/{obj.name}");
      entry.SetLabel(AudioManager.Label, true);
    }
    
    AssetDatabase.SaveAssets();
    EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
  }

  [MenuItem(ConvertLinkName)]
  private static void ConvertRoomLink()
  {
    foreach (var target in Selection.objects)
    {
      if (target is GameObject obj)
      {
        obj.RunAllObject(child =>
        {
        });
        
        EditorUtility.SetDirty(target);
      }
    }
  }
  
}
