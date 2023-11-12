using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "MyPluggableAI/State", order = 0)]
public class StateAI : ScriptableObject
{
    
    public ActionAI[] actions;
    public ActionAI[] actionsOnce;
    public TransitionAI[] transitions;
    public Color sceneGizmoColor = Color.gray;
    
    public void UpdateState(StateController controller)
    {
        if (controller.RandomWait())
        {
            DoActions(controller);
            CheckTransitions(controller);
        }
    }
    
    private void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }
    
    public void DoActionsOnce(StateController controller)
    {
        for (int i = 0; i < actionsOnce.Length; i++)
        {
            actionsOnce[i].Act(controller);
        }
    }
    
    private void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(controller);

            if (decisionSucceeded)
            {
                controller.TransitionToState(transitions[i].trueState);
            }
            else
            {
                controller.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
