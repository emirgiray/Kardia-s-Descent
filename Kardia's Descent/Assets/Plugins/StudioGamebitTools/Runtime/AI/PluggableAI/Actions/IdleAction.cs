using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/IdleAction")]
    public class IdleAction : Action
    {
    
        public override void Act(StateController controller)
        {
            Idle(controller);
        }

        private void Idle(StateController controller)
        {
            if (controller.navMeshAgent!=null && controller.navMeshAgent.pathPending)
            {
                controller.navMeshAgent.isStopped = true;
            }
        
        
        }
    }
}





