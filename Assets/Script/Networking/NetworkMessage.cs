using Mirror;

namespace PixelCollector.Networking
{
  public struct AuthResponseMessage : NetworkMessage
  {
    public byte code;
    public string message;
  }
}