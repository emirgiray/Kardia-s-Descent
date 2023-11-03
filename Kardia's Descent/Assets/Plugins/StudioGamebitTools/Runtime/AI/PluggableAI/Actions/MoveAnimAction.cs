using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace SGT_Tools.AI
{

    [CreateAssetMenu(menuName = "PluggableAI/Actions/MoveAnimAction")]
    public class MoveAnimAction : Action
    {

        [SerializeField]private string Walk = null;
        [SerializeField] private string Strafe = null;
        [Tooltip("Eğer obje gittiği yöne dönsün istiyorsan Tik at True yap")]
        [SerializeField] private bool Rotate = false;

        public override void Act(StateController controller)
        {
            MoveAnim(controller);
        }


        private void MoveAnim(StateController controller)
        {

           // controller.navMeshAgent.updateRotation = Rotate;

            /*if (Strafe!=null)
            {
                controller.Anim.SetFloat(Strafe, controller.transform.InverseTransformDirection(controller.navMeshAgent.desiredVelocity).x, 0.1f, Time.deltaTime);
            }*/
        
            Vector3 relVelocity = controller.transform.InverseTransformDirection(controller.navMeshAgent.velocity);
            relVelocity.y = 0;
                
         
            
            if (Walk!=null)
            {
                controller.Anim.SetFloat(Walk,  relVelocity.magnitude);
            }
        

        }




    }
}




