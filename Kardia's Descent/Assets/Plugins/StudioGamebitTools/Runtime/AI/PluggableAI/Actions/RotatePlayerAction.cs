using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace SGT_Tools.AI
{

    [CreateAssetMenu(menuName = "PluggableAI/Actions/RotatePlayerAction")]
    public class RotatePlayerAction : Action
    {

	    public float turnSpeed= 0.05f;


	    public override void Act(StateController controller)
	    {
		    RotatePlayer(controller);
		    TurnOn (controller);
	    }


	    private bool TurnOn(StateController controller)
	    {
		    if (controller.TurnActive) {
			    return true;
		    } else {
			    return false;
		    }

	    }



	    private void RotatePlayer(StateController controller)
	    {
		    if(TurnOn(controller)&& controller.TurnPlayer)
            {

		    Vector3 direction = controller.Player.gameObject.transform.position - controller.gameObject.transform.position;
		    direction.y = 0f;
		    controller.gameObject.transform.rotation = Quaternion.Slerp (controller.gameObject.transform.rotation, Quaternion.LookRotation (direction),turnSpeed);
		    }
	    }
    }
}








