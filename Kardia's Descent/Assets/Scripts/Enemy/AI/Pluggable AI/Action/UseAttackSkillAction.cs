using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseAttackSkillAction", menuName = "MyPluggableAI/Actions/UseAttackSkillAction", order = 0)]
public class UseAttackSkillAction : ActionAI
{
    public override void Act(StateController controller)
    {
        UseSkill(controller, controller.skillContainer.selectedSkill, controller.targetPlayer);
    }

    private void UseSkill(StateController controller, SkillsData controllerSelectedSkill, Player controllerTargetPlayer)
    {
        /*controllerSelectedSkill.ActivateSkill(controller.enemy.gameObject, controllerTargetPlayer.characterTile, () =>
        {
            controllerSelectedSkill = null;
            controller.enemy.AttackEnd();
            /*if (controllerSelectedSkill != null)
            {
                controller.skillContainer.DeslectSkill(controllerSelectedSkill);
            }#1#

            
        });*/
        
        controller.skillContainer.UseSkill(controllerSelectedSkill, controllerTargetPlayer.characterTile, controller.enemy);
    }
}


