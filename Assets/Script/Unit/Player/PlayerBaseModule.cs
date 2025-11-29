using Mirror;
using PixelCollector.Networking.Packet;
using UnityEngine;

namespace PixelCollector.Unit.Player
{
  public class PlayerBaseModule : UnitBaseModule
  {
    public float speed = 10f;
    
    protected Vector2 direction;
    
    #region Unity Callback

    protected override void FixedUpdate()
    {
      base.FixedUpdate();
      
      if (direction != Vector2.zero) 
        body.linearVelocity = direction;
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
    
    #endregion
  }
}