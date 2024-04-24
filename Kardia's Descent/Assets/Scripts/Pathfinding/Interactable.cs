using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    [SerializeField] private Tile objectTile;
    [SerializeField] private bool findTileAtStart = false;
    [SerializeField] private float yOffset = 0.18f;

    [SerializeField] private VFXSpawner VFX;
    
    
    public enum CharacterClass
    {
        None, Heart, Player, Chest
    }

    public CharacterClass InteractableType;

    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private HeartData heart;
    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private MeshRenderer heartRenderer;
    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private GameObject heartVFX;
    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private Material[] rarityMaterials;
    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private GameObject[] rarityVFXs;

    
    
    [ShowIf("InteractableType", CharacterClass.Player)]
    [SerializeField] private Player player;

    
    public UnityEvent OnInteractEvent;
    
    void Start()
    {
        if (findTileAtStart)
        {
            FindTileAtstart();
        }
        else
        {
            if (objectTile == null)
            {
                Debug.LogError("Interactable: " + gameObject.name + " has no object tile !!!");
            }
        }

        switch (InteractableType)
        {
            case CharacterClass.None:
                break;
            case CharacterClass.Heart:
                switch (heart.heartRarity)
                {
                    case HeartData.HeartRarity.Common:
                        heartRenderer.material = rarityMaterials[0];
                        Instantiate(rarityVFXs[0], heartVFX.transform.position, Quaternion.identity, heartVFX.transform);
                        break;
                    case HeartData.HeartRarity.Rare:
                        heartRenderer.material = rarityMaterials[1];
                        Instantiate(rarityVFXs[1], heartVFX.transform.position, Quaternion.identity, heartVFX.transform);
                        break;
                    case HeartData.HeartRarity.Legendary:
                        heartRenderer.material = rarityMaterials[2];
                        Instantiate(rarityVFXs[2], heartVFX.transform.position, Quaternion.identity, heartVFX.transform);
                        break;
                }

                
                break;
            case CharacterClass.Player:
                
                player.animator.enabled = true;
                player.animator.SetTrigger("Sleep");
                break;
            case CharacterClass.Chest:
                
                break;

                
        }
        
    }

    public void OnInteract(Character character)
    {
        OnInteractEvent.Invoke();
        if (VFX != null) VFX.SpawnVFX(gameObject.transform);
        switch (InteractableType)
        {
            case CharacterClass.None:
                break;
            case CharacterClass.Heart:
                character.heartContainer.PickUpHeart(heart);
                objectTile.Occupied = false;
                objectTile.ResetOcupying();
                Destroy(gameObject);
                break;
            
            case CharacterClass.Player:
                player.UnlockPlayer();
                everythingUseful.LevelManager.PlayerUnlocked(player.transform);
                objectTile.Occupied = false;
                objectTile.ResetOcupying();
                
                player.UnlockTile = objectTile; 
                //player.FinalizePosition(objectTile, true);
                
                GameObject tempPreview = Instantiate(player.PlayerPreview, everythingUseful.MainPrefabScript.playerPreviewParent.transform);
                tempPreview.transform.localPosition = new Vector3(everythingUseful.MainPrefabScript.SelectedPlayers.Count -1 * 10, 0, 0);
                //previews.Add(tempPreview);
            
                GameObject tempPartyRoundCard = Instantiate(everythingUseful.MainPrefabScript.PartyRoundCardPrefab, everythingUseful.MainPrefabScript.PartyRoundCardsSlot);
                //tempPartyRoundCard.GetComponent<CharacterRoundCard>().Init(player, everythingUseful.TurnSystem.RoundInfo.GetComponent<RoundInfo>());
                tempPartyRoundCard.GetComponent<CharacterRoundCard>().Init(player, everythingUseful.TurnSystem.RoundInfo.GetComponent<RoundInfo>(), true);
                tempPartyRoundCard.GetComponent<Image>().sprite = player.characterSprite;
                //partyRoundCards.Add(tempPartyRoundCard);
                
                
                UnlockabledCharacterData unlockabledCharacterData = new UnlockabledCharacterData()
                {
                    playerID = player.playerID,
                    isUnlocked = true
                };
                everythingUseful.AllPlayers.allPlayers[player.playerID].isUnlocked = true;

                everythingUseful.SaveLoadSystem.metaSaveData.UnlockableCharacterDatas.Clear();
                for (int i = 0; i < everythingUseful.SaveLoadSystem.allPlayers.allPlayers.Count; i++)
                {
                    everythingUseful.SaveLoadSystem.metaSaveData.UnlockableCharacterDatas.Add(new UnlockabledCharacterData()
                    {
                        playerID = everythingUseful.SaveLoadSystem.allPlayers.allPlayers[i].playerID,
                        isUnlocked = everythingUseful.SaveLoadSystem.allPlayers.allPlayers[i].isUnlocked
                    });
                }
                player.animator.SetTrigger("WakeUp");
                everythingUseful.SaveLoadSystem.MetaSave();
                everythingUseful.Interact.TrySelectPlayer(player);
                everythingUseful.Interact.CharacterSelectedAction?.Invoke(player.characterTile, 1f);
               
                Destroy(gameObject);
                break;
            case CharacterClass.Chest:
                throw new ArgumentOutOfRangeException();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
    
    public void FindTileAtstart()
    {
        if (objectTile != null)
        { 
            FinalizePosition(objectTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, PathfinderVariables.Instance.tileMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
        
        Debug.Log("No tile found for interactable: " + gameObject.name + " at position: " + transform.position + "");
    }

    [Button, GUIColor(0.1f, 1f, 0.1f)]
    public void FindTile()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, PathfinderVariables.Instance.tileMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
    }
    
    
    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0,yOffset,0);
        objectTile = tile;
        tile.Occupied = true;
        tile.occupiedByInteractable = true;
        tile.occupyingInteractable = this;
        tile.occupyingGO = gameObject;
        
    }
}
