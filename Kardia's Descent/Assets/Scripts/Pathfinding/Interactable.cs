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
        None, Heart, Character, Chest
    }

    public CharacterClass InteractableType;

    [ShowIf("InteractableType", CharacterClass.Heart)]
    [SerializeField] private HeartData heart;
    
    
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
                Debug.LogError("Cover point: " + gameObject.name + " has no object tile !!! ");
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
                break;
            case CharacterClass.Character:
                throw new ArgumentOutOfRangeException();
                break;
            case CharacterClass.Chest:
                throw new ArgumentOutOfRangeException();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        objectTile.Occupied = false;
        objectTile.ResetOcupying();
        
        Destroy(gameObject);
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
        
        Debug.Log("No tile found for cover point: " + gameObject.name + " at position: " + transform.position + "");
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
