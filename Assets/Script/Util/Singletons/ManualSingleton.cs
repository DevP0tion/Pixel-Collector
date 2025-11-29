using UnityEngine;

namespace PixelCollector.Util.Singletons
{
  public class ManualSingleton<T> : MonoBehaviour where T : ManualSingleton<T>
  {
    protected static T instance;

    /// <summary>
    /// 싱글톤이 존재할 시 해당 싱글톤을 반환하고, 아니라면 Null을 반환합니다.
    /// </summary>
    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        // 해당 컴포넌트를 가지고 있는 게임 오브젝트를 찾아서 반환한다.
        instance = (T)FindAnyObjectByType(typeof(T));

        return instance;
      }
    }

    /// <summary>
    /// 해당 싱글톤이 활성화되어있는지 여부입니다.
    /// </summary>
    public static bool HasInstance => instance;
    
    private void OnDestroy()
    {
      if (instance == this) instance = null;
    }
  }
}