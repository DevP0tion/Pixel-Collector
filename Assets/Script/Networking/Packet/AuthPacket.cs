
using Mirror;

namespace PixelCollector.Networking.Packet
{
  public struct AuthPacket : IPacket
  {
    public string username;
    public string password;

    public void Write(NetworkWriter writer)
    {
      writer.Write(username);
      writer.Write(password);
    }

    public static AuthPacket Read(NetworkReader reader)
    {
      return new AuthPacket
      {
        username = reader.ReadString(),
        password = reader.ReadString()
      };
    }
  }
}