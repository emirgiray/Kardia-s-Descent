using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckActionPointsDecision", menuName = "MyPluggableAI_V2/Decisions/CheckActionPointsDecision", order = 0)]
public class CheckActionPointsDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckActionPoints(controller);
    }

    private bool CheckActionPoints(StateController controller)
    {
        if (controller.enemy.remainingActionPoints > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

