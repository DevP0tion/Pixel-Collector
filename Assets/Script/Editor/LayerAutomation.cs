using UnityEditor;
using UnityEngine;

namespace PixelCollector.Editor
{
    public static class LayerAutomation
    {
        private const string SemiPlatformTagChangerPath = "Assets/Prefabs/SemiPlatform Tag Change";

        [MenuItem(SemiPlatformTagChangerPath, true)]
        private static bool ValidateTagSemiPlatforms()
        {
            // 선택된 오브젝트 중 하나라도 GameObject(프리팹)일 경우 메뉴 활성화
            foreach (var obj in Selection.objects)
            {
                if (PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab)
                    return true;
            }

            return false;
        }
        
        [MenuItem(SemiPlatformTagChangerPath)]
        private static void TagSemiPlatforms()
        {
            int modifiedCount = 0;

            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefabRoot == null) continue;

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                bool modified = false;

                foreach (Transform t in instance.GetComponentsInChildren<Transform>(true))
                {
                    if (t.TryGetComponent<PlatformEffector2D>(out var effector) &&
                        !t.gameObject.CompareTag("SemiPlatform"))
                    {
                        if (effector.useOneWay)
                        {
                            t.gameObject.tag = "SemiPlatform";
                            modified = true;
                        }
                    }
                }

                if (modified)
                {
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                    modifiedCount++;
                }

                Object.DestroyImmediate(instance);
            }

            Debug.Log($"✅ 선택한 프리팹 중 {modifiedCount}개에 SemiPlatform 태그를 적용했습니다.");
        }
    }
}