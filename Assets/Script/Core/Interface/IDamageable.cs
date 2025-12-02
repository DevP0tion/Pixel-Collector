using UnityEngine;

namespace PixelCollector.Core.Interface
{
  public interface IDamageable
  {
    Team Team { get; }
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
    /// <param name="sender">공격하는 주체입니다.</param>
    void Damage(float damage, IAttacker sender = null);
  }

  public static class IDamageableExtensions
  {
    /// <summary>
    /// 오브젝트에 바로 피해를 줄 수 있게해주는 확장 메서드입니다.
    /// </summary>
    /// <param name="obj">IDamageable 컴포넌트가 붙어있는 오브젝트입니다.</param>
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
    /// <param name="sender">공격하는 주체입니다.</param>
    public static void Damage(this GameObject obj, float damage, IAttacker sender = null)
    {
      if (obj && obj.TryGetComponent(out IDamageable damageable))
      {
        damageable.Damage(damage, sender);
      }
    }

    /// <summary>
    /// 오브젝트에 바로 피해를 줄 수 있게해주는 확장 메서드입니다.
    /// </summary>
    /// <param name="coll">IDamageable 컴포넌트가 붙어있는 오브젝트입니다.</param>
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
    /// <param name="sender">공격하는 주체입니다.</param>
    public static bool Damage(this Collider2D coll, float damage, IAttacker sender)
    {
      if (coll && coll.TryGetComponent(out IDamageable damageable))
      {
        damageable.Damage(damage, sender);
        return true;
      }
      
      return false;
    }
  }
}