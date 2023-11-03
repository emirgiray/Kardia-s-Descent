

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/AnimBoolAction")]
    public class AnimBoolAction : Action
    {


        [SerializeField] private string AnimName = null;
        [SerializeField] private bool _Bool = false;
        public override void Act(StateController controller)
        {
            Anim(controller);
        }

        private void Anim(StateController controller)
        {

            controller.Anim.SetBool(AnimName,_Bool);

        }




    }
}

