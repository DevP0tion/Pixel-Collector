using System;
using System.Net;
using System.Threading.Tasks;
using PixelCollector.Networking;
using PixelCollector.Networking.Client;
using SocketIOClient;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelCollector
{
  public class ServerManagement : MonoBehaviour
  {
    [SerializeField] private NetClientManager netClientManager;
    private void Awake()
    { 
      var socket = new SocketIOUnity("http://localhost:7777", new SocketIOOptions
      {
        Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
      });
      
      socket.Connect();
      
      netClientManager.StartServer();
    }
  }
}
