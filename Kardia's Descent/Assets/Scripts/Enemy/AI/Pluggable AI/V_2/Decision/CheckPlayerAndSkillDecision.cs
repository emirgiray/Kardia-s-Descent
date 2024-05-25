using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckPlayerAndSkillDecision", menuName = "MyPluggableAI_V2/Decisions/CheckPlayerAndSkillDecision", order = 8)]

public class CheckPlayerAndSkillDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return Check(controller);
    }

    private bool Check(StateController controller)
    {
        bool result = false;
        int tileScore = 0;
        int prevTileScore = 0;

        if (controller.forcedSkillToUse.skillData == null) // default choosing state, there is no forced skill
        {
            foreach (var skill in controller.skillContainer.skillsList)
            {
                result = CheckForCurrentTile(controller, controller.enemy.characterTile, skill, tileScore, prevTileScore, result);
            }
        }
        else // a forced skill is set, check if it can be used
        {
            result = CheckForCurrentTile(controller, controller.enemy.characterTile, controller.forcedSkillToUse, tileScore, prevTileScore, result);
        }

        //Debug.Log($"Result: {result}");
        return result;
    }

    private bool CheckForCurrentTile(StateController controller, Tile tile, SkillContainer.Skills skill, int tileScore, int prevTileScore, bool result)
    {
        if (skill.skillReadyToUse )
        {
            Action oncompleted;
            int tilesChecked = 0;
            
            var attackTiles = controller.pathfinder.GetAttackableTiles(controller.enemy, skill, tile, tile, out oncompleted);
            
            foreach (var attackTile in attackTiles) // target tile is not set yet but its not important until UseSkillAction
            {
                if (skill.skillData.skillClass == SkillsData.SkillClass.Summon) // we dont want to summon here, another decision will handle that
                {
                    continue;
                }
                tilesChecked++;
                //Debug.Log($"{attackTiles.Count}, skill range: {skill.range}, tiles checked: {tilesChecked} /");
                if (attackTile.occupiedByPlayer && !attackTile.occupyingPlayer.isDead && attackTile.occupyingPlayer.inCombat)
                {
                  
                    if (skill.skillData.skillType == SkillsData.SkillType.Melee)
                    {
                        tileScore = 50 + skill.damage;
                    }
                    else
                    {
                         //Debug.Log($"enemy is in cover");
                        tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverDamageDebuff(tile, attackTile, skill) /*- controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill)*/;
                    }

                    if (skill.skillData.skillClass == SkillsData.SkillClass.Summon) // we dont want to summon here, another decision will handle that
                    {
                        tileScore = -9999999;
                    }
                    
                    //Debug.Log($"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} /");
                    if (tileScore > prevTileScore)
                    {
                        controller.decidedAttackSkill = skill;
                        
                        controller.targetPlayerTile = controller.forcedTargetPlayerTile == null ? attackTile : controller.forcedTargetPlayerTile; //if forced target is set, use it, otherwise use the attack tile
                         //Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / ");
                        result = true; //this means cover found 
                        prevTileScore = tileScore;
                    }
                        
                    
                  
                }
            }
        }

        controller.decidedMoveTile = tile;
        return result;
    }
    
}
