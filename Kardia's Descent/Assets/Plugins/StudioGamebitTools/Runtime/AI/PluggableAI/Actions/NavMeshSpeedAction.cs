using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/NavMeshSpeedAction")]
    public class NavMeshSpeedAction : Action
    {
        [SerializeField] private float NavSpeed =0;

        public override void Act(StateController controller)
        {
            NavCheck(controller);
        }



        private void NavCheck(StateController controller)
        {

            controller.navMeshAgent.speed = NavSpeed;

        }
    }
}





