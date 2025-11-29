using System;
using UnityEngine;

namespace PixelCollector.Util.Extensions
{
  public static class GameObjectExtension
  {
    public static void RunAllObject(this GameObject obj, Action<GameObject> action)
    {
      if (obj == null || action == null) return;
      
      action(obj);

      foreach (Transform child in obj.transform)
      {
        child.gameObject.RunAllObject(action);
      }
    }
  }
}