using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WaitUntilYourTurnDecision",
    menuName = "MyPluggableAI_V2/Decisions/WaitUntilYourTurnDecision", order = 1)]
public class WaitUntilYourTurnDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return WaitUntilYourTurn(controller);
    }

    private bool WaitUntilYourTurn(StateController controller)
    {
        /*if (controller.enemy.everythingUseful.TurnSystem.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove)
        {
            if (controller.enemy.turnOrder == controller.enemy.everythingUseful.TurnSystem.currentEnemyTurnOrder)
            {
                return true;
            }
        }*/

        if (controller.enemy.TurnSystem.turnState == TurnSystem.TurnState.Enemy && controller.enemy.turnOrder == controller.enemy.TurnSystem.currentEnemyTurnOrder)
        {
            return true;
        }

        return false;
    }
}
