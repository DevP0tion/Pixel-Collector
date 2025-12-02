using System;
using Cysharp.Threading.Tasks;
using PixelCollector.Bullet.Properties;
using PixelCollector.Core;
using PixelCollector.Core.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCollector.Scene.Intro
{
  public class GameLoader : MonoBehaviour
  {
    public GameManager manager;

    private async void Start()
    {
      await BulletProperties.Load();
      
      if (Application.platform == RuntimePlatform.WindowsServer)
      {
        SceneManager.LoadScene(Defines.FieldScene);
      }
      else
      {
        SceneManager.LoadScene(Defines.LobbyScene);
      }
    }
  }
}
