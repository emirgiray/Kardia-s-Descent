using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionAI : ScriptableObject
{
    public abstract bool Decide(StateController controller);
}
