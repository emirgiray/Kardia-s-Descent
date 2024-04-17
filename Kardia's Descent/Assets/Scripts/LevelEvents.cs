using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LevelEvents : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
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
    }

    
}
