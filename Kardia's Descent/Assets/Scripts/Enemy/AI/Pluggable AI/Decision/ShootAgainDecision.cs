using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAgainDecision", menuName = "MyPluggableAI/Decisions/ShootAgainDecision", order = 0)]
public class ShootAgainDecision : DecisionAI    
{
    public override bool Decide(StateController controller)
    {
        return ShootAgain(controller);
    }

    private bool ShootAgain(StateController controller)
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
