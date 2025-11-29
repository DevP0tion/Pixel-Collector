using UnityEngine;

namespace PixelCollector.Util.Singletons
{
  public class DDOLSingleton<T> : Singleton<T> where T : DDOLSingleton<T>
  {
    protected virtual void Awake()
    {
      if (instance && instance != this)
      {
        Destroy(gameObject);
        return;
      }

      instance = this as T;
      
      if(transform.parent != null && transform.root != null) // 해당 오브젝트가 자식 오브젝트라면
        DontDestroyOnLoad(transform.root.gameObject); // 부모 오브젝트를 DontDestroyOnLoad 처리
      else
        DontDestroyOnLoad(gameObject); // 해당 오브젝트가 최 상위 오브젝트라면 자신을 DontDestroyOnLoad 처리
    }
  }
}