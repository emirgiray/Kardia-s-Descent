using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MoveToDecidedTileAction", menuName = "MyPluggableAI_V2/Actions/MoveToDecidedTileAction", order = 1)]
public class MoveToDecidedTileAction : ActionAI
{
     public override void Act(StateController controller)
    {
        Move(controller);
    }
 // bool doonce = true;
    private void Move(StateController controller)//this INCLUDES the player tile
    {
        
        
        controller.canExitState = false;
        if (controller.enemy.everythingUseful.TurnSystem.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove)
        {
            if (controller.enemy.turnOrder == controller.enemy.everythingUseful.TurnSystem.currentEnemyTurnOrder /*&& doonce*/)
            {
                if (controller.enemy.characterTile == controller.decidedMoveTile)
                {
                    // Debug.Log($"DECIDEDI MOVE TILE IS SAME AS ENEMY TILE");
                    controller.canExitState = true;
                    return;
                }
                else
                {
                    controller.enemy.StartMove(FindPathToTargetPlayer(controller), true, () => controller.canExitState = true);
                }
                
                 
                 // doonce = false;

            }
        }
    }

    /*private void OnDisable()
    {
        doonce = true;
    }*/
    
    private Path FindPathToTargetPlayer(StateController controller)
    {
        Path path = controller.pathfinder.GetPathBetween(controller.enemy, controller.enemy.characterTile, controller.decidedMoveTile/*, true*/);

        if (path.tiles[path.tiles.Count - 1].Occupied)
        {
            path.tiles.RemoveAt(path.tiles.Count - 1);
            return path;
        }
        else
        {
            return path;
        }
        
    }
}
