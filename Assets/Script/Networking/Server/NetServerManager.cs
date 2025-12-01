using System.Collections.Generic;
using Mirror;
using PixelCollector.Networking.Client;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  public class NetServerManager : NetworkManager
  {
    public Dictionary<NetworkConnectionToClient, NetworkPlayer> players = new(); 
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
      base.OnServerAddPlayer(conn);
      
      Debug.Log("Player connected: " + conn.connectionId);
      
      var player = new NetworkPlayer
      {
        conn = conn,
        username = $"Player{conn.connectionId}",
        connectedAt = System.DateTime.Now
      };
      players[conn] = player;
    }
  }
}