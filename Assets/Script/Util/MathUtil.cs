using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelCollector.Util
{
  public delegate T Operator<T>(T origin);
  public static class MathUtil
  {
    #region LookAt2D

    public static void LookAt2D(this Transform transform, Transform target, Vector2 direction)
      => transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(direction.normalized, (target.position - transform.position).normalized));

    public static void LookAt2D(this Transform transform, Transform target) => LookAt2D(transform, target, Vector2.up);
    
    #endregion
    
    #region Vector3
    
    public static Vector3 X(this Vector3 origin, float x) => new (x, origin.y, origin.z);
    public static Vector3 Y(this Vector3 origin, float y) => new (origin.x, y, origin.z);
    public static Vector3 Z(this Vector3 origin, float z) => new (origin.x, origin.y, z);
    
    public static Vector3 X(this Vector3 origin, Operator<float> x) => new (x(origin.x), origin.y, origin.z);
    public static Vector3 Y(this Vector3 origin, Operator<float> y) => new (origin.x, y(origin.y), origin.z);
    public static Vector3 Z(this Vector3 origin, Operator<float> z) => new (origin.x, origin.y, z(origin.z));
    
    public static Vector3 TransformPoint(this Vector3 origin, Transform transform) => transform.TransformPoint(origin);
    
    #endregion
    
    #region Vector2
    
    public static Vector2 X(this Vector2 origin, float x) => new (x, origin.y);
    public static Vector2 Y(this Vector2 origin, float y) => new (origin.x, y);
    
    public static Vector2 X(this Vector2 origin, Operator<float> x) => new (x(origin.x), origin.y);
    public static Vector2 Y(this Vector2 origin, Operator<float> y) => new (origin.x, y(origin.y));
    
    public static float ToAngle(this Vector2 direction)
    {
      var angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      
      return (angle < 0 ? angle + 360 : angle) - 90;
    }
    
    public static Vector2 TransformPoint(this Vector2 origin, Transform transform) => transform.TransformPoint(origin);

    public static Quaternion GetDirection(this Vector2 origin, Vector2 target)
    {
      var dir = target - origin;
      var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      return Quaternion.Euler(0, 0, angle);
    }

    public static Quaternion ToQuaternion(this Vector2 origin) => Quaternion.Euler(0, 0, Mathf.Atan2(origin.y, origin.x) * Mathf.Rad2Deg);
    
    #endregion
    
    #region Float

    public static Vector2 ToDirection(this float angle)
    {
      var rad = (angle + 90f) * Mathf.Deg2Rad;
      return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    /// <summary>
    /// 두 각도의 모듈러 차이를 구합니다.
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static float GetAngleDiff(float angle1, float angle2)
    {
      float diff = Mathf.Abs(angle1 - angle2) % 360f;
      if (diff > 180f)
        diff = 360f - diff;
      return diff;
    }
    
    #endregion
    
    #region Quaternion
    
    public static Quaternion X(this Quaternion origin, float x) => Quaternion.Euler(x, origin.y, origin.z);
    public static Quaternion Y(this Quaternion origin, float y) => Quaternion.Euler(origin.x, y, origin.z);
    public static Quaternion Z(this Quaternion origin, float z) => Quaternion.Euler(origin.x, origin.y, z);
    
    public static Quaternion AddX(this Quaternion origin, float value) => Quaternion.Euler(origin.x + value, origin.y, origin.z);
    public static Quaternion AddY(this Quaternion origin, float value) => Quaternion.Euler(origin.x, origin.y + value, origin.z);
    public static Quaternion AddZ(this Quaternion origin, float value) => Quaternion.Euler(origin.x, origin.y, origin.z + value);
    
    public static Quaternion X(this Quaternion origin, Operator<float> x) => Quaternion.Euler(x(origin.x), origin.y, origin.z);
    public static Quaternion Y(this Quaternion origin, Operator<float> y) => Quaternion.Euler(origin.x, y(origin.y), origin.z);
    public static Quaternion Z(this Quaternion origin, Operator<float> z) => Quaternion.Euler(origin.x, origin.y, z(origin.z));

    public static Vector2 ToVector2Direction(this Quaternion origin)
    {
      var angle = origin.eulerAngles.z * Mathf.Deg2Rad;
      return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    
    #endregion
    
    #region Array
    
    /// <summary>
    /// 배열 내부의 랜덤한 값를 추출합니다.
    /// </summary>
    /// <param name="array">값을 추출할 배열입니다.</param>
    /// <returns>배열 내부의 무작위로 추출된 값입니다.</returns>
    public static T GetRandom<T>(this T[] array)
    {
      return array[Random.Range(0, array.Length)];
    }
    
    #endregion

    #region Enum

    /// <summary>
    ///   열거형의 목록을 가져와서 기존 열거형 값이랑 비교 후 다음 열거형을 반환
    /// </summary>
    /// <param name="value">원본 열거형의 값</param>
    /// <param name="reverse">역방향으로 가져올 것인지</param>
    /// <typeparam name="T">값을 받아올 열거형의 종류</typeparam>
    /// <returns></returns>
    public static T Next<T>(this T value, bool reverse = false) where T : struct, Enum
    {
      var enums = (T[])Enum.GetValues(typeof(T));
      var index = Array.IndexOf(enums, value);
      index = reverse ? index - 1 : index + 1;

      return enums[index < 0 ? enums.Length - 1 : index > enums.Length - 1 ? 0 : index];
    }

    #endregion
    
    #region Layer
    
    public static bool Contains(this LayerMask mask, int layer) => (mask.value & (1 << layer)) != 0;
    public static bool Contains(this LayerMask mask, GameObject gameObject) => gameObject && Contains(mask, gameObject.layer);
    public static bool Contains(this LayerMask mask, Component component) => component && Contains(mask, component.gameObject);
    
    #endregion
  }
}