using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PixelCollector.Util
{
  public class DebugSymbol
  {
    /// <summary>
    /// 미리 정의해놓는 디버그 심볼 목록입니다.
    /// </summary>
    #region Definition

    public static readonly HashSet<DebugSymbol> Symbols = new();
    public static DebugSymbol Editor { get; private set; } = new("EDITOR") { onRelease = () => Editor = null };
    public static DebugSymbol UI { get; private set; } = new("UI") { onRelease = () => UI = null };
    public static DebugSymbol Save { get; private set; } = new ("SAVE") { onRelease = () => Save = null };
    public static DebugSymbol ETC { get; private set; } = new ("ETC") { onRelease = () => ETC = null };
    
    #endregion

    /// <summary>
    /// 심볼의 명칭입니다.
    /// </summary>
    public readonly string name;
    
    /// <summary>
    /// 심볼의 파괴가능 여부입니다.
    /// </summary>
    public readonly bool dontDestroy;
    
    /// <summary>
    /// 심볼이 현재 활성화되어있는지 여부입니다.
    /// </summary>
    public bool isActive = false;
    
    /// <summary>
    /// 심볼이 메모리에서 해제됬을 떄 호출되는 이벤트입니다.
    /// </summary>
    public event Action onRelease;
    
    /// <summary>
    /// 심볼을 새로 생성합니다. Get 또는 Create 를 사용해주세요.
    /// </summary>
    private DebugSymbol(string name, bool dontDestroy = true)
    {
      this.name = name;
      this.dontDestroy = dontDestroy;
      
      Symbols.Add(this);
    }

    /// <summary>
    /// 심볼을 가져오거나 생성합니다.
    /// </summary>
    /// <param name="name">가져올 심볼의 명칭입니다.</param>
    /// <param name="force">true일 경우 디버그 심볼이 없으면 생성합니다.</param>
    /// <param name="dontDestroy">false일 경우 심볼을 메모리에서 해제가 불가능합니다.</param>
    public static DebugSymbol Get(string name, bool force = true, bool dontDestroy = false)
    {
      var result = Symbols.FirstOrDefault(x => x.name == name);

      if (result != null) return result;
      
      if (!force) return null;
      
      var newSymbol = new DebugSymbol(name, dontDestroy);
      Symbols.Add(newSymbol);
      return newSymbol;
    }

    /// <summary>
    /// 심볼들이 활성화상태인지 체크합니다.
    /// </summary>
    /// <param name="symbols">활성화 여부를 검사할 심볼들입니다.</param>
    public static bool IsActive(params string[] symbols)
    {
      var active = true;
      
      foreach (var symbol in symbols)
      {
        var target = Get(symbol, false);
        if (target == null)
        {
          active = false;
          break;
        }
        
        active &= target.isActive;
        
        if (!active) break;
      }
      
      return active;
    }

    /// <summary>
    /// 심볼을 메모리에서 해제합니다.
    /// dontDestroy가 true일 경우 불가능합니다.
    /// </summary>
    public void Release()
    {
      if(dontDestroy) return;
      
      Symbols.Remove(this);
      onRelease?.Invoke();
    }

    /// <summary>
    /// 심볼이 활성화상태일때 로그를 찍습니다.
    /// </summary>
    public void Log(object message)
    {
      if (!isActive) return;
      
      Debug.Log(message);
    }
    
    /// <summary>
    /// 심볼이 활성화상태일때 로그를 찍습니다.
    /// </summary>
    public void Log(object message, Object context)
    {
      if (!isActive) return;
      
      Debug.Log(message, context);
    }
    
    #region Operator

    public static implicit operator DebugSymbol(string symbol) => Get(symbol, true);
    public static implicit operator bool(DebugSymbol symbol) => symbol.isActive;

    #endregion
  }
}