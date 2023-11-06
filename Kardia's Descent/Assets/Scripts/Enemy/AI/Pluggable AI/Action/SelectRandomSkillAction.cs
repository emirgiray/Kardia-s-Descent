using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectRandomSkillAction", menuName = "MyPluggableAI/Actions/SelectRandomSkillAction", order = 0)]
public class SelectRandomSkillAction : ActionAI
{
    public override void Act(StateController controller)
    {
        SelectRandomSkill(controller);
    }

    public void SelectRandomSkill(StateController controller)
    {
        int random = Random.Range(0, controller.skillContainer.skillsList.Count);
        controller.skillContainer.SelectSkill(controller.skillContainer.skillsList[random].skillData ,controller.enemy); 
        //controller.skillContainer.skillSelected = true;
    }
}
