using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/NavmeshRotationAction")]
    public class NavmeshRotationAction : Action
    {
        [SerializeField] private bool NavRotate = false;
        public override void Act(StateController controller)
        {
            Idle(controller);
        }

        private void Idle(StateController controller)
        {

            controller.navMeshAgent.updateRotation = NavRotate;

        }
    }
}


