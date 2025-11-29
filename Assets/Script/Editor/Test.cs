using System.Threading.Tasks;
using NUnit.Framework;
using PixelCollector.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCollector.Editor
{
  public class Test
  {
    [Test]
    public async Task SAVETest()
    {
      var save = (await SAVE.GetAllSaves())[0];

      // save.LoadAll().Wait();
      
      save.name = "Test";
      save.Save();
    }

    [Test]
    public async Task SAVELoadTest()
    {
      var save = await SAVE.Load("Test");
    }

    [Test]
    public void LoadAllSAVETest()
    {
      var saves = SAVE.GetAllSaves().Result;

      foreach (var save in saves)
      {
        Debug.Log(save.MetaData);
      }
    }
  }
}