using System.Collections.Generic;
using Mirror;
using PixelCollector.Bullet;
using PixelCollector.Bullet.Properties;
using PixelCollector.Networking.Packet;
using PixelCollector.Util;
using PixelCollector.Util.Singletons;
using UnityEngine;
using UnityEngine.Pool;

namespace PixelCollector.Core.Manager
{
  public class BulletManager : NetworkSingleton<BulletManager>
  {
    #region Poolling
    
    [SerializeField] private Transform released;
    [SerializeField] private List<GameObject> instances = new();

    /// <summary>
    ///   불릿 매니저 인스턴스가 바뀔 경우를 대비해 인스턴스 변수로 선언
    /// </summary>
    private readonly Dictionary<string, ObjectPool<BulletBase>> pools = new();

    private ObjectPool<BulletBase> InitPool(string id, GameObject bulletPrefab)
    {
      if (pools.TryGetValue(id, out var pool)) return pool;

      return pools[id] = new ObjectPool<BulletBase>(() =>
        {
          // Create
          var bullet = Instantiate(bulletPrefab).GetComponent<BulletBase>();
          instances.Add(bullet.gameObject);
          bullet.transform.SetParent(Instance.transform);
          return bullet;
        },
        bullet =>
        {
          // Get
          bullet.gameObject.SetActive(true);
          bullet.transform.SetParent(Instance.transform);
        },
        bullet =>
        {
          // Release
          bullet.gameObject.SetActive(false);
          bullet.transform.SetParent(Instance.released);
        },
        bullet =>
        {
          // Destroy
          Destroy(bullet.gameObject);
        }
      );
    }

    #endregion
    
    private static void ShootFunc(BulletProperties properties, Vector3 startPos, Vector3 targetPosition, Team team, float damage)
    {
      var bullet = properties.Pooling();
      bullet.transform.position = startPos;
      bullet.Team = team;
      bullet.transform.rotation = ((Vector2)bullet.transform.position).GetDirection(targetPosition);
      bullet.direction = bullet.transform.rotation.ToVector2Direction();
      bullet.damage = damage;
    }

    public static void Shoot(BulletPacket packet)
    {
      if (NetworkServer.active)
      {
        ShootFunc(packet.Type, packet.startPos, packet.targetPos, packet.Team, packet.damage);
        Instance.ShootRpc(packet);
      }
      else
      {
        Instance.ShootRequest(packet);
      }
    }
    
    #region Networking

    [Command(requiresAuthority = false)]
    private void ShootRequest(BulletPacket packet) => Shoot(packet);

    [ClientRpc]
    private void ShootRpc(BulletPacket packet)
    {
      if(NetworkServer.active) return;
      
      ShootFunc(BulletProperties.Bullets[packet.typeName], packet.startPos, packet.targetPos, packet.Team, packet.damage);
    }
    
    #endregion
    
    #region Unity Event

    
    
    #endregion
  }
}