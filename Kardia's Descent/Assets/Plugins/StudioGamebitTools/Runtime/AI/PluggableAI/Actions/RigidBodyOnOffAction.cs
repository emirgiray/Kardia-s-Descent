using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/RigidbodyOnOffAction")]
    public class RigidBodyOnOffAction : Action
    {
    
        [SerializeField] private bool IsKinematic = false;
        [SerializeField] private bool Gravity = false;
    

        public override void Act(StateController controller)
        {
            RigidbodyControl(controller);
        }



        private void RigidbodyControl(StateController controller)
        {
            controller.rb.isKinematic = IsKinematic;
            controller.rb.useGravity = Gravity;

        }
    }
}


