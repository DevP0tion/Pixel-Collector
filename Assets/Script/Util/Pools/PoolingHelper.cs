using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace PixelCollector.Util
{
  public static class PoolingHelper
  {
    private static PoolingManager Manager => PoolingManager.Instance;
    private static Dictionary<string, string> GUIDNames = new();

    public static string GuidToName(string guid) => GUIDNames.ContainsKey(guid) ? GUIDNames[guid] : "";
    public static void ClearCache() => GUIDNames.Clear();
    
    /// <summary>
    /// 어드레서블 경로의 에셋을 로딩하여 id 키로 풀링매니저에 등록합니다.
    /// </summary>
    /// <param name="id">풀링매니저에 설정할 키입니다.</param>
    /// <param name="path">로딩할 에셋의 어드레서블 경로입니다.</param>
    /// <param name="releaseOnDestroy">PoolingManager가 파괴됬을 시 언로딩 여부입니다.</param>
    /// <returns>오브젝트 풀입니다.</returns>
    public static ObjectPool<PooledObject> Load(string id, AssetReference path, bool releaseOnDestroy = true) => Manager.Register(id, path, releaseOnDestroy);
    
    /// <summary>
    /// 어드레서블 경로의 에셋을 로딩하여 id 키로 풀링매니저에 등록합니다.
    /// </summary>
    /// <param name="id">풀링매니저에 설정할 키입니다.</param>
    /// <param name="assetAddress">로딩할 에셋의 어드레서블 경로입니다.</param>
    /// <returns>오브젝트 풀입니다.</returns>
    public static ObjectPool<PooledObject> Load(string id, string assetAddress) => Load(id, new AssetReference(assetAddress));
    
    /// <summary>
    /// 어드레서블 경로의 에셋을 로딩하여 id 키로 풀링매니저에 등록합니다.
    /// </summary>
    /// <param name="id">로딩할 에셋의 어드레서블 경로입니다.</param>
    /// <returns>오브젝트 풀입니다.</returns>
    public static ObjectPool<PooledObject> Load(string id) => Load(id, id);
    
    /// <summary>
    /// 프리팹의 오브젝트 풀을 없으면 생성해서 풀링합니다.
    /// </summary>
    /// <param name="prefab">풀링할 프리팹입니다.</param>
    /// <returns>풀링된 오브젝트입니다.</returns>
    public static PooledObject Pooling(this GameObject prefab)
    {
      if (!Manager.IsRegistered(prefab.name))
        Manager.Register(prefab.name, prefab);
      
      return Manager.Get(prefab.name);
    }

    public static PooledObject Pooling(string id, bool force = false)
    {
      if(Manager.IsRegistered(id))
        return Manager.Get(id);
      
      return force ? Load(id).Get() : null;
    }

    /// <summary>
    /// 어드레서블 주소를 기반으로 오브젝트를 풀링합니다. <br/>
    /// 이후 같은 오브젝트를 풀링하고자 할 시 해당 프리팹의 이름으로 풀링할 수 있습니다.
    /// </summary>
    /// <param name="reference">풀링하고자 하는 프리팹의 주소입니다.</param>
    /// <returns>풀링된 오브젝트입니다.</returns>
    public static PooledObject Pooling(this AssetReference reference)
    {
      if (!Manager.IsRegistered(GuidToName(reference.AssetGUID)))
      {
        var obj = reference.LoadAssetAsync<GameObject>().WaitForCompletion();
        GUIDNames[reference.AssetGUID] = obj.name;
        return Manager.Register(obj.name, reference).Get();
      }
        
      return Manager.Get(GuidToName(reference.AssetGUID));
    }
    
    /// <summary>
    /// 오브젝트를 해제하거나 파괴합니다.
    /// </summary>
    /// <param name="obj">해제할 오브젝트입니다.</param>
    public static void Release(this PooledObject obj) => Manager.Release(obj);
    
    
    /// <summary>
    /// 오브젝트를 해제하거나 파괴합니다.
    /// </summary>
    /// <param name="obj">해제할 오브젝트입니다.</param>
    public static void Release(this GameObject obj)
    {
      if (obj && obj.TryGetComponent<PooledObject>(out var pooledObject))
      {
        pooledObject.Release();
      }
      else Object.Destroy(obj);
    }
  }
}