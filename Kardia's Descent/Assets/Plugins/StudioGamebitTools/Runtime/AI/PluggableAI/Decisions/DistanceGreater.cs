using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DistanceGreater")]
    public class DistanceGreater : Decision
   
    {
        public float GreaterDistance;
        [Multiline]
        public string DeveloperDescription = "Eğer belirlediğimiz Distance'dan büyükse işlemler gerçekleşir";
        public override bool Decide(StateController controller)
        {
            return Scan(controller);
        }

        private bool Scan(StateController controller)
        {


            if (Vector3.Distance(controller.PlayerCenterPosition.transform.position, controller.transform.position) > GreaterDistance)

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



