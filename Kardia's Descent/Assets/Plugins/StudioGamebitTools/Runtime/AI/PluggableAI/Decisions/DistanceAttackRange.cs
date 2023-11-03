using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DistanceAttackRangeDecision")]
    public class DistanceAttackRange : Decision
    {
        public float Distance;
        public override bool Decide(StateController controller)
        {
            return Scan(controller);
        }

        private bool Scan(StateController controller)
        {


		    if (Vector3.Distance(controller.PlayerCenterPosition.transform.position, controller.transform.position) <= Distance)

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

