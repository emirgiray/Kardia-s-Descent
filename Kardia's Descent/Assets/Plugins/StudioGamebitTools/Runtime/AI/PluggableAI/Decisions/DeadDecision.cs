using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DeadDecision")]
    public class DeadDecision : Decision

    {
    
        [Multiline]
        public string DeveloperDescription = "Eğer karakterin Health'i 0 altına düşerse ölüm ilanı verilir.";
        public override bool Decide(StateController controller)
        {
            return DeadCheck(controller);
        }

        private bool DeadCheck(StateController controller)
        {


            if (controller.Health.isDead==true)

            {
                return true;
            }
            else
            {
           
                return false;
            }


        }

    }
}

