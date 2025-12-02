using System;
using kcp2k;
using Mirror;
using UnityEngine;

namespace PixelCollector.Networking.Client
{
  public class NetClientManager : NetworkManager
  {
    private static NetClientManager instance;
    
    #region Unity Callback
    
    public override void Awake()
    {
      base.Awake();

      if (instance)
        Destroy(gameObject);
      else
      {
        DontDestroyOnLoad(gameObject);
        instance = this;
      }
    }
    
    #endregion

    public void JoinServer()
    {
      NetworkClient.Connect("localhost");
    }
  }
}