using System.Collections;
using System.Collections.Generic;
using CodeMonkey.CameraSystem;
using UnityEngine;

public class EverythingUsefulAssigner : MonoBehaviour
{
    public static EverythingUsefulAssigner Instance { get; private set; }
    
    public EverythingUseful EverythingUseful;
    public GameManager GameManager;
    public LevelManager LevelManager;
    public PathfinderVariables PathfinderVariables;
    public SceneChanger SceneChanger;
    public SaveLoadSystem SaveLoadSystem;
    public MainPrefabScript MainPrefabScript;
    public Interact Interact;
    public TurnSystem TurnSystem;
    public UIManager UIManager;
    public CameraSystem CameraSystem;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        
        EverythingUseful.GameManager = GameManager;
        EverythingUseful.LevelManager = LevelManager;
        EverythingUseful.PathfinderVariables = PathfinderVariables;
        EverythingUseful.SceneChanger = SceneChanger;
        EverythingUseful.SaveLoadSystem = SaveLoadSystem;
        EverythingUseful.MainPrefabScript = MainPrefabScript;
        EverythingUseful.Interact = Interact;
        EverythingUseful.TurnSystem = TurnSystem;
        EverythingUseful.UIManager = UIManager;
        EverythingUseful.CameraSystem = CameraSystem;
    }

    public void InitAwake()
    {
        EverythingUseful.GameManager = GameManager;
        EverythingUseful.LevelManager = LevelManager;
        EverythingUseful.PathfinderVariables = PathfinderVariables;
        EverythingUseful.SceneChanger = SceneChanger;
        EverythingUseful.SaveLoadSystem = SaveLoadSystem;
        EverythingUseful.MainPrefabScript = MainPrefabScript;
        EverythingUseful.Interact = Interact;
        EverythingUseful.TurnSystem = TurnSystem;
        EverythingUseful.UIManager = UIManager;
        EverythingUseful.CameraSystem = CameraSystem;
    }
}
