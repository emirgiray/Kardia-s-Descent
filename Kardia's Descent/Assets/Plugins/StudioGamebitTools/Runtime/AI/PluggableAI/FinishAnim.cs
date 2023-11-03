using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    public class FinishAnim : StateMachineBehaviour {

        [SerializeField] private string StateMachineName = "";
        public StateController controller;
        [Tooltip("Eğer StateMachinedan çıkarken exit yapmak istersen bunu true olarak kullan ama animasyondan exit yapmak istiyorsan false bırak")]
        [SerializeField]private bool StateMachineExit = false;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (controller != null&& !StateMachineExit && stateInfo.IsName(StateMachineName))
            {
                controller.AnimFinish(StateMachineName);
                controller.AnimOver = true;
            
           
            }
            else if (controller == null)
            {
           
            }


        }


        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {


        }

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if (controller != null&& StateMachineExit)
            {
            
            
            }
            else if (controller == null)
            {
           
            }


        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {


        //}
    }

}

