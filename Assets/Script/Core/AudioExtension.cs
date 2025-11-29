using PixelCollector.Util;
using UnityEngine;

namespace PixelCollector.Core
{
  public static class AudioExtension
  {
    /// <summary>
    /// 오브젝트에서 사운드 재생을 위한 오디오플레이어를 가져옵니다.
    /// </summary>
    /// <param name="target">타겟 오브젝트입니다.</param>
    /// <returns>오디오 플레이어입니다.</returns>
    public static ObjectAudioPlayer GetAudioPlayer(this GameObject target)
      => target.TryGetComponent(out ObjectAudioPlayer player) ? player : target.AddComponent<ObjectAudioPlayer>();
    
    /// <summary>
    /// 오브젝트에서 사운드 재생을 위한 오디오플레이어를 가져옵니다.
    /// </summary>
    /// <param name="target">타겟 오브젝트입니다.</param>
    /// <returns>오디오 플레이어입니다.</returns>
    public static ObjectAudioPlayer GetAudioPlayer(this Component target)
      => target.TryGetComponent(out ObjectAudioPlayer player) ? player : target.gameObject.AddComponent<ObjectAudioPlayer>();
  }
}