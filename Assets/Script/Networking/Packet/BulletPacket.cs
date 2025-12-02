using System;
using Mirror;
using PixelCollector.Bullet.Properties;
using PixelCollector.Core;
using UnityEngine;

namespace PixelCollector.Networking.Packet
{
  [Serializable]
  public struct BulletPacket : IPacket
  {
    [NaughtyAttributes.ReadOnly] public string typeName;
    [NaughtyAttributes.ReadOnly] public string teamName;
    public Vector3 startPos;
    public Vector3 targetPos;
    public float damage;

    public BulletProperties Type => BulletProperties.Bullets[typeName];
    public Team Team => Team.Get(teamName);

    public BulletPacket(string typeName, string teamName, Vector3 startPos, Vector3 targetPos, float damage)
    {
      this.typeName = typeName;
      this.teamName = teamName;
      this.startPos = startPos;
      this.targetPos = targetPos;
      this.damage = damage;
    }

    public BulletPacket(BulletProperties type, Team team, Vector3 startPos, Vector3 targetPos, float damage) : this(
      type.name, team.Name, startPos, targetPos, damage = 1) {}

    public void Write(NetworkWriter writer)
    {
      writer.WriteString(typeName);
      writer.WriteString(teamName);
      writer.WriteVector3(startPos);
      writer.WriteVector3(targetPos);
      writer.WriteFloat(damage);
    }
    
    public static BulletPacket Read(NetworkReader reader)
    {
      return new BulletPacket(
        reader.ReadString(), 
        reader.ReadString(),
        reader.ReadVector3(),
        reader.ReadVector3(),
        reader.ReadFloat()
        );
    }
  }
}