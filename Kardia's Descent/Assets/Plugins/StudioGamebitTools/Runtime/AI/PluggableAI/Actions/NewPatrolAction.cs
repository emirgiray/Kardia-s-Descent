
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/NewPatrol")]
    public class NewPatrolAction : Action
    {
        public float waitTimeInPatrolPoint = 3;
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
         
                controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
                
                
                if (controller.navMeshAgent.reachedDestination)
                {
                    if (controller.CheckIfCountDownElapsed(waitTimeInPatrolPoint))
                    {
                        controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
                        controller.stateTimeElapsed = 0;
                    }
                    
                }

            
        }
    }
}
   

