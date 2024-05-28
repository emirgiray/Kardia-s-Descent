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
            //Debug.Log($"controller.lastUsedSkill: {controller.lastUsedSkill.skillData.name}");
            if (skill != controller.lastUsedSkill && skill.skillReadyToUse && controller.enemy.remainingActionPoints >= skill.actionPointUse && skill.skillData.skillClass != SkillsData.SkillClass.Summon)
            {
                unusedSkills.Add(skill);
            }
        }
        if (controller.skillContainer.skillsList.Count == 1 || unusedSkills.Count == 0) return;
        controller.forcedSkillToUse = unusedSkills[Random.Range(0, unusedSkills.Count)];
        controller.skillForced = true;
        //Debug.Log($"controller.forcedSkillToUse: {controller.forcedSkillToUse.skillData.name}");
        
    }
}
