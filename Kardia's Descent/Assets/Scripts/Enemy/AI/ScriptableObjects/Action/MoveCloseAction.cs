using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveCloseAction", menuName = "ScriptableObjects/AI/Actions/MoveCloseAction", order = 0)]
public class MoveCloseAction : ActionAI
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        
        
        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove && controller.RandomWait())
        {
            if (controller.enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder)
            {
                controller.enemy.StartMove(FindPathToTargetPlayer(controller, controller.targetPlayer));
               // controller.enemy.canMove = false;
                //controller.enemy.remainingMoveRange = 0;
            }
        }
    }

    List<Tile> reachableTiles = new List<Tile>();
    List<Tile> attackableTiles = new List<Tile>();
    List<Tile> tilesBetwen = new List<Tile>();
    
    private Path FindPathToTargetPlayer(StateController controller, Player targetPlayer)
    {
        reachableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.enemy.remainingMoveRange);
        attackableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.selectedSkill.skillRange);
        tilesBetwen = Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, targetPlayer.characterTile, true);
        //move until target player is in skillRange

        return Pathfinder.Instance.FindPath(controller.enemy.characterTile, tilesBetwen[controller.enemy.remainingMoveRange - 1]);

        
        /*foreach (var tile in attackableTiles)//maybe do this in a decision
        {
            if (tile == targetPlayer.characterTile)
            {
                return null;
            }

        }
        */
        
        /*foreach (var tile in reachableTiles)
        {
            if (tile == targetPlayer.characterTile)
            {
                return Pathfinder.Instance.FindPath(controller.enemy.characterTile, targetPlayer.characterTile);
            }
        }*/
        
        /*foreach (var tile in tilesBetwen)
        {
            foreach (var reachableTile in reachableTiles)
            {
                if (tile == reachableTile)
                {
                    return Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile);
                }
            }
        }*/
        
        // return null;
        
    }
}
