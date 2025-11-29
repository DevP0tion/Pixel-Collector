using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCollector.Networking.Packet
{
  public struct MovePacket : IPacket
  {
    public Vector2 direction;
    public bool canceled;
    
    public MovePacket(InputAction.CallbackContext ctx)
    {
      direction = ctx.ReadValue<Vector2>();
      canceled = ctx.canceled;
    }

    public void Write(NetworkWriter writer)
    {
      writer.Write(direction);
      writer.Write(canceled);
    }

    public static MovePacket Read(NetworkReader reader)
    {
      return new MovePacket
      {
        direction = reader.ReadVector2(),
        canceled = reader.ReadBool()
      };
    }
  }
}