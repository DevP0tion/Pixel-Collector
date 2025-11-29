using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using PixelCollector.Core.Manager;
using UnityEngine;

namespace PixelCollector.Util
{
  public class ObjectAudioPlayer : MonoBehaviour
  {
    [SerializeField, ReadOnly] private List<AudioSource> sourcePool = new();
    [SerializeField] private Transform audioSourceParent;
    private float Volume => (AudioManager.ObjectVolume / 100) * (AudioManager.MasterVolume / 100);
    
    #region Unity Events
#if UNITY_EDITOR

    private void Reset()
    {
      if (!audioSourceParent)
      {
        var obj = new GameObject("Audio Sources");
        obj.transform.SetParent(transform);
        audioSourceParent = obj.transform;
      }
    }

#endif
    private void Awake()
    {
      if (!audioSourceParent)
      {
        var obj = new GameObject("Audio Sources");
        obj.transform.SetParent(transform);
        audioSourceParent = obj.transform;
      }
      
      AudioManager.Instance?.onObjectVolumeChanged.AddListener(UpdateVolume);
    }

    private void OnDestroy()
    {
      if(AudioManager.HasInstance) AudioManager.Instance.onObjectVolumeChanged.RemoveListener(UpdateVolume);
      Destroy(audioSourceParent.gameObject);
    }
    
    #endregion

    private void UpdateVolume()
    {
      if(sourcePool == null || sourcePool.Count == 0) return;

      foreach (var source in sourcePool)
      {
        source.volume = Volume;
      }
    }

    /// <summary>
    /// 현재 재생중인 클립들을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetPlayList()
      => from source in sourcePool where source.isPlaying select source.clip.name;
    
    /// <summary>
    /// 어드레서블 참조를 기반으로 사운드를 재생합니다.
    /// </summary>
    /// <param name="reference">재생할 클립의 어드레서블 참조입</param>
    /// <param name="loop">반복재생 여부</param>
    /// <returns>재생하는 오디오 소스를 반환합니다.</returns>
    public AudioSource Play(ReferenceWrapper reference, bool loop = false)
      => AudioManager.TryGetClip(reference, out var clip, true) ? Play(clip, loop) : null;

    /// <summary>
    /// 문자열 키를 기반으로 사운드를 재생합니다.
    /// </summary>
    /// <param name="clipName">재생할 클립의 파일명</param>
    /// <param name="loop">반복재생 여부</param>
    /// <returns>재생하는 오디오 소스를 반환합니다.</returns>
    public AudioSource Play(string clipName, bool loop = false)
      => AudioManager.TryGetClip(clipName, out var clip, true) ? Play(clip, loop) : null;

    /// <summary>
    /// 오디오클립을 받아서 사운드를 재생합니다.
    /// </summary>
    /// <param name="clip">재생할 오디오클립입니다.</param>
    /// <param name="loop">반복재생 여부</param>
    /// <returns>재생하는 오디오 소스를 반환합니다.</returns>
    public AudioSource Play(AudioClip clip, bool loop = false)
    {
      if (clip == null) return null;
      var source = (from target in sourcePool where !target.isPlaying select target).FirstOrDefault();

      if (source == null)
      {
        source = new GameObject("AudioSource").AddComponent<AudioSource>();
        source.transform.SetParent(audioSourceParent);
        source.transform.localPosition = Vector3.zero;
        source.transform.localRotation = Quaternion.identity;
        source.transform.localScale = Vector3.one;
        sourcePool.Add(source);

        source.outputAudioMixerGroup = AudioManager.ObjectMixer;
        source.spatialBlend = 1;
      }

      source.clip = clip;
      source.loop = loop;
      source.volume = Volume;
      source.Play();
      
      return source;
    }
    
    public void PlayInAnimationEvent(string key) => Play(key, false);

    /// <summary>
    /// 재생중인 모든 클립을 중지시킵니다.
    /// </summary>
    public void StopAll()
    {
      foreach (var source in sourcePool)
        source.Stop();
    }
    
    /// <summary>
    /// 문자열 키를 기반으로 재생중인 사운드를 중지시킵니다.
    /// </summary>
    /// <param name="clipName">중지할 클립의 파일명</param>
    public void Stop(string clipName)
    {
      if (sourcePool == null || sourcePool.Count == 0) return;

      foreach (var source in sourcePool.Where(source => source.clip.name == clipName))
        source.Stop();
    }

    /// <summary>
    /// 오디오클립을 기반으로 재생중인 사운드를 중지시킵니다.
    /// </summary>
    /// <param name="clip">중지할 클립</param>
    public void Stop(AudioClip clip)
    {
      if (sourcePool == null || sourcePool.Count == 0) return;
      
      foreach (var source in sourcePool.Where(source => source.clip == clip))
        source.Stop();
    }

    /// <summary>
    /// 어드레서블 참조를 기반으로 재생중인 사운드를 중지시킵니다.
    /// </summary>
    /// <param name="reference">중지할 클립의 어드레서블 참조</param>
    public void Stop(ReferenceWrapper reference)
    {
      if (sourcePool == null || sourcePool.Count == 0) return;
      var clipName = reference.path.Replace($"{AudioManager.Label}/", "");
      
      foreach (var source in sourcePool.Where(source => source.clip.name == clipName))
        source.Stop();
    }
  }
}