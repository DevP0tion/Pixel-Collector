using UnityEngine;

namespace PixelCollector.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class QuickFolderAccess : EditorWindow
    {
        [MenuItem("Tools/Quick Folder Access")]
        public static void ShowWindow()
        {
            GetWindow<QuickFolderAccess>("Folder Shortcut");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Entity Prefabs"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Prefabs/Entities");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }

            if (GUILayout.Button("Enemy Scripts"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/Entities/Enemy");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            if (GUILayout.Button("Enemy Data"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Data/Enemy");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            if (GUILayout.Button("Scene Folder"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scenes");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
        }
    }
}
