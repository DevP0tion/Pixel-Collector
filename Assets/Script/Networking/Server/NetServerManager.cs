using System.Collections.Generic;
using Mirror;
using PixelCollector.Networking.Client;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  public class NetServerManager : NetworkManager
  {
    public Dictionary<int, NetworkPlayer> players = new(); 
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
      base.OnServerConnect(conn);
      
      Debug.Log("Player connected: " + conn.connectionId);
      
      var player = new NetworkPlayer
      {
        conn = conn,
        username = $"Player{conn.connectionId}",
        connectedAt = System.DateTime.Now
      };
      players[conn.connectionId] = player;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
      base.OnServerDisconnect(conn);
      
      Debug.Log("Player disconnected: " + conn.connectionId);
      
      players.Remove(conn.connectionId);
    }

    public static NetworkPlayer GetPlayer(NetworkConnectionToClient conn)
    {
      var server = singleton as NetServerManager;
      if (server != null && server.players.TryGetValue(conn.connectionId, out var player))
      {
        return player;
      }
      return null;
    }
  }
}