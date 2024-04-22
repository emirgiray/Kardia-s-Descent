using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AllSceneTypes", menuName = "ScriptableObjects/ScenesAndTypes/AllSceneTypes", order = 0)]
public class AllSceneTypes : ScriptableObject
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    
    [Tooltip("This will change according to scenes being seen")]
    public List<SceneType> remainingSceneTypes = new();
    public List<SceneType> defaultAllSceneTypes = new();

    [Button, GUIColor(1f, 1f, 1f)]
    // this should be called once a run ends
    public void ResetToDefault()
    {
        List<SceneType> temp = new();
        foreach (var sceneType in defaultAllSceneTypes)
        {
            temp.Add(sceneType);
        }
        remainingSceneTypes = temp;
    }

    public void RemoveSceneType(SceneType sceneType)
    {
        remainingSceneTypes.Remove(sceneType);
        everythingUseful.SaveLoadSystem.remainingSceneTypes = remainingSceneTypes;
        everythingUseful.SaveLoadSystem.SaveRemainingSceneTypes();
    }
    
    // also dont forget to save and load remainingSceneTypes
}
