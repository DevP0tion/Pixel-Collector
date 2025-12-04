using System;
using PixelCollector.Bullet.Properties;
using PixelCollector.Core;
using PixelCollector.Core.Interface;
using PixelCollector.Unit;
using PixelCollector.Unit.Player;
using PixelCollector.Util;
using UnityEngine;

namespace PixelCollector.Bullet
{
  public class BulletBase : PooledObject, IAttacker
  {
    [field: SerializeField] public Team Team { get; set; }
    public float damage = 1f;
    public float speed = 10f;
    public float lifeTime = 5f;
    public Vector2 direction;
    public Rigidbody2D body;
    public int ownerId;
    
    private BulletProperties properties;
    
    #region Unity Callback

    protected void FixedUpdate()
    {
      if (lifeTime > 0)
      {
        lifeTime -= Time.fixedDeltaTime;
        body.linearVelocity = direction.normalized * speed;
      }
      else if(gameObject.activeSelf)
        Release();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
      if (other.TryGetComponent<IDamageable>(out var damageable))
      {
        if (ownerId != -1 && other.TryGetComponent<PlayerBaseModule>(out var player))
        {
          if (player.connectionToClient.connectionId != ownerId)
          {
            player.Damage(damage, this);
            Release();
            return;
          }
        }
        
        if (damageable is UnitBaseModule unit && unit.team != Team)
        {
          damageable.Damage(damage, this);
          Release();
        }
      }
      else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
      {
        Release();
      }
      
    }

    #endregion

    public BulletProperties Properties
    {
      get => properties;
      set
      {
        properties = value;
      } 
    }
  }
}