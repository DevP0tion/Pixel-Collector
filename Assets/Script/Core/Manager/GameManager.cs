using NaughtyAttributes;
using PixelCollector.Bullet.Properties;
using PixelCollector.Util.Singletons;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Tables;

namespace PixelCollector.Core.Manager
{ 
  public class GameManager : DDOLSingleton<GameManager>
  {
    
    /// <summary>
    /// 현재 활성화된 언어의 키에 맞는 문자열을 가져올 수 있습니다.
    /// GameManager.Instance.Localization["가져올 문자열의 키"]로 값을 가져올 수 있습니다.
    /// </summary>
    // public StringTable Localization => (StringTable)localization.GetTable(LocalizationSettings.SelectedLocale);

    #if UNITY_EDITOR
    // 유니티 에디터 전용 에셋 로딩 확인용 코드
    
    public SerializableDictionaryBase<string, BulletProperties> loadedBullets = new();
    public SerializableDictionaryBase<string, AudioClip> loadedAudios = new();
    #endif

    [Button]
    private void Test()
    {
      // var entry = tableData.GetEntry("IntroScene_loading");
      // Addressables.LoadAssetsAsync<StringTable>(new AssetLabelReference { labelString = "Locale" }, table =>
      // {
      //   Debug.Log(table.LocaleIdentifier);
      // });
      var generalTableData = Addressables.LoadAssetAsync<SharedTableData>(new AssetReference("General/Shared_Data")).WaitForCompletion();

      Debug.Log(generalTableData);
    }
  }
}
