using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckSummonDecision", menuName = "MyPluggableAI_V2/Decisions/CheckSummonDecision", order = 9)]

public class CheckSummonDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckSummon(controller);
    }

    private bool CheckSummon(StateController controller)
    {
        bool result = false;
        SkillContainer.Skills summonSkill = null;
        foreach (var skill in controller.skillContainer.skillsList)
        {
            //Debug.Log($"{skill.skillData.name} ready: {skill.skillReadyToUse} , remaining cooldown: {skill.remainingSkillCooldown} , cooldown: {skill.skillCooldown}");
            //  Debug.Log($"{skill.skillData.skillClass == SkillsData.SkillClass.Summon}");
            //Debug.Log($"skill: {skill.damage}");
            if (skill.skillData.skillClass == SkillsData.SkillClass.Summon && skill.skillReadyToUse  && controller.enemy.remainingActionPoints >= skill.actionPointUse)
            {
                
                result = true; // confirm that there is a summon skill ready to use
                summonSkill = skill;
                
                controller.decidedMoveTile = controller.enemy.characterTile;
                controller.decidedAttackSkill = skill;
                controller.skillContainer.SelectSkill(controller.decidedAttackSkill);
                
                break;
            }
        }

        if (result)
        {
            foreach (var waypoint in controller.waypoints) // we use waypoints as spawn points for summoned units
            {
                //Debug.Log($"{waypoint.name} occupied: {waypoint.Occupied} , selectable: {waypoint.selectable}");
                if (!waypoint.Occupied && waypoint.selectable) // if there is at least one empty waypoint, return true
                {
                    result = true;
                    break;
                }
                
                result = false;
                
            }
        }

        //Debug.Log($"result: {result}");
        return result;
    }
}
