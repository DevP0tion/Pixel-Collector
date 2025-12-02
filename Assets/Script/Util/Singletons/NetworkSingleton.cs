using Mirror;
using UnityEngine;

namespace PixelCollector.Util.Singletons
{
  public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
  {
    private static T instance;

    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        // 해당 컴포넌트를 가지고 있는 게임 오브젝트를 찾아서 반환한다.
        instance = (T)FindAnyObjectByType(typeof(T));

        return instance; // 인스턴스를 찾지 못한 경우
      }
    }
    
    public static bool IsActive => instance != null;

    protected virtual void OnDestroy()
    {
      instance = null;
    }
  }
}