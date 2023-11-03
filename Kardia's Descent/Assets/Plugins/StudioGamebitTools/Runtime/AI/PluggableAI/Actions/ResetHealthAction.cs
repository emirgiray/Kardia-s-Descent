using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/ResetHealthAction")]
    public class ResetHealthAction : Action
    {



        public override void Act(StateController controller)
        {
            ResetHealth(controller);
        }

        private void ResetHealth(StateController controller)
        {

            controller.Health.HealthAi = controller.Health.Max;

        }




    }
}



