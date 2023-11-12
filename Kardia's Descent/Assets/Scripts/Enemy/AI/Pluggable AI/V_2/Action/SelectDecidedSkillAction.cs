using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectDecidedSkillAction", menuName = "MyPluggableAI_V2/Actions/SelectDecidedSkillAction", order = 2)]
public class SelectDecidedSkillAction : ActionAI
{
    public override void Act(StateController controller)
    {
        controller.skillContainer.SelectSkill(controller.decidedAttackSkill);
    }
}
