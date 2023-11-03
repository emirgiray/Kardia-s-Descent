using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/NavMeshIsStop")]
    public class NavMeshIsStop : Action
    {
        [SerializeField] private bool IsStopTrue = false;
        public override void Act(StateController controller)
        {
            Idle(controller);
        }

        private void Idle(StateController controller)
        {
            if (controller.navMeshAgent.pathPending && controller.navMeshAgent.isActiveAndEnabled)
            {
                controller.navMeshAgent.isStopped = IsStopTrue;
            }
        }
    }
}



