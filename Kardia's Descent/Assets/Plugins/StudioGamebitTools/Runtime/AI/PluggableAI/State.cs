using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    [CreateAssetMenu (menuName ="PluggableAI/State") ]
    public class State : ScriptableObject
    {
        public Action[] actions;
        public Action[] ActionOnce;
        public Transition[] transitions;
        public Color sceneGizmoColor=Color.yellow;
        [Multiline]
        public string DeveloperDescription;
    
        public void UpdateState (StateController controller)
        {
            DoActions(controller);
            CheckTransitions(controller);

        }

        private void DoActions (StateController controller)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Act(controller);
            }
        }


        public void DoActionsOnce(StateController controller)
        {
            for (int i = 0; i < ActionOnce.Length; i++)
            {
                ActionOnce[i].Act(controller);
            }
        }



        private void CheckTransitions(StateController controller)


        {
            for (int i = 0; i < transitions.Length; i++)
            {
                bool decisionSucceeded = transitions[i].decision.Decide(controller);

                if (decisionSucceeded)
                {
                    controller.StateChange.Invoke(transitions[i].decision);
                    controller.TransitionToState(transitions[i].trueState);
                }
                else
                {
                    controller.TransitionToState(transitions[i].falseState);
                }
            }
        }

    }
}

