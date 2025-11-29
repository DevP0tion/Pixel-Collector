using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelCollector.IO.SubModules.Players;
using PixelCollector.Util;
using UnityEngine;

namespace PixelCollector.IO.SubModules
{
  public abstract class SubModule : ISAVEModule
  {
    protected readonly Dictionary<string, ISAVEModule> children = new();
    [JsonIgnore]
    protected string name;
    [JsonIgnore]
    public abstract string ModuleType { get; }

    #region MetaData
    
    protected readonly Dictionary<string, string> childModuleInfo = new();
    
    #endregion

    public SubModule(string name)
    {
      this.name = name;
    }

    [JsonIgnore]
    public virtual string Name
    {
      get => name;
      set => name = value;
    }

    public virtual JObject BeforeSave()
    {
      JObject result = new (), metaData = new (), childJson = new();
      
      // 자식 정보를 메타데이터로 입력
      foreach (var (_, node) in children)
        childJson[node.Name] = node.ModuleType;
      
      metaData["children"] = childJson;
      result["metaData"] = metaData;
      
      result.ReadObject(this);

      return result;
    }

    public virtual void Read(JObject data)
    {
      childModuleInfo.Clear();

      var metaData = data.Get("metaData", JsonUtil.Blank);
      foreach (var (key, value) in metaData.Get("children", JsonUtil.Blank))
      {
        if (value is null || value.Type != JTokenType.String) continue;
        
        childModuleInfo.Add(key, value.Get(nameof(SAVEModule)));
      }
      
      data.ReadJson(this);
    }
    
    public virtual void Read(IJsonSerializable data) => Read(data.ToJson());

    public virtual void Save(string parentPath)
    {
      var isRoot = name == SAVE.RootName;
      var path = Path.Combine(parentPath, !isRoot ? name : "");

      if (children.Count > 0)
      {
        Directory.CreateDirectory(path);
      
        foreach (var (_, node) in children)
        {
          node.Save(path);
        }
      }

      // 현재 데이터 모듈 저장 구현
      var data = BeforeSave();
      
      
      // 데이터 저장
      var filename = isRoot ? Path.Combine(path, name + ".json") : path + ".json";
      
      using var writer = File.CreateText(filename);
      using (var jsonWriter = new JsonTextWriter(writer))
      {
        jsonWriter.Formatting = Formatting.Indented;
        data.WriteTo(jsonWriter);

        jsonWriter.Close();
      }

      writer.Close();
    }

    public virtual async Task Load(string path, bool chainLoading)
    {
      var isRoot = name == SAVE.RootName;
      var filename = isRoot ? Path.Combine(path, name + ".json") : path + ".json";

      try
      {
        var jObject = JObject.Parse(File.ReadAllText(filename));
        Read(jObject);
      }
      catch (JsonReaderException)
      {
        throw new Exception($"[SAVE-{name}] Error while loading {filename}.");
      }

      if (chainLoading && Directory.Exists(path))
      {
        foreach (var child in Directory.GetFiles(path))
        {
          var childInfo = new FileInfo(child);
          
          if (!childInfo.Extension.Equals(".json"))
          {
            DebugSymbol.Save.Log($"[SAVE-{name}] {childInfo.Name} is not json. skipping...");
            continue;
          }
          
          var fileName = childInfo.Name.Replace(childInfo.Extension, "");

          if (fileName == name)
          {
            DebugSymbol.Save.Log($"[SAVE-{name}] {childInfo.Name} is root. skipping...");
            continue;
          }
          
          ISAVEModule childModule = childModuleInfo.GetValueOrDefault(fileName, nameof(SAVEModule)) switch
          {
            nameof(PlayerModule) => Node<PlayerModule>(fileName, true),
            _ => Node(fileName, true),
          };
          await childModule.Load(Path.Combine(path, fileName), true);
        }
      }
    }

    public virtual SAVEModule Node(string key, bool force = false)
    {
      if (children.TryGetValue(key, out var node))
      {
        if (node is SAVEModule value) return value;
        else throw new Exception("Node type is not match");
      }
      
      if (!force) throw new Exception("Node not found");
      
      var resultNode = new SAVEModule(key);
      children.Add(key, resultNode);
      return resultNode;
    }

    public virtual T Node<T>(string key, bool force = false) where T : ISAVEModule
    {
      if(children.TryGetValue(key, out var node))
      {
        if (node is not T value) throw new Exception("Node type is not match");
        
        return value;
      }
      
      if (!force) throw new Exception("Node not found");
      
      var result = typeof(T) switch
      {
        var type when type == typeof(SAVEModule) => (T) (object) new SAVEModule(key),
        var type when type == typeof(PlayerModule) => (T) (object) new PlayerModule(key),
        var type when type == typeof(AchievementModule) => (T)  (object) new AchievementModule(key),
        _ => (T) Activator.CreateInstance(typeof(T), key),
      };
      
      children.Add(key, result);
      return result;
    }
  }
}