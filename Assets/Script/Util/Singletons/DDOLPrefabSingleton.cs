using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PixelCollector.Util.Singletons
{
  public class DDOLPrefabSingleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T instance;

    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        var reference = new AssetReferenceGameObject(instance.GetType().Name);
        var obj = reference.InstantiateAsync().WaitForCompletion();
        reference.ReleaseAsset();
        
        // 생성된 게임 오브젝트에서 해당 컴포넌트를 instance에 저장한다.
        instance = obj.GetComponent<T>();
        DontDestroyOnLoad(obj);

        return instance;
      }
    }
  }
}