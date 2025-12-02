using System;

namespace PixelCollector.Util.Singletons
{
  using UnityEngine;

  public class Singleton<T> : MonoBehaviour where T : Singleton<T>
  {
    protected static T instance;

    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        // 해당 컴포넌트를 가지고 있는 게임 오브젝트를 찾아서 반환한다.
        instance = (T)FindAnyObjectByType(typeof(T));

        if (instance != null) return instance; // 인스턴스를 찾지 못한 경우
        
        // 새로운 게임 오브젝트를 생성하여 해당 컴포넌트를 추가한다.
        var obj = new GameObject(typeof(T).Name, typeof(T));
        
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
      if (instance == null)
        instance = (T)this;
      else if (instance != this)
        Destroy(gameObject);
    }
    
    protected virtual void OnDestroy()
    {
      if (instance == this) instance = null;
    }
  }
}