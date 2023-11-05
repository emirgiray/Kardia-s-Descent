using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class DecisionAI : ScriptableObject
{
    public abstract bool Decide(StateController controller);
}
