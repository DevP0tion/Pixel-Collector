using System;
using System.Collections.Generic;
using System.Linq;
using PixelCollector.Util.Singletons;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace PixelCollector.Util
{
  public class PoolingManager : Singleton<PoolingManager>
  {
    private readonly Dictionary<string, ObjectPool<PooledObject>> pools = new();
    [SerializeField] private SerializableDictionaryBase<string, Transform> releasedContainers = new();
    [SerializeField] private SerializableDictionaryBase<string, AssetReference> assetReferences = new();
    private HashSet<AssetReference> refSet = new();
    
    /// <summary>
    /// key 값의 오브젝트를 풀링합니다.
    /// </summary>
    /// <param name="key">풀링할 게임오브젝트의 키입니다.</param>
    /// <returns>풀링한 게임오브젝트입니다.</returns>
    public PooledObject Get(string key)
    {
      if (pools.TryGetValue(key, out var pool))
      {
        return pool.Get();
      }
      
      throw new MissingReferenceException($"{key}는 존재하지 않는 오브젝트 풀입니다.");
    }

    /// <summary>
    /// AssetReference 확인 용도 메서드입니다.
    /// </summary>
    /// <param name="key"></param>
    public AssetReference GetRef(string key) => assetReferences[key];
    
    /// <summary>
    /// 풀링한 오브젝트를 해제합니다. <br/>
    /// 풀링된 오브젝트가 아닐경우 파괴합니다.
    /// </summary>
    /// <param name="obj">해제할 오브젝트입니다.</param>
    public void Release(PooledObject obj)
      => obj.Release();

    /// <summary>
    /// 프리팹을 key의 오브젝트 풀로 생성합니다.
    /// </summary>
    /// <param name="key">풀의 키입니다.</param>
    /// <param name="prefab">풀의 원본 프리팹입니다.</param>
    /// <returns>생성된 오브젝트 풀입니다.</returns>
    public ObjectPool<PooledObject> Register(string key, GameObject prefab)
    {
      if (pools.ContainsKey(key)) throw new Exception($"해당 {key} 풀은 이미 생성되어있습니다.");

      var container = new GameObject(key);
      container.transform.SetParent(transform);
      releasedContainers[key] = container.transform;
      
      var result = new ObjectPool<PooledObject>(() =>
      {
        // create
        var obj = Instantiate(prefab, container.transform);

        if (!obj.TryGetComponent(out PooledObject pooledObject))
        {
          pooledObject = obj.AddComponent<PooledObject>();
        }

        return pooledObject;
      }, obj =>
      {
        // get
        obj.pool = pools[key];
        obj.gameObject.SetActive(true);
      }, obj =>
      {
        // release
        obj.gameObject.SetActive(false);
        if (container)
          obj.transform.SetParent(container.transform);
        obj.pool = null;

      }, Destroy);

      pools.Add(key, result);
      return result;
    }

    /// <summary>
    /// 어드레서블 경로의 프리팹을 풀링매니저에 등록합니다.
    /// </summary>
    /// <param name="key">등록할 키입니다.</param>
    /// <param name="reference">프리팹의 어드레서블 경로입니다.</param>
    /// <param name="releaseOnDestroy">매니저가 파괴될 시 에셋을 언로딩하는지 여부입니다.</param>
    /// <returns>등록된 오브젝트 풀입니다.</returns>
    public ObjectPool<PooledObject> Register(string key, AssetReference reference, bool releaseOnDestroy = true)
    {
      if (pools.ContainsKey(key)) throw new Exception($"해당 {key} 풀은 이미 생성되어있습니다.");
      if (releaseOnDestroy)
      {
        assetReferences[key] = reference;
        refSet.Add(reference);
      }
      
      var obj = reference.IsValid() ? (GameObject)reference.Asset : reference.LoadAssetAsync<GameObject>().WaitForCompletion();

      return Register(key, obj);
    }
    
    /// <param name="key">확인할 풀의 키 입니다.</param>
    /// <returns>해당 키의 오브젝트 풀이 만들어져있는지 여부</returns>
    public bool IsRegistered(string key) => pools.ContainsKey(key);

    public bool IsRegistered(AssetReference reference) => refSet.Contains(reference);

    #region Unity Event
    
    private void OnDestroy()
    {
      foreach (var (_, pool) in pools)
      {
        pool.Dispose();
      }
      
      pools.Clear();

      foreach (var pair in assetReferences)
      {
        if(pair.Value.IsValid())
          pair.Value.ReleaseAsset();
      }
      
      assetReferences.Clear();
      refSet.Clear();
      PoolingHelper.ClearCache();
    }
    
    #endregion
  }
}