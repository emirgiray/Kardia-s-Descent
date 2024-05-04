using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using FischlWorks_FogWar;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//remember to make this script execute first but after turn system
public class MainPrefabScript : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    public static MainPrefabScript Instance;
    public Camera MainCamera;
    public SaveLoadSystem SaveLoadSystem;
    public GameManager GameManager;
    public LevelManager LevelManager;
    public SceneChanger SceneChanger;
    public Interact Interact;
    public TurnSystem TurnSystem;
    public List<GameObject> SelectedPlayers = new();
    public List<GameObject> InventoryUISlots = new();
    public GameObject playerPreviewParent;
    
    public GameObject PartyRoundCardsParent;
    public Transform PartyRoundCardsSlot;
    public GameObject PartyRoundCardPrefab;
    
    private List<Transform> PlayerSlots = new();
    public List<GameObject> spawnedPlayers = new();
    [HideInInspector]
    public List<Player> spawnedPlayerScripts = new();
    [HideInInspector]
    public List<GameObject> previews = new();
    [HideInInspector]
    public List<GameObject> partyRoundCards = new();
    private List<GameObject> inventoryUIs = new();
    public csFogWar fogWar;
    public void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        

        InitAwake();
        
    }

    public void InitAwake()
    {
        if (SceneChanger.isOnMainMenu) //if in main menu
        {
            MainCamera.enabled = false;
            SaveLoadSystem.loadOnAwake = false;
            
        }
        else
        {
            MainCamera.enabled = true;
            SaveLoadSystem.loadOnAwake = true;
            
        }

        /*if (DOTween.Init() == null)
        {
            Debug.Log($"Null dotween");
            DOTween.Init();
        }
        else
        {
            Debug.Log($"dotween");
        }*/

    }
    public void InitializeLevel()
    {
        fogWar = FindObjectOfType<csFogWar>();
        PlayerSlots = GameObject.Find("Player Slots").GetComponentsInChildren<Transform>().ToList();
        List<Transform> slotsToRemove = new();
        foreach (var slot in PlayerSlots)
        {
            if (!slot.name.Contains("Player Slot"))
            {
                slotsToRemove.Add(slot);
            }
        }
        foreach (var remove in slotsToRemove)
        {
            PlayerSlots.Remove(remove);
        }
        
        PlayerSlots.RemoveAt(0); //remove the parent transform
        playerPreviewParent = GameObject.Find("Player Preview");

        for (int i = 0; i < SelectedPlayers.Count; i++)
        {
            GameObject player = Instantiate(SelectedPlayers[i], PlayerSlots[i].position, PlayerSlots[i].rotation);
            player.name = player.name.Replace("(Clone)", "");
            spawnedPlayers.Add(player);

            Player playerScript = player.GetComponent<Player>();
            playerScript.inventory.InventoryUISlot = InventoryUISlots[i];
            spawnedPlayerScripts.Add(playerScript);

            fogWar.AddFogRevealerRevelear(player.transform);

            GameObject preview = Instantiate(playerScript.PlayerPreview, playerPreviewParent.transform);
            preview.transform.localPosition = new Vector3(i * 10, 0, 0);
            previews.Add(preview);

            GameObject partyRoundCard = Instantiate(PartyRoundCardPrefab, PartyRoundCardsSlot);
            partyRoundCard.GetComponent<CharacterRoundCard>().Init(playerScript, TurnSystem.RoundInfo.GetComponent<RoundInfo>(), true);
            partyRoundCards.Add(partyRoundCard);
        }
    
        PartyRoundCardsParent.SetActive(true);
        everythingUseful.CameraSystem.OnCharacterSelected(spawnedPlayerScripts[0].characterTile, 0.001f);
    }

    public void ClearPlayers()
    {
        foreach (var player in spawnedPlayerScripts)
        {
            player.characterTile.ResetOcupying();
            player.characterTile.Occupied = false;
            player.characterTile.occupiedByPlayer = false;
            
        }
        spawnedPlayerScripts.Clear();
        SelectedPlayers.Clear();
        foreach (var player in spawnedPlayers)
        {
            Destroy(player);
        }
        spawnedPlayers.Clear();
        
        foreach (var preview in previews)
        {
            Destroy(preview);
        }
        previews.Clear();
        
        foreach (var card in partyRoundCards)
        {
            Destroy(card);
        }
        partyRoundCards.Clear();
        
        foreach (var player in everythingUseful.LevelManager.allEntities)
        {
            if (player != null) everythingUseful.TurnSystem.RemoveCard(player);
        }
        everythingUseful.LevelManager.allEntities.Clear();
        
        fogWar.RemoveAllFogRevelearRevelear();
        
    }

    public void ClearPrevious()
    {
        foreach (var player in spawnedPlayerScripts)
        {
            player.inCombat = false;
            player.canAttack = true;
            player.ResetActionPoints();
            player.SkillContainer.ForceResetSkillCooldowns();
            player.characterState = Character.CharacterState.Idle;
        }

        foreach (var var in everythingUseful.TurnSystem.enemiesInCombat)
        {
            Destroy(var);
        }
        everythingUseful.TurnSystem.enemiesInCombat.Clear();
        foreach (var var in everythingUseful.TurnSystem.playersInCombat)
        {
            Destroy(var);
        }
        everythingUseful.TurnSystem.playersInCombat.Clear();
        foreach (var var in everythingUseful.TurnSystem.allEntitiesInCombat)
        {
            Destroy(var);
        }
        everythingUseful.TurnSystem.allEntitiesInCombat.Clear();
        
        everythingUseful.TurnSystem.turnState = TurnSystem.TurnState.FreeRoamTurn;
        
        foreach (var entity in everythingUseful.LevelManager.allEntities)
        {
            if (entity != null) everythingUseful.TurnSystem.RemoveCard(entity);
        }
        everythingUseful.UIManager.turnAndRoundGO.SetActive(false);
        
        foreach (var playerS in spawnedPlayerScripts)
        {
            Destroy(playerS.inventory.SpawnedInventoryUI);
        }
        spawnedPlayerScripts.Clear();
        
        foreach (var player in spawnedPlayers)
        {
            Destroy(player);
        }
        spawnedPlayers.Clear();
        
        foreach (var preview in previews)
        {
            Destroy(preview);
        }
        previews.Clear();
        
        foreach (var card in partyRoundCards)
        {
            Destroy(card);
        }
        partyRoundCards.Clear();
        
        everythingUseful.Interact.ClearHighlightAttackableTiles();
        everythingUseful.Interact.ClearHighlightReachableTiles();
        
        everythingUseful.TurnSystem.RoundInfoArrowsScript.TurnOffAllArrows();
        
    }
}
