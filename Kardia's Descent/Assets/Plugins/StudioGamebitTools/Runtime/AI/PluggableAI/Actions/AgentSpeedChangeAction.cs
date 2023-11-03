
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/AgentSpeedChangeAction")]
    public class AgentSpeedChangeAction : Action
    {


        [SerializeField] private float agentSpeed;
        
        public override void Act(StateController controller)
        {
            Anim(controller);
        }

        private void Anim(StateController controller)
        {

            controller.navMeshAgent.maxSpeed = agentSpeed;

        }




    }
}
