using System;
using NaughtyAttributes;
using UnityEngine;

namespace PixelCollector.Util.UI
{
  public class WorldGaugeBar : MonoBehaviour
  {
    [Label("시작 색상")] public Color startColor;
    [Label("끝 색상")] public Color endColor;
    [Label("최대값")] public float max;
    [Label("현재 값"), SerializeField, GetSet(nameof(Value))] private float value;
    
    [SerializeField] private RectTransform rect;
    [SerializeField] private SpriteRenderer sprite;

    public float Value
    {
      get => value;
      set
      {
        this.value = value < 0 ? 0 : Math.Min(value, max);
        
        var percent = this.value / max;
        
        rect.localScale = rect.localScale.X(percent);
        sprite.color = Color.Lerp(startColor, endColor, percent);
      }
    }

    public Color Color
    {
      get => sprite.color;
      set => sprite.color = value;
    }

    public void ChangeMax(float maxValue)
    {
      max = maxValue;
      Value = Value;
    }
  }
}