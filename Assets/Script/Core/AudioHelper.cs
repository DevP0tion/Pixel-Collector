using PixelCollector.Core.Manager;
using PixelCollector.Util;
using UnityEngine;
using AudioType = PixelCollector.Core.Manager.AudioType;

namespace PixelCollector.Core
{
  [ AddComponentMenu( "Audio/Audio Helper" )]
  public class AudioHelper : MonoBehaviour
  {
    #region State 
    #if UNITY_EDITOR
    
    [SerializeField, Range(0, 100), GetSet(nameof(MasterVolume))] public float masterVolume = 0;
    [SerializeField, Range(0, 100), GetSet(nameof(BackgroundVolume))] public float backgroundVolume = 0;
    [SerializeField, Range(0, 100), GetSet(nameof(EffectVolume))] public float effectVolume = 0;
    [SerializeField, Range(0, 100), GetSet(nameof(ObjectVolume))] public float objectVolume = 0;
    
    #endif

    public float MasterVolume
    {
      get => AudioManager.MasterVolume;
      set => AudioManager.MasterVolume = value;
    }

    public float BackgroundVolume
    {
      get => AudioManager.BackgroundVolume;
      set => AudioManager.BackgroundVolume = value;
    }

    public float EffectVolume
    {
      get => AudioManager.EffectVolume;
      set => AudioManager.EffectVolume = value;
    }

    public float ObjectVolume
    {
      get => AudioManager.ObjectVolume;
      set => AudioManager.ObjectVolume = value;
    }
    
    #endregion
    
    #region Controller
    
    public void Play(AudioClip clip, AudioType type)
    {
      if (!clip) return;
      AudioManager.Play(clip, type);
    }

    public void PlayBackground(AudioClip clip)
    {
      if (!clip) return;
      AudioManager.Play(clip, AudioType.Background);
    }

    public void PlayBackground(string clipName) => AudioManager.Play(clipName, AudioType.Background);
    
    public void PlayEffect(AudioClip clip)
    {
      if (!clip) return;
      AudioManager.Play(clip, AudioType.Effect);
    }
    
    public void PlayEffect(string clipName) => AudioManager.Play(clipName, AudioType.Effect);

    public void Stop(AudioType type)
    {
      AudioManager.Stop(type);
    }
    
    #endregion
  }
}