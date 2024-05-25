using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndTurnAction", menuName = "MyPluggableAI/Actions/EndTurnAction", order = 0)]
public class EndTurnAction : ActionAI
{
    public override void Act(StateController controller)
    {
        //Debug.Log($"{this.name} Ended Turn");
        controller.stateControllerMono.StartCoroutine(TurnSkipDelay(controller));
    }
    
    public IEnumerator TurnSkipDelay(StateController controller)
    {
        yield return new WaitForSeconds(1.5f);
        controller.EndTurn();
        
    }
}
