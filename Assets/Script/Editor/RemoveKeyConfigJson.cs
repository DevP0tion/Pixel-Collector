using System.IO;
using UnityEditor;
using UnityEngine;

namespace PixelCollector.Editor
{
    public class RemoveKeyConfigJson : EditorWindow
    {
        const string KeyBindsFileName = "KeyBinds.json";
        private static string KeyBindsFilePath =>
            Path.Combine(Application.persistentDataPath, KeyBindsFileName);

        [MenuItem("Tools/Remove Key Config Json")]
        public static void DeleteKeyConfigFile()
        {
            if (File.Exists(KeyBindsFilePath))
            {
                File.Delete(KeyBindsFilePath);
                Debug.Log("키셋팅 파일을 삭제했습니다.");
            }
            else
            {
                Debug.Log("키셋팅 파일이 없습니다.");
                
            }
        }
    }
}