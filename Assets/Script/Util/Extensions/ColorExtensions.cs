using UnityEngine;

namespace PixelCollector.Util.Extensions
{ 
  public static class ColorExtensions
  {
     public static Color WithAlpha(this Color color, float alpha)
     {
        color.a = alpha;
        return color;
     }

     public static Color32 WithAlpha(this Color32 color, byte alpha)
     {
       color.a = alpha;
       return color;
     }
  }
}
