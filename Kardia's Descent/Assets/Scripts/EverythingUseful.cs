using System.Collections;
using System.Collections.Generic;
using CodeMonkey.CameraSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "EverythingUseful", menuName = "ScriptableObjects/EverythingUseful", order = 0)]
public class EverythingUseful : ScriptableObject
{
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
    
}
