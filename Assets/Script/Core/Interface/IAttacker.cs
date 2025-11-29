using PixelCollector.Core;
using UnityEngine;

namespace PixelCollector.Core.Interface
{
  public interface IAttacker
  {
    /// <summary>
    /// 현재 엔티티의 팀입니다. <br/>
    /// 플레이어는 Player, 적은 Enemy, 버프는 None으로 설정되어있습니다. <br/>
    /// 서로 다른 팀에만 피해를 입힐 수 있습니다.
    /// </summary>
    Team Team { get; }
  }
}