using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/ChangeStateAction")]
    public class ChangeStateAction : Action
    {
        [SerializeField] private State ChangeTothisState = null;
        [Multiline]
        [SerializeField] public string DeveloperDescription = "State'i değiştirir.Resetlemek için kullanıyorum";

        public override void Act(StateController controller)
        {
            Damage(controller);
        }


        private void Damage(StateController controller)
        {

            ChangeState(controller);



        }


        private void ChangeState(StateController controller)
        {



            controller.currentstate = ChangeTothisState;

        }
    

    }
}





