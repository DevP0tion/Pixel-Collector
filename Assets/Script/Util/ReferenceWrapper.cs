using System;
using UnityEngine.AddressableAssets;

namespace PixelCollector.Util
{
  [Serializable]
  public class ReferenceWrapper
  {
    /// <summary>
    /// 어드레서블 참조입니다.
    /// </summary>
    public AssetReference assetReference;
    
    /// <summary>
    /// 읽기전용 주소입니다.
    /// </summary>
    public string path;

    public ReferenceWrapper() { }

    public ReferenceWrapper(string path)
    {
      assetReference = new AssetReference(path);
      this.path = path;
    }

    public ReferenceWrapper(AssetReference assetReference, string path)
    {
      this.assetReference = assetReference;
      this.path = path;
    }

    public override string ToString() => path;
    
    public static implicit operator AssetReference(ReferenceWrapper wrapper) => wrapper.assetReference;
    public static implicit operator ReferenceWrapper(string path) => new  (path);
  }
}