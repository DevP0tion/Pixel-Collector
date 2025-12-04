using System;
using Mirror;
using PixelCollector.Core;
using PixelCollector.Core.Interface;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCollector.Unit
{
  [RequireComponent(typeof(NetworkIdentity))]
  public class UnitBaseModule : NetworkBehaviour, IDamageable
  {
    #region MetaData

    public Team team = Team.None;
    
    #endregion
    
    #region Binding
    
    [SerializeField] protected Rigidbody2D body;
    
    #endregion

    [field:SerializeField] public virtual float MaxHealth { get; protected set; }
    public virtual float Health
    {
      get => realHealth;
      set
      {
        if (value > 0)
          realHealth = value > MaxHealth ? MaxHealth : value;
        else
        {
          realHealth = 0;
          onDeath?.Invoke();
        }
        
        onHealthChanged?.Invoke(realHealth);
      }
    }
    
    private float realHealth;
    
    #region Unity Callback

    protected virtual void FixedUpdate()
    {
    }

    #endregion
    
    #region Event
    
    public UnityEvent<float> onHealthChanged;
    public UnityEvent onDeath;
    
    #endregion

    public void Damage(float damage, IAttacker sender = null)
    {
      Health -= damage;
    }
  }
}