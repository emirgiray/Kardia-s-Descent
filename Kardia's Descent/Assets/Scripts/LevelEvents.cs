using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LevelEvents : MonoBehaviour
{
    [FoldoutGroup("Events")]
    public UnityEvent AwakeEvent;
    [FoldoutGroup("Events")]
    public UnityEvent StartEvent;
    
    private void Awake()
    {
        AwakeEvent.Invoke();
        
        if(SaveLoadSystem.Instance.loadOnAwake && !SaveLoadSystem.Instance.loadedForFirstTime) SaveLoadSystem.Instance.LoadGame();
        MainPrefabScript.Instance.Awake();
    }
    
    private void Start()
    {
        StartEvent.Invoke();
    }

    
}
