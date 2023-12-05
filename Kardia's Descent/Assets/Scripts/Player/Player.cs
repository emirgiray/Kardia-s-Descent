using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private RenderTexture playerPortrait;
    [FoldoutGroup("Stats For Quests")] [SerializeField] private int killsInTurn = 0;
    
    private void OnEnable()
    {
        TurnSystem.Instance.FriendlyTurn += base.StartTurn;
        TurnSystem.Instance.OnPlayerTurnEvent += CheckRemoveStun;
    }
    private void OnDisable()
    {
        TurnSystem.Instance.FriendlyTurn -= base.StartTurn;
        TurnSystem.Instance.OnPlayerTurnEvent -= CheckRemoveStun;
    }
    /*private void Update()
    {
        if (characterState == CharacterState.Attacking)
        {
            //Rotate(transform.position, Interact.Instance.currentTile.transform.position);
            if (Interact.Instance.isMouseOverUI == false)
            {
                // StartCoroutine(RotateEnum(transform.position, Interact.Instance.currentTile.transform.position));
                Rotate(transform.position, Interact.Instance.currentTile.transform.position);
            }
        }
    }*/

    /*private void Start()
    {
        switch (characterClass)
        {
            case CharacterClass.None:
                break;
            case CharacterClass.Tank:
                break;
            case CharacterClass.Rogue:
                break;
            case CharacterClass.Sniper:
                break;
            case CharacterClass.Bombardier:
                break;
            case CharacterClass.TheRegular:
                break;
            case CharacterClass.Medic:
                break;
            case CharacterClass.Support:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }*/

    public RenderTexture GetPlayerPortrait()
    {
        return playerPortrait;
    }

    [Button]
    public List<Tile> gettilesinbetween(Tile dest)
    {
        
        return Pathfinder.Instance.GetTilesInBetween(characterTile, dest, true);
    }
}
