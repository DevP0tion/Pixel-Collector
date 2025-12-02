using Mirror;
using PixelCollector.Bullet.Properties;
using PixelCollector.Networking.Client;
using PixelCollector.Networking.Packet;
using PixelCollector.Networking.Server;
using UnityEngine;

namespace PixelCollector.Unit.Player
{
  public class PlayerBaseModule : UnitBaseModule
  {
    public float speed = 10f;
    public BulletProperties bullet;
    
    protected Vector2 direction;
    
    #region Unity Callback

    protected override void FixedUpdate()
    {
      base.FixedUpdate();
      
      if (direction != Vector2.zero) 
        body.linearVelocity = direction;
    }
    
    #endregion
    
    #region Networking

    public override void OnStartClient()
    {
      base.OnStartClient();

      if (NetworkServer.active)
        NetServerManager.GetPlayer(connectionToClient).playerObject = this;
      else if (isLocalPlayer)
        NetClientManager.LocalPlayer = this;
    }

    #endregion
    
    #region Control

    #if !OFFLINE
    [Command]
    #endif
    public void MoveCommand(MovePacket packet)
    {
      direction = packet.canceled ? Vector2.zero : packet.direction * speed;
    }

    [Command]
    public void ShootCommand(Vector3 target)
    {
      bullet.Shoot(Team, transform.position, target, 1);
    }
    
    #endregion
  }
}