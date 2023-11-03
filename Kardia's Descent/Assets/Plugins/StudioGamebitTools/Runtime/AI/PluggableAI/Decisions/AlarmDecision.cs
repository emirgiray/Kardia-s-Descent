
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/AlarmDecision")]
    public class AlarmDecision : Decision

    {

       
        public override bool Decide(StateController controller)
        {
            return AnimOverCheck(controller);
        }

        private bool AnimOverCheck(StateController controller)
        {


            if (controller.alarm)

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
