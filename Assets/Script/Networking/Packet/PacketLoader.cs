using Mirror;
using PixelCollector.Networking.Packets;

namespace PixelCollector.Networking.Packet
{
  public static class PacketLoader
  {
    public static void WriteIPacket(this NetworkWriter writer, IPacket packet)
    {
      packet.Write(writer);
    }

    public static MovePacket ReadMovePacket(this NetworkReader reader) => MovePacket.Read(reader);
    public static AuthPacket ReadAuthPacket(this NetworkReader reader) => AuthPacket.Read(reader);
    public static BulletPacket ReadBulletPacket(this NetworkReader reader) => BulletPacket.Read(reader);
  }
}