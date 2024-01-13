using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Tile objectTile;
    [SerializeField] private bool findTileAtStart = false;
    
    [SerializeField] private float yOffset = 0.18f;
    
    public enum CharacterClass
    {
        None, Heart, Player, Chest
    }

    public CharacterClass InteractableType;

    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private HeartData heart;

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
        
    }

    public void OnInteract(Character character)
    {
        OnInteractEvent.Invoke();
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
                objectTile.Occupied = false;
                objectTile.ResetOcupying();
                Destroy(gameObject);
                player.FinalizePosition(objectTile, true);
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
