using System;
using PixelCollector.Bullet.Properties;
using PixelCollector.Core;
using PixelCollector.Core.Interface;
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
      Debug.Log("Bullet hit " + other.gameObject.name);
      if (other.TryGetComponent<IDamageable>(out var damageable))
      {
        if (damageable.Team != Team)
        {
          Debug.Log("Bullet damaged " + other.gameObject.name);
          damageable.Damage(damage, this);
          Release();
        }
      }
      else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
      {
        Debug.Log("Bullet hit obstacle");
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