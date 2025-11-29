using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PixelCollector.Util.Singletons
{
  public abstract class PrefabSingleton<T> : MonoBehaviour where T : PrefabSingleton<T>
  {
    private AssetReferenceGameObject reference = null;
    private static T instance = null;

    /// <summary>
    /// get 시 어드레서블에서 해당 클래스명 주소로 된 프리팹을 찾아와서 인스턴스합니다.
    /// </summary>
    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        instance = (T)FindAnyObjectByType(typeof(T));
        if (instance != null) return instance;
        
        var reference = new AssetReferenceGameObject(typeof(T).Name);
        var obj = reference.InstantiateAsync().WaitForCompletion();
        obj.GetComponent<PrefabSingleton<T>>().reference = reference;
        
        // 생성된 게임 오브젝트에서 해당 컴포넌트를 instance에 저장한다.
        instance = obj.GetComponent<T>();

        return instance;
      }
    }
    
    /// <summary>
    /// 해당 싱글톤이 활성화되어있는지 여부입니다.
    /// </summary>
    public static bool HasInstance => instance;

    protected virtual void Awake()
    {
      if (instance && instance != this)
        Destroy(gameObject);
      else
        instance = this as T;
    }

    protected virtual void OnDestroy()
    {
      if (reference != null)
      {
        reference.ReleaseInstance(gameObject);
        reference.ReleaseAsset();
      }
      
      instance = null;
    }
  }
}