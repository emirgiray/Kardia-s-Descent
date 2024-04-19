using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ForceChangeSkillAction", menuName = "MyPluggableAI_V2/Actions/ForceChangeSkillAction", order = 1)]

public class ForceChangeSkillAction : ActionAI
{
    public override void Act(StateController controller)
    {
        ForceChangeSkill(controller);
    }
    
    public void ForceChangeSkill(StateController controller)
    {
        
        List<SkillContainer.Skills> unusedSkills = new List<SkillContainer.Skills>();
        
        foreach (var skill in controller.skillContainer.skillsList)
        {
            if (skill != controller.lastUsedSkill)
            {
                unusedSkills.Add(skill);
            }
        }
        
        controller.forcedSkillToUse = unusedSkills[Random.Range(0, unusedSkills.Count)];
        
    }
}
