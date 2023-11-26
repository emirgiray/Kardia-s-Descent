using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Character
{
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

    [Button]
    public List<Tile> gettilesinbetween(Tile dest)
    {
        
        return Pathfinder.Instance.GetTilesInBetween(characterTile, dest, true);
    }
}
