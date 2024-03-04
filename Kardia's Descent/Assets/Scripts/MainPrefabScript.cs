using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FischlWorks_FogWar;
using UnityEngine;

//remember to make this script execute first but after turn system
public class MainPrefabScript : MonoBehaviour
{
    public static MainPrefabScript Instance;
    
    public List<GameObject> SelectedPlayers = new();
    public List<GameObject> InventoryUISlots = new();

    public Transform PartyRoundCardsSlot;
    public GameObject PartyRoundCardPrefab;
    
    private List<Transform> PlayerSlots = new();
    private List<GameObject> spawnedPlayers = new();
    public List<Player> spawnedPlayerScripts = new();
    private csFogWar fogWar;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
       
    }

    public void InitializeLevel()
    {
        // todo load selected players from save system, also load health values hearts etc (or add players to dont destroy on load)
        
        fogWar = FindObjectOfType<csFogWar>();
        PlayerSlots = GameObject.Find("Player Slots").GetComponentsInChildren<Transform>().ToList();
        PlayerSlots.RemoveAt(0); //remove the parent transform
        GameObject playerPreviewParent = GameObject.Find("Player Preview");
        
        for (int i = 0; i < SelectedPlayers.Count; i++)
        {
            GameObject temp = Instantiate(SelectedPlayers[i], PlayerSlots[i].position, PlayerSlots[i].rotation/*, CharacterSlots[i]*/);
            temp.name = temp.name.Replace(" Variant(Clone)", "");
            spawnedPlayers.Add(temp);
            spawnedPlayerScripts.Add(temp.GetComponent<Player>());
            spawnedPlayerScripts[i].inventory.InventoryUISlot = InventoryUISlots[i];
            fogWar.AddFogRevealerRevelear(temp.transform);
            
            GameObject tempPreview = Instantiate(spawnedPlayerScripts[i].PlayerPreview, playerPreviewParent.transform);
            tempPreview.transform.localPosition = new Vector3(i * 10, 0, 0);
            
            GameObject tempPartyRoundCard = Instantiate(PartyRoundCardPrefab, PartyRoundCardsSlot);
            tempPartyRoundCard.GetComponent<CharacterRoundCard>().Init(spawnedPlayerScripts[i], TurnSystem.Instance.RoundInfo.GetComponent<RoundInfo>());
        }
    }
}
