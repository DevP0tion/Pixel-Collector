using UnityEngine;
using UnityEngine.Pool;

namespace PixelCollector.Util
{
  public class PooledObject : MonoBehaviour
  {
    public ObjectPool<PooledObject> pool = null;

    public virtual void Release()
    {
      if (pool != null)
      {
        pool.Release(this);
      }
      else Destroy(gameObject);
    }

    public virtual PooledObject Duplicate() => pool != null ? pool.Get() : gameObject.GetComponent<PooledObject>();

    public static implicit operator GameObject(PooledObject obj) => obj.gameObject;
  }
}