
using UnityEngine;
using UnityEditor;
using System.IO;

public class SGT_GetPathFolder : EditorWindow
{



    [MenuItem("Assets/Get Path Folder Name")]
    static void GetFolderName()
    {

        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        Debug.Log(filePath);
        EditorGUIUtility.systemCopyBuffer = filePath;
    }
}
