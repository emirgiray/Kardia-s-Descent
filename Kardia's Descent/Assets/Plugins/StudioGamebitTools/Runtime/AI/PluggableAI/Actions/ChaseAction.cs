using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
    public class ChaseAction : Action
    {
	    [Tooltip("Eğer player belirlediğimiz mesafe dışında hareket ederse player pozisyonunu update eder.")]
	    public float UpdatePosition=0.4f;
	    public float ChaseSpeed=2;
	    private Vector3 agentDestinationPos;

        public override void Act(StateController controller)
        {
            Chase(controller);
        }


	    private void Chase (StateController controller) {
		    // Update destination if the player moves one unit
		    //if (Vector3.Distance (agentDestinationPos, controller.Player.transform.position) > UpdatePosition) {

		    //	agentDestinationPos = controller.Player.transform.position;
			    GoTarget (controller);




		    //}
	    }


	    private	void GoTarget(StateController controller){  //Mesafe uzaksa player'a doğru koştur.

       

            if (controller.navMeshAgent.pathPending&& controller.navMeshAgent.isActiveAndEnabled)
            {
                agentDestinationPos = controller.PlayerCenterPosition.transform.position;
                controller.navMeshAgent.destination = agentDestinationPos;
                controller.navMeshAgent.isStopped = false;
                controller.navMeshAgent.speed = ChaseSpeed;
            
            }
				
				
				

	    }


    }
}


