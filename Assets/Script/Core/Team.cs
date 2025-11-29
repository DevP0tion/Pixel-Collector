using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace PixelCollector.Core
{
  [Serializable]
  public struct Team : IEquatable<Team>
  {
    public static readonly List<Team> ActiveTeams = new();
    private static int nextId;

    #region Fields

    [SerializeField] private Color color;
    [SerializeField, ReadOnly] private string name;
    [SerializeField, ReadOnly] private int id;
    
    public Color Color => color;
    public string Name => name;
    public int Id => id;

    #endregion
    
    private Team(string name, Color color)
    {
      this.name = name;

      var index = ActiveTeams.FindIndex(team => team.name == name);

      if (index == -1)
      {
        id = nextId++;
        this.color = color;
        ActiveTeams.Add(this);
      }
      else
      {
        id = index;
        this.color = ActiveTeams[index].color;
      }
    }

    public static Team Get(string name)
    {
      var team = None;

      foreach (var activeTeam in ActiveTeams.Where(activeTeam => activeTeam.name == name))
      {
        team = activeTeam;
        break;
      }
      
      return team;
    }
    
    #region Operators

    public static bool operator ==(Team left, Team right) => left.id == right.id;
    public static bool operator !=(Team left, Team right) => !(left == right);
    public bool Equals(Team other) => id == other.id;
    public override bool Equals(object obj) => obj is Team other && Equals(other);
    public override int GetHashCode() => id;
    
    public static implicit operator Team(string name) => Get(name);
    
    #endregion

    #region Preload

    public static readonly Team None, Player, Enemy;

    static Team()
    {
      None = new Team("None", Color.white);
      Player = new Team("Player", Color.blue);
      Enemy = new Team("Enemy", Color.red);
    }

    #endregion
  }
}