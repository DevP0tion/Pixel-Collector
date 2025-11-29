using System.Collections.Generic;
using Newtonsoft.Json;

namespace PixelCollector.IO.SubModules
{
  public class AchievementModule : SubModule
  {
    public override string ModuleType => nameof(AchievementModule);

    #region Data

    [JsonProperty]
    public bool GotFirstManaCrystal = false;
    
    [JsonProperty]
    public bool KillRat = false;
    
    [JsonProperty]
    public bool KillGuardian = false;
    
    [JsonProperty]
    public bool KillSentinel = false;

    [JsonProperty] 
    private HashSet<string> completedCutScene = new();

    #endregion

    public AchievementModule(string name) : base(name)
    {
    }

    public bool IsActive(string achievementName) => achievementName switch
    {
      nameof(KillRat) => KillRat,
      nameof(KillGuardian) => KillGuardian,
      nameof(KillSentinel) => KillSentinel,
      _ => false
    };
  }
}