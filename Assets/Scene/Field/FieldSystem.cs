using PixelCollector.Networking.Client;
using PixelCollector.Util.Singletons;
using Unity.Cinemachine;
using UnityEngine;

namespace PixelCollector.Scene.Field
{
  public class FieldSystem : Singleton<FieldSystem>
  {
    [SerializeField] private CinemachineCamera vCamera;

    protected override void Awake()
    {
      base.Awake();

      var player = NetClientManager.LocalPlayer;
      if (player != null)
        vCamera.Target = new CameraTarget
        {
          TrackingTarget = NetClientManager.LocalPlayer.transform
        };
      
      NetClientManager.Instance.OnPlayerChanged += p => vCamera.Target = new CameraTarget {TrackingTarget = p.transform};
    }
  }
}