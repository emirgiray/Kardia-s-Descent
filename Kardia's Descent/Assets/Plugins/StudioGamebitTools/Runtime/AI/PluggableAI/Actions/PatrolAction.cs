using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            if (controller.navMeshAgent.pathPending&& controller.navMeshAgent.isActiveAndEnabled)
            {
                controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
                controller.navMeshAgent.isStopped = false;
       
      

            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.slowdownDistance && !controller.navMeshAgent.pathPending)
            {
                controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
            }

            }
        }
    }
}




