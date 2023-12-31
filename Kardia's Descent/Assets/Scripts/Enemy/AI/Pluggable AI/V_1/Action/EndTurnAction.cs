using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndTurnAction", menuName = "MyPluggableAI/Actions/EndTurnAction", order = 0)]
public class EndTurnAction : ActionAI
{
    public override void Act(StateController controller)
    {
        //Debug.Log($"{this.name} Ended Turn");
        controller.EndTurn();
    }
}
