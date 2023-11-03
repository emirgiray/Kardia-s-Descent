using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/AnimFinishCheckDecision")]
    public class AnimFinishCheckDecision : Decision

    {
   
        [Multiline]
        public string DeveloperDescription = "!!! Dikkat et animasyona'a Tag eklemelisin.AnimControllerdan Animasyona Tag Eklemezsen çalışmaz.";
        public override bool Decide(StateController controller)
        {
            return AnimOverCheck(controller);
        }

        private bool AnimOverCheck(StateController controller)
        {


            if (controller.AnimOver)

            {
                //controller.AnimOver = false;
                //controller.OneTimeAnim = false; // Tekrar aktif olabilmesi için burdan resetliyorum.
           
                return true;

            }
            else
            {

                return false;
            }


        }

    }
}

