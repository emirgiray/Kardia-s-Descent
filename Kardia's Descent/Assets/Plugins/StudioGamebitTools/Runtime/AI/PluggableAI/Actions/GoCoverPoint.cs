
using UnityEngine;



namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/GoCoverPoint")]
    public class GoCoverPoint : Action
    {
       
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
         
            controller.navMeshAgent.destination = controller.CoverPoint.position;
                

        }
    }
}