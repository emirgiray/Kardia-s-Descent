using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SerializableTypeSprite
{
    public SceneType.TypeName typeName;
    [PreviewField(Height = 50, Alignment = ObjectFieldAlignment.Left)]
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "TypeSpriteChanger", menuName = "ScriptableObjects/ScenesAndTypes/TypeSpriteChanger", order = 0)]
public class TypeSpriteChanger : ScriptableObject
{
    [SerializeField]
    private List<SerializableTypeSprite> typeSpritesList;

    private static Dictionary<SceneType.TypeName, Sprite> typeSprites;

    public void Init()
    {
        typeSprites = new Dictionary<SceneType.TypeName, Sprite>();

        foreach (var typeSprite in typeSpritesList)
        {
            typeSprites[typeSprite.typeName] = typeSprite.sprite;
        }
    }

    public Sprite GetTypeSprite(SceneType.TypeName typeName)
    {
        Init();
        if (typeSprites.TryGetValue(typeName, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for type name: {typeName}");
            return null;
        }
    }
    
    private void OnValidate()
    {
        for (int i = 0; i < typeSpritesList.Count; i++)
        {
            if (i > 0 && typeSpritesList[i].typeName == SceneType.TypeName.None)
            {
                SerializableTypeSprite typeSprite = typeSpritesList[i];
                typeSprite.typeName = GetNextTypeName(typeSpritesList[i - 1].typeName);
                typeSpritesList[i] = typeSprite;
            }
        }
    }
    private SceneType.TypeName GetNextTypeName(SceneType.TypeName current)
    {
        int nextValue = ((int)current + 1) % Enum.GetValues(typeof(SceneType.TypeName)).Length;
        return (SceneType.TypeName)nextValue;
    }
}