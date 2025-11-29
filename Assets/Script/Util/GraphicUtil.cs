using System.Collections;
using UnityEngine;

namespace PixelCollector.Util
{
    public static class GraphicUtil
    {
        public static void DamageColorOverlay(MonoBehaviour owner, SpriteRenderer sprite)
        {
            owner.StartCoroutine(DamageColorOverlayCoroutine(sprite));
        }
        static IEnumerator DamageColorOverlayCoroutine(SpriteRenderer sprite)
        {
            sprite.material.SetFloat("_Alpha", 1);
            float duration = 0.3f;
            float remainedTime = duration;

            while (remainedTime > 0)
            {
                yield return null;
                remainedTime -= Time.deltaTime;
                if (remainedTime < 0) remainedTime = 0;
                sprite.material.SetFloat("_Alpha", remainedTime / duration);
            }
        }
        
        public static Vector3 ConvertGamePosToUIPos(Vector3 worldPos, Camera camera)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
            screenPos.z = 0;
            return screenPos;
        }
    }
}