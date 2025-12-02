using System;
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
    private static void ShootFunc(BulletPacket packet)
    {
      var bullet = packet.Type.Pooling();
      bullet.transform.position = packet.startPos;
      bullet.Team = packet.Team;
      bullet.transform.rotation = ((Vector2)bullet.transform.position).GetDirection(packet.targetPos);
      bullet.direction = bullet.transform.rotation.ToVector2Direction();
      bullet.damage = packet.damage;
      bullet.lifeTime = packet.lifeTime;
    }

    public static void Shoot(BulletPacket packet)
    {
      if (NetworkServer.active)
      {
        ShootFunc(packet);
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
      
      ShootFunc(packet);
    }
    
    #endregion
    
    #region Unity Event

    
    
    #endregion
  }
}