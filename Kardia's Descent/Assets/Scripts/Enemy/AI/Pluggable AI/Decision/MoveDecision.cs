using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveDecision", menuName = "MyPluggableAI/Decisions/MoveDecision", order = 0)]
public class MoveDecision : DecisionAI
{
    //[SerializeField] private List<Tile> attackableTiles;

    public  override bool Decide(StateController controller)
    {
        return Move(controller);
    }

    private bool Move(StateController controller)
    {
        //attackableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.selectedSkill.skillRange);
        foreach (var tile in Pathfinder.Instance.GetAttackableTiles(controller.enemy.characterTile, controller.skillContainer.selectedSkill.skillRange))
        {
            if (tile == controller.targetPlayer.characterTile)//if character is already in range of the skill dont move
            {
                Debug.Log("player in range of skill");
                return false;
            }

        }
        Debug.Log("player NOT in range of skill");
        return true;
    }
}
