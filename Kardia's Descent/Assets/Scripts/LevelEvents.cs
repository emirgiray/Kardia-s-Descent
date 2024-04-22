using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LevelEvents : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    [SerializeField] private List<Door> EndDoorsInScene = new();
    [FoldoutGroup("Events")]
    public UnityEvent AwakeEvent;
    [FoldoutGroup("Events")]
    public UnityEvent StartEvent;
    
    private void Awake()
    {
        AwakeEvent.Invoke();
        
        /*if(everythingUseful.SaveLoadSystem.loadOnAwake)*/ if(!everythingUseful.SceneChanger.isOnMainMenu) everythingUseful.SaveLoadSystem.InitAwake();
        MainPrefabScript.Instance.InitAwake();
        EverythingUsefulAssigner.Instance.InitAwake();

       
        
    }
    
    private void Start()
    {
        StartEvent.Invoke();
        
        FindEndDoors();
    }
    
    private void FindEndDoors()
    {
        var allDoors = FindObjectsOfType<Door>();

        foreach (var door in allDoors)
        {
            if (door.isEndDoor)
            {
                EndDoorsInScene.Add(door);
            }
        }
        
        everythingUseful.SceneChanger.EndDoorsInScene = EndDoorsInScene;
        everythingUseful.SceneChanger.DecideRandomScenes();
    }
    
}
