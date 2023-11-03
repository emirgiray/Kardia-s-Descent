using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DisableSelfDecision")]
    public class DisableSelfDecision : Decision

    {
        public float ResetWithTimer;
        [Multiline]
        public string DeveloperDescription = "Eğer karakterin damage yerse bu fonksiyon true verir ve DamageState'ine geçer.";
        public override bool Decide(StateController controller)
        {
            return DisableCheck(controller);
        }

        private bool DisableCheck(StateController controller)
        {


            if (controller.CheckIfCountDownElapsed(ResetWithTimer))

            {
                controller.gameObject.SetActive(false);
                return true;
            
            }
            else
            {

                return false;
            }


        }

    }
}


