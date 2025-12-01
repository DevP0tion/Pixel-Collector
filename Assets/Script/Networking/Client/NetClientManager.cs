using System;
using kcp2k;
using Mirror;
using UnityEngine;

namespace PixelCollector.Networking.Client
{
  public class NetClientManager : NetworkManager
  {
    public override void Start()
    {
      base.Start();

      StartClient(new Uri("kcp://localhost:10224"));
    }
  }
}