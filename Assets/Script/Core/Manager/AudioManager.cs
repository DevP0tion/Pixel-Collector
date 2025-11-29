using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using PixelCollector.Util;
using PixelCollector.Util.Singletons;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace PixelCollector.Core.Manager
{
  public class AudioManager : PrefabSingleton<AudioManager>
  {
    public const string ObjectLabel = "Object"; 
    public const string Label = "Audio";

    private static readonly Dictionary<string, AudioClip> clips = new();
    private static readonly Dictionary<string, AssetReference> references = new();
    
    public static AudioMixerGroup ObjectMixer => Instance.objectGroup;

    #region Binding

    private const string Binding = "Binding";
    
    [Foldout(Binding), SerializeField] private AudioMixer mixer;
    [Foldout(Binding), SerializeField] private AudioSource backgroundSource = null;
    [Foldout(Binding), SerializeField] private List<AudioSource> effectSources = new(); 
    [Foldout(Binding), SerializeField] private AudioMixerGroup masterGroup, effectGroup, backgroundGroup, objectGroup;
    
#if UNITY_EDITOR
    [Foldout(Binding), SerializeField] private SerializableDictionaryBase<string, AudioClip> loadedClips = new();
#endif
    
    #endregion

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
      masterVolume = PlayerPrefs.GetFloat(nameof(MasterVolume), 80);
      backgroundVolume = PlayerPrefs.GetFloat(nameof(BackgroundVolume), 80);
      effectVolume = PlayerPrefs.GetFloat(nameof(EffectVolume), 80);
      objectVolume = PlayerPrefs.GetFloat(nameof(ObjectVolume), 80);
    }
    
    #region Unity Event

    protected override void Awake()
    {
      base.Awake();
      
      MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 80);
      BackgroundVolume = PlayerPrefs.GetFloat("BackgroundVolume", 80);
      EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 80);
      ObjectVolume = PlayerPrefs.GetFloat("ObjectVolume", 80);
    }
    
    private void FixedUpdate()
    {
      transform.eulerAngles = Vector3.zero;
    }

    protected override void OnDestroy()
    {
      onObjectVolumeChanged.RemoveAllListeners();
    }
    
    #endregion
    
    #region Volumes

    private void SetEffectVolume(float value)
    {
      if(effectSources == null) return;
      
      foreach (var source in effectSources)
      {
        source.volume = value;
      }
    }

    private static float masterVolume = 80;
    /// <summary>
    /// 각 볼륨을 총괄하는 마스터 볼륨입니다.
    /// 해당 볼륨을 변경시 다른 모든 볼륨이 영향받습니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float MasterVolume
    {
      get => masterVolume;
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
        {
          // Instance.SetEffectVolume((input / 100) * (BackgroundVolume / 100));
          // Instance.backgroundSource.volume = (input / 100) * (EffectVolume / 100);
          float dB = Mathf.Log10(Mathf.Clamp(input / 100, 0.0001f, 1f)) * 20;
          Instance.mixer.SetFloat("Master", dB);
          Instance.onObjectVolumeChanged.Invoke();
        }
        
        masterVolume = input;
        PlayerPrefs.SetFloat(nameof(MasterVolume), input);
      }
    }

    private static float backgroundVolume = 80;
    /// <summary>
    /// 배경 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float BackgroundVolume
    {
      get => backgroundVolume;
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
        {
          // Instance.backgroundSource.volume = (input / 100) * (MasterVolume / 100);
          float dB = Mathf.Log10(Mathf.Clamp(input / 100, 0.0001f, 1f)) * 20;
          Instance.mixer.SetFloat("Background", dB);
        }

        backgroundVolume = input;
        PlayerPrefs.SetFloat(nameof(BackgroundVolume), input);
      }
    }

    private static float effectVolume = 80;
    /// <summary>
    /// 효과음 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float EffectVolume
    {
      get => effectVolume;
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
        {
          float dB = Mathf.Log10(Mathf.Clamp(input / 100, 0.0001f, 1f)) * 20;
          Instance.mixer.SetFloat("Object", dB);
          Instance.mixer.SetFloat("UI", dB);
          //Instance.SetEffectVolume((input / 100) * (MasterVolume / 100));
        }

        effectVolume = input;
        PlayerPrefs.SetFloat(nameof(EffectVolume), input);
      }
    }
    
    private static float objectVolume = 80;
    /// <summary>
    /// 오브젝트 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float ObjectVolume
    {
      get => objectVolume;
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
        {
          Instance.onObjectVolumeChanged.Invoke();
        }
        
        objectVolume = input;
        PlayerPrefs.SetFloat(nameof(ObjectVolume), input);
      }
    }

    #endregion
    
    #region Event
    
    public UnityEvent onObjectVolumeChanged = new();
    
    #endregion
    
    #region Controller

    /// <summary>
    /// name 명칭의 오디오 클립을 불러옵니다. <br/>
    /// 불러오는데 실패할 경우 Null을 반환합니다.
    /// </summary>
    /// <param name="name">불러올 오디오 클립의 파일명입니다.</param>
    /// <param name="forceLoad">참일 때 메모리에 없을 경우 불러옵니다.</param>
    /// <returns>불러온 오디오클립입니다.</returns>
    public static AudioClip GetClip(string name, bool forceLoad = false)
    {
      if (clips.TryGetValue(name, out var clip)) return clip;
      if (!forceLoad) return null;

      var reference = new AssetReference($"{Label}/{name}");

      try
      {
        clip = reference.LoadAssetAsync<AudioClip>().WaitForCompletion();
        
        clips[name] = clip;
        references[name] = reference;
      }
      catch (Exception e)
      {
        Debug.LogError(e);
        return null;
      }

#if UNITY_EDITOR
      if(HasInstance)
        Instance.loadedClips[name] = clip;
#endif
      
      return clip;
    }

    /// <summary>
    /// 어드레서블 참조를 기반으로 클립을 불러옵니다.
    /// </summary>
    /// <param name="reference">불러올 클립의 어드레서블 참조입니다.</param>
    /// <param name="forceLoad">참일 때 메모리에 없을 경우 불러옵니다.</param>
    /// <returns>불러온 오디오클립입니다.</returns>
    public static AudioClip GetClip(ReferenceWrapper reference, bool forceLoad = false)
    {
      if(reference == null) return null;
      var name = reference.path.Replace($"{Label}/", "");
      
      if (clips.TryGetValue(name, out var clip)) return clip;
      if (!forceLoad) return null;

      try
      {
        clip = reference.assetReference.LoadAssetAsync<AudioClip>().WaitForCompletion();
        
        references[name] = reference.assetReference;
        clips[name] = clip;
      }
      catch (Exception e)
      {
        Debug.LogError(e);
        return null;
      }
      
#if UNITY_EDITOR
      if(HasInstance)
        Instance.loadedClips[name] = clip;
#endif
      
      return clip;
    }

    /// <summary>
    /// name 명칭의 오디오 클립을 불러와서 clip으로 넘겨줍니다.
    /// </summary>
    /// <param name="name">불러올 클립의 파일명입니다.</param>
    /// <param name="clip">불러온 클립입니다.</param>
    /// <param name="forceLoad">참일 때 메모리에 없을 경우 불러옵니다.</param>
    /// <returns>정상적으로 불러왔는지 여부입니다.</returns>
    public static bool TryGetClip(string name, out AudioClip clip, bool forceLoad = false)
    {
      clip = GetClip(name, forceLoad);
      
      return clip != null;
    }

    /// <summary>
    /// name 명칭의 오디오 클립을 불러와서 clip으로 넘겨줍니다.
    /// </summary>
    /// <param name="reference">불러올 클립의 어드레서블 참조입니다.</param>
    /// <param name="clip">불러온 클립입니다.</param>
    /// <param name="forceLoad">참일 때 메모리에 없을 경우 불러옵니다.</param>
    /// <returns>정상적으로 불러왔는지 여부입니다.</returns>
    public static bool TryGetClip(ReferenceWrapper reference, out AudioClip clip, bool forceLoad = false)
    {
      clip = GetClip(reference, forceLoad);
      
      return clip != null;
    }

    /// <summary>
    /// 불러온 오디오 클립을 메모리에서 해제합니다.
    /// </summary>
    /// <param name="name">해제할 클립의 명칭입니다.</param>
    /// <returns>정상적으로 해제했는지 여부입니다.</returns>
    public static bool ReleaseClip(string name)
    {
      if (references.TryGetValue(name, out var reference))
      {
        clips[name].UnloadAudioData();
        clips.Remove(name);
        reference.ReleaseAsset();
        
#if UNITY_EDITOR
        if (HasInstance)
          Instance.loadedClips.Remove(name);
#endif
        
        return true;
      }
      else return false;
    }
    
    /// <summary>
    /// 오디오클립을 받아와서 사운드를 재생합니다.
    /// </summary>
    /// <param name="clip">재생할 클립</param>
    /// <param name="type">재생할 사운드의 종류</param>
    public static void Play(AudioClip clip, AudioType type = AudioType.Effect)
    {
      if(!HasInstance) return;

      if (type == AudioType.Background)
      {
        Instance.backgroundSource.clip = clip;
        Instance.backgroundSource.Play();
      }
      else
      {
        var availableSource = (from source in Instance.effectSources where source.isPlaying == false select source);

        var audioSources = availableSource as AudioSource[] ?? availableSource.ToArray();
        if (audioSources.Any())
        {
          audioSources.First().clip = clip;
          audioSources.First().Play();
        }
        else
        {
          // 없을 경우 생성하여 붙이기
          var obj = new GameObject("EffectSource");
          obj.transform.SetParent(Instance.transform);
          obj.transform.localPosition = Vector3.zero;
          var source = obj.AddComponent<AudioSource>();

          source.outputAudioMixerGroup = ObjectMixer;
          source.clip = clip;
          source.Play();
          
          Instance.effectSources.Add(source);
        }
      }
    }
    
    /// <summary>
    /// 문자열 키를 기반으로 사운드를 재생합니다.
    /// </summary>
    /// <param name="clipName">재생할 클립의 파일명</param>
    /// <param name="type">재생할 사운드의 종류</param>
    public static void Play(string clipName, AudioType type = AudioType.Effect)
    {
      if(!HasInstance) return;
      if(TryGetClip(clipName, out var clip, true))
      {
        Play(clip, type);
        return;
      }

#if UNITY_EDITOR
      Debug.LogWarning($"AudioClip {clipName} is not found.");
#endif
    }
    
    /// <summary>
    /// 어드레서블 경로를 기반으로 사운드를 재생합니다.
    /// </summary>
    /// <param name="reference">재생할 클립의 경로입니다.</param>
    /// <param name="type">재생할 사운드의 종류</param>
    public static void Play(ReferenceWrapper reference, AudioType type = AudioType.Effect)
    {
      if(!HasInstance) return;
      if(TryGetClip(reference, out var clip, true))
      {
        Play(clip, type);
        return;
      }
      
      #if UNITY_EDITOR
      Debug.LogWarning($"AudioClip {reference.path} is not found.");
      #endif
    }

    /// <summary>
    /// type의 사운드를 중지시킵니다.
    /// </summary>
    /// <param name="type">중지할 사운드의 타입입니다.</param>
    public static void Stop(AudioType type = AudioType.Background)
    {
      if(!HasInstance) return;

      if (type == AudioType.Background)
        Instance.backgroundSource.Stop();
      else
      {
        foreach (var source in Instance.effectSources)
        {
          source.Stop();
        }
      }
      
    }
    
    #endregion
  }

  public enum AudioType
  {
    Background,
    Effect
  }
}
