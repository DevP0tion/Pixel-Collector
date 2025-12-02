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
    public float lifeTime;

    public BulletProperties Type => BulletProperties.Bullets[typeName];
    public Team Team => Team.Get(teamName);

    public BulletPacket(string typeName, string teamName, Vector3 startPos, Vector3 targetPos, float damage, float lifeTime)
    {
      this.typeName = typeName;
      this.teamName = teamName;
      this.startPos = startPos;
      this.targetPos = targetPos;
      this.damage = damage;
      this.lifeTime = lifeTime;
    }

    public BulletPacket(BulletProperties type, Team team, Vector3 startPos, Vector3 targetPos, float damage, float lifeTime) 
      : this(type.name, team.Name, startPos, targetPos, damage = 1, lifeTime = 5) {}

    public void Write(NetworkWriter writer)
    {
      writer.WriteString(typeName);
      writer.WriteString(teamName);
      writer.WriteVector3(startPos);
      writer.WriteVector3(targetPos);
      writer.WriteFloat(damage);
      writer.WriteFloat(lifeTime);
    }
    
    public static BulletPacket Read(NetworkReader reader)
    {
      return new BulletPacket(
        reader.ReadString(), 
        reader.ReadString(),
        reader.ReadVector3(),
        reader.ReadVector3(),
        reader.ReadFloat(),
        reader.ReadFloat()
        );
    }
  }
}