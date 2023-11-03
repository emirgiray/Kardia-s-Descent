using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/ListEmptyCheckDecision")]
    public class ListEmptyCheckDecision : Decision

    {
   
        [Multiline]
        public string DeveloperDescription = "Eğer Character Listesi boş ise true verir";
        public override bool Decide(StateController controller)
        {
            return CheckList(controller);
        }

        private bool CheckList(StateController controller)
        {


            if (controller.Characters.Count<=0)

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

