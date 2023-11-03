using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/ListFullCheckDecision")]
    public class ListFullCheckDecision : Decision

    {
        [SerializeField] private int checkHowmanyChar = 0;
        [Multiline]
        public string DeveloperDescription = "Eğer Character Listesi boş ise true verir";
        public override bool Decide(StateController controller)
        {
            return CheckList(controller);
        }

        private bool CheckList(StateController controller)
        {


            if (controller.Characters.Count == checkHowmanyChar)

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

