
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/AnimFinishDecision")]
    public class AnimFinishDecision : Decision

    {

        [Multiline]
        public string DeveloperDescription = "!!! Dikkat et animasyonun ismini yazmalısın.";
        public string AnimName;
        public override bool Decide(StateController controller)
        {
            return AnimOverCheck(controller);
        }

        private bool AnimOverCheck(StateController controller)
        {


            if (controller.AnimFinish(AnimName))

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


