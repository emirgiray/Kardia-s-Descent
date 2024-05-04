using System.Collections;
using System.Collections.Generic;
using CodeMonkey.CameraSystem;
using SGT_Tools.Audio;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "EverythingUseful", menuName = "ScriptableObjects/EverythingUseful", order = 0)]
public class EverythingUseful : ScriptableObject
{
    [FoldoutGroup("Assigners")]
    public GameManager GameManager;
    [FoldoutGroup("Assigners")]
    public LevelManager LevelManager;
    [FoldoutGroup("Assigners")]
    public PathfinderVariables PathfinderVariables;
    [FoldoutGroup("Assigners")]
    public SceneChanger SceneChanger;
    [FoldoutGroup("Assigners")]
    public SaveLoadSystem SaveLoadSystem;
    [FoldoutGroup("Assigners")]
    public MainPrefabScript MainPrefabScript;
    [FoldoutGroup("Assigners")]
    public Interact Interact;
    [FoldoutGroup("Assigners")]
    public TurnSystem TurnSystem;
    [FoldoutGroup("Assigners")]
    public UIManager UIManager;
    [FoldoutGroup("Assigners")]
    public CameraSystem CameraSystem;
    [FoldoutGroup("Assigners")]
    public AllPlayers AllPlayers;
    [FoldoutGroup("Assigners")]
    public SGT_AudioSFXManager AudioManager;
    public GameObject SpawnTextGO;
    

    [Tooltip("Used for damage, heal, miss")]
    public void SpawnText(string value, Color color, Transform RandomSpawnLocation, float yOffset, float animDelay, SGT_Health health)
    {
        Vector3 PosAroundHead = SGT_Math.GetPositionAroundObject(RandomSpawnLocation, 0.5f);
        Vector3 randomPosAroundHead = new Vector3(PosAroundHead.x, PosAroundHead.y + yOffset, PosAroundHead.z);
        
        GameObject spawnedHitText = Instantiate(SpawnTextGO, randomPosAroundHead, Quaternion.identity);
        var spawnText = spawnedHitText.GetComponent<SpawnText>();
        if (value != "MISS" && int.Parse(value) >= health.Max)
        {
            spawnText.text.text = health.Max.ToString();
        }
        else
        {
            spawnText.text.text = value;
        }
        spawnText.text.color = color;
        
        //spawnText.SetDestroyTime(destroyTime);
        spawnText.SetAnimDelay(animDelay);
    }

    [Tooltip("Used for stat increases, decreases, buffs, debuffs, etc.")]
    public void SpawnText(string value, Color color, Transform staticSpawnLocation, float yOffset, float animDelay, bool isBuff)
    {
        /*Vector3 PosAroundHead = SGT_Math.GetPositionAroundObject(RandomSpawnLocation, 0.5f);
        Vector3 randomPosAroundHead = new Vector3(PosAroundHead.x, PosAroundHead.y + yOffset, PosAroundHead.z);
        
        GameObject spawnedHitText = Instantiate(SpawnTextGO, randomPosAroundHead, Quaternion.identity);*/
        
        Vector3 upperPosAroundHead = new Vector3(staticSpawnLocation.position.x, staticSpawnLocation.position.y + yOffset, staticSpawnLocation.position.z);
        GameObject spawnedHitText = Instantiate(SpawnTextGO, upperPosAroundHead, Quaternion.identity);
        var spawnText = spawnedHitText.GetComponent<SpawnText>();
        spawnText.text.text = value;
        spawnText.text.color = color;
        
        if (isBuff)
        {
            spawnText.buffIcon.SetActive(true);
        }
        else
        {
            spawnText.debuffIcon.SetActive(true);
        }    
        
        //spawnText.SetDestroyTime(destroyTime);
        spawnText.SetAnimDelay(animDelay);
    }


    public void SwitchGameUI()
    {
        UIManager.SwitchGameUI();
    }

    public void AddRevelaer(GameObject gameObject)
    {
        MainPrefabScript.fogWar.AddFogRevealerRevelear(gameObject.transform);
    }
    
}
