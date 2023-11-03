using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObjects/AI/State", order = 0)]
public class StateAI : ScriptableObject
{
    
    public ActionAI[] actions;
    public Color sceneGizmoColor = Color.gray;
    
    public void UpdateState(StateController controller)
    {
        DoActions(controller);
    }
    
    private void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }
}
