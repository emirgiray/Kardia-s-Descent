using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DistanceLowerAndGreater")]
    public class DistanceLowAndGrtr : Decision

    {

        public float LowerDistance;
        public float GreaterDistance;
        [Multiline]
        public string DeveloperDescription = "Eğer belirlediğimiz Distanceların arasında işlemler gerçekleşir";
        public override bool Decide(StateController controller)
        {
            return DistanceLowCheck(controller);
        }

        private bool DistanceLowCheck(StateController controller)
        {


            if (Vector3.Distance(controller.PlayerCenterPosition.transform.position, controller.transform.position) < LowerDistance && Vector3.Distance(controller.PlayerCenterPosition.transform.position, controller.transform.position) > GreaterDistance)

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


