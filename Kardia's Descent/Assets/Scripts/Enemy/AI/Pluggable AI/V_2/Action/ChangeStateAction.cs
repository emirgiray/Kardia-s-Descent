using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStateAction", menuName = "MyPluggableAI_V2/Actions/ChangeStateAction", order = 0)]
public class ChangeStateAction : ActionAI
{
    [SerializeField] private StateAI ChangeToThisState = null;
    [Multiline] [SerializeField] public string DeveloperDescription = "Used to change state, needed for back to back actions to have a delay between them.";

    public override void Act(StateController controller)
    {
        controller.currentState = ChangeToThisState;
    }

    
        

}
