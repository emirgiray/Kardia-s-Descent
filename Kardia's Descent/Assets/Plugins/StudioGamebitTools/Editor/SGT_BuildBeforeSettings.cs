

#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.Presets;

class SGT_BuildBeforeSettings : IPreprocessBuildWithReport
{
    //Reset playersetting and some scriptableobjects before build.

    public int callbackOrder { get { return 0; } }


    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Run This Before Build for target " + report.summary.platform + " at path " + report.summary.outputPath);



        // //If you create GameVersion.asset, You need to create in this path.
        // SGT_GameVersion Version = (SGT_GameVersion)AssetDatabase.LoadAssetAtPath("Assets/Resources/SGT_GameVersion.asset", typeof(SGT_GameVersion));
        //
        //
        //
        // //GameVersion
        // var so = new SerializedObject(Version);
        //
        // float _CurrentGameVersion = so.FindProperty("GameVersion").floatValue;
        // float _NewGameVersion = _CurrentGameVersion + 0.01f;
        // so.FindProperty("GameVersion").floatValue = (float)Mathf.Round(_NewGameVersion*100.0f)*0.01f;
        //
        // //BundleVersion
        // int _CurrentBundleVersion= so.FindProperty("BundleVersion").intValue;
        // int _NewBundleVersion = _CurrentBundleVersion + 1;
        // so.FindProperty("BundleVersion").intValue = _NewBundleVersion;
        //
        // PlayerSettings.bundleVersion = _NewGameVersion.ToString();
        //
        // PlayerSettings.Android.bundleVersionCode = _NewBundleVersion;
        //
        // so.ApplyModifiedProperties();


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }



}
#endif