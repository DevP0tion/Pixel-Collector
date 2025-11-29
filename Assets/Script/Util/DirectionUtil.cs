

using UnityEngine;

namespace PixelCollector.Util
{
    public static class DirectionUtil
    {
        public static Vector2 GetHorizontalDirection(Vector2 vector)
        {
            return vector.x < 0 ? Vector2.left : Vector2.right;
        }

        public static Vector2 GetHorizontalDirection(float xValue)
        {
            return xValue < 0 ? Vector2.left : Vector2.right;
        }
    }
}