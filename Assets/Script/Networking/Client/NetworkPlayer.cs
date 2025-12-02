using Mirror;
using PixelCollector.Unit.Player;

namespace PixelCollector.Networking.Client
{
  public class NetworkPlayer
  {
    public NetworkConnectionToClient conn;
    public string username;
    public System.DateTime connectedAt;
    public PlayerBaseModule playerObject;
  }
}