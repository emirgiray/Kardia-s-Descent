using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneType", menuName = "ScriptableObjects/ScenesAndTypes/SceneType", order = 0)]
public class SceneType : ScriptableObject
{
    public SceneField Scene;
    [PreviewField(Height = 100, Alignment = ObjectFieldAlignment.Left)]
    public Sprite typeImage;
    [OnValueChanged("SetTypeSprite")]
    public TypeName typeName;
    public enum TypeName
    {
        None, Basic, Elite, Boss, Shop, Heal, Event
    }

    [SerializeField] private bool changeSpriteByType = true;
    
    public TypeSpriteChanger TypeSpriteChanger;
    public void SetTypeSprite()
    {
        if (!changeSpriteByType) return;
        typeImage = TypeSpriteChanger.GetTypeSprite(typeName);
    }
}
