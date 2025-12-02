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
    
    private BulletProperties properties;
    
    #region Unity Callback

    protected void FixedUpdate()
    {
      
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
    
    public virtual void Shoot(Vector2 direction)
    {
      
    }
  }
}