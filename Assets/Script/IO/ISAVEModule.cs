using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PixelCollector.IO
{
  public interface ISAVEModule
  {
    public string ModuleType { get; }
    public string Name { get; set; }
    
    public JObject BeforeSave();
    public void Read(JObject data);
    public void Read(IJsonSerializable data);
    public void Save(string parentPath);
    public Task Load(string path, bool chainLoading);
    public SAVEModule Node(string key, bool force = false);
    public T Node<T>(string key, bool force = false) where T : ISAVEModule;
  }
}