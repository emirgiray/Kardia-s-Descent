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
            controller.decidedMoveTile, controller.targetPlayer.characterTile, controller.skillContainer.selectedSkill);//reduce accuracy if target player in cover
        
        UseSkill(controller, controller.skillContainer.selectedSkill, controller.targetPlayer);
    }

    private void UseSkill(StateController controller, SkillContainer.Skills controllerSelectedSkill, Player controllerTargetPlayer)
    {
        /*controllerSelectedSkill.ActivateSkill(controller.enemy.gameObject, controllerTargetPlayer.characterTile, () =>
        {
            controllerSelectedSkill = null;
            controller.enemy.AttackEnd();
            /*if (controllerSelectedSkill != null)
            {
                controller.skillContainer.DeselectSkill(controllerSelectedSkill);
            }#1#

            
        });*/
        
        controller.skillContainer.UseSkill(controllerSelectedSkill, controllerTargetPlayer.characterTile, controller.enemy, ()=> controller.canExitState = true);

    }
}


