using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseAttackSkillAction", menuName = "MyPluggableAI/Actions/UseAttackSkillAction", order = 0)]
public class UseAttackSkillAction : ActionAI
{
    public override void Act(StateController controller)
    {
        controller.canExitState = false;
        controller.skillContainer.selectedSkill.accuracy -= controller.skillContainer.CalculateCoverAccuracyDebuff(
            controller.decidedMoveTile, controller.targetPlayerTile, controller.skillContainer.selectedSkill);//reduce accuracy if target player in cover
        
        controller.skillContainer.selectedSkill.damage -= controller.skillContainer.CalculateCoverDamageDebuff(
            controller.decidedMoveTile, controller.targetPlayerTile, controller.skillContainer.selectedSkill);//reduce damage if target player in cover

        Action OnCompleteAddedAction;
        controller.pathfinder.GetAttackableTiles(controller.enemy, controller.skillContainer.selectedSkill, controller.enemy.characterTile, controller.targetPlayerTile, out OnCompleteAddedAction);
        
        controller.skillContainer.UseSkill(controller.skillContainer.selectedSkill, controller.targetPlayerTile, controller.enemy, ()=>
        {
            controller.canExitState = true;
            if (OnCompleteAddedAction != null) OnCompleteAddedAction();
            
            //if forced skill is the same as selected skill, then reset forced skill
            if (controller.forcedSkillToUse == controller.skillContainer.selectedSkill)
            {
                controller.forcedSkillToUse.skillData = null;
            }
            controller.forcedTargetPlayerTile = null;
            controller.lastUsedSkill = controller.skillContainer.selectedSkill;
        });
    }

   
}


