using Newtonsoft.Json;
using UnityEngine;

namespace PixelCollector.IO.SubModules.Players
{
  public class PlayerModule : SubModule
  {
    public override string ModuleType => nameof(PlayerModule);

    #region Data

    public int deathVersion = 0;

    #endregion

    public PlayerModule(string name) : base(name)
    {
    }
  }
}