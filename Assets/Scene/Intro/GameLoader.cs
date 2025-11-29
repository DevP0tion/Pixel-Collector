using System;
using PixelCollector.Core;
using PixelCollector.Core.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCollector
{
  public class GameLoader : MonoBehaviour
  {
    public GameManager manager;

    private void Awake()
    {
      SceneManager.LoadScene(Defines.LobbyScene);
    }
  }
}
