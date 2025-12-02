using System;
using kcp2k;
using Mirror;
using PixelCollector.Unit.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace PixelCollector.Networking.Client
{
  public class NetClientManager : NetworkManager
  {
    public static NetClientManager Instance { get; private set; }
    
    private PlayerBaseModule localPlayer;
    public static PlayerBaseModule LocalPlayer
    {
      get => Instance.localPlayer;
      set
      {
        Instance.localPlayer = value;
        Instance.OnPlayerChanged?.Invoke(value);
      }
    }
    
    #region Event
    
    public event Action<PlayerBaseModule> OnPlayerChanged;
    
    #endregion
    
    #region Unity Callback
    
    public override void Awake()
    {
      base.Awake();

      if (Instance)
        Destroy(gameObject);
      else
      {
        DontDestroyOnLoad(gameObject);
        Instance = this;
      }
    }
    
    #endregion

    public void JoinServer()
    {
      NetworkClient.Connect("localhost");
    }
  }
}