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
        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove)
        {
            if (controller.enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder /*&& doonce*/)
            {
                 controller.enemy.StartMove(FindPathToTargetPlayer(controller), () => controller.canExitState = true);
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
      return Pathfinder.Instance. GetPathBetween(controller.enemy.characterTile, controller.decidedMoveTile, true);
    }
}
