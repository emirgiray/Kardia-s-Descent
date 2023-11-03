
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/CoverPointArriveDecision")]
    public class CoverPointArriveDecision : Decision

    {

      
        public override bool Decide(StateController controller)
        {
            return AnimOverCheck(controller);
        }

        private bool AnimOverCheck(StateController controller)
        {


            if (controller.navMeshAgent.reachedDestination)

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
