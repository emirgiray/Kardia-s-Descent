using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
#endif

//todo bg colors
[CreateAssetMenu(fileName = "SceneType", menuName = "ScriptableObjects/ScenesAndTypes/SceneType", order = 0)]
public class SceneType : ScriptableObject
{
    [OnValueChanged("CheckName")]
    public SceneField Scene;
    [PreviewField(Height = 100, Alignment = ObjectFieldAlignment.Left)]
    public Sprite typeImage;
    public Color bgColor = Color.white;
    [OnValueChanged("Set")]
    public TypeName typeName;
    public enum TypeName
    {
        None, Basic, Elite, Boss, Shop, Heal, Event, Companion
    }

    [SerializeField] private bool changeSpriteByType = true;
    
    public TypeSpriteChanger TypeSpriteChanger;

    [Button, GUIColor(1f, 1f, 1f)]
    public void Set()
    {
        if (!changeSpriteByType) return;
        typeImage = TypeSpriteChanger.GetTypeSprite(typeName);
        bgColor = TypeSpriteChanger.GetTypeColor(typeName);
    }

    /*public void CheckName()
    {
        if (name != Scene.SceneName)
        {
            //Debug.LogError("SceneType name and SceneField name does not match !!!");
            name = Scene.SceneName;
        }
    }*/
    
    
#if UNITY_EDITOR
    public void CheckName()
    {
        if (name != Scene.SceneName && Scene != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(assetPath, Scene.SceneName);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
