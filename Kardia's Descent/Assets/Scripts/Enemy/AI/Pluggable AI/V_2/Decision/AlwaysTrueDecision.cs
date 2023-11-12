using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlwaysTrueDecision", menuName = "MyPluggableAI_V2/Decisions/AlwaysTrueDecision", order = 0)]

public class AlwaysTrueDecision : DecisionAI
{
    [Multiline]
    [SerializeField] public string DeveloperDescription = "Always returns true";

    public override bool Decide(StateController controller)
    {
        return true;
    }
}

