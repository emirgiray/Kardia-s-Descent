using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/AnimTestAction")]
    public class AnimAction : Action
    {


        [SerializeField] private string[] AnimName = null;
        [SerializeField] private bool AfterCompletedFinishAnim = false;
        [SerializeField] private bool DamageAnim = false;
        public override void Act(StateController controller)
        {
            Anim(controller);
        }

        private void Anim(StateController controller)
        {

            controller.AnimController(AnimName, AfterCompletedFinishAnim, DamageAnim);

        }




    }
}

