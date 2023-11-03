using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace SGT_Tools.AI
{

    [CreateAssetMenu(menuName = "PluggableAI/Actions/NavigationOnOffAction")]
    public class NavOnOffAction : Action
    {
        [SerializeField]private bool NavigationAgent = false;

        public override void Act(StateController controller)
        {
            NavCheck(controller);
        }



        private void NavCheck(StateController controller)
        {
        
            controller.navMeshAgent.enabled = NavigationAgent;
        
        }
    }
}


