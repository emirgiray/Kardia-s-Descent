using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/RandomWaitTimeDecision")]
    public class RandomWaitTimeDecision : Decision
    {

	    //Random bir rakamla bekletip diğer state'e geçer. State Controllerdan değişkeninini ve zamanını ayarlayabilirsin.

        public override bool Decide(StateController controller)
        {
            return RandomWaitDecisions(controller);
        }

    
        private bool RandomWaitDecisions(StateController controller)
        {
       

		    if (controller.RandomWait())

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



