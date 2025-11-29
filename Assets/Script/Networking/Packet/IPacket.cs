using Mirror;

namespace PixelCollector.Networking.Packet
{
  public interface IPacket : NetworkMessage
  {
    void Write(NetworkWriter writer);
  }
}