using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckEndOfTurnDecision", menuName = "MyPluggableAI/Decisions/CheckEndOfTurnDecisions", order = 0)]
public class CheckEndOfTurnDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return controller.enemy.remainingActionPoints <= 0;
    }

    public bool CheckIfEndTurn(StateController controller)
    {
        return CheckIfEndofMovePoints(controller) && CheckIfEndofActionPoints(controller);
    }
    
    public bool CheckIfEndofMovePoints(StateController controller)
    {
        return controller.enemy.remainingActionPoints <= 0;
    }
    public bool CheckIfEndofActionPoints(StateController controller)
    {
        return controller.enemy.remainingActionPoints <= 0;
    }
}
