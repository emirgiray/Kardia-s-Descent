using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckMoveSkillDecision", menuName = "MyPluggableAI_V2/Decisions/CheckMoveSkillDecision", order = 3)]
public class CheckMoveSkillDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckMoveSkill(controller);
    }
    int prevTileScore = 0;
    private bool CheckMoveSkill(StateController controller)
    {
        int tilesChecked = 0;
        bool result = false;
        int tileScore = 0;
        prevTileScore = 0;
        int tilesWithoutCover = 0;
        int tilesWithNoActionPointLeftAfterMove = 0;

        List<Tile> reachableTiles = controller.GetReachableTiles();
        
        foreach (var tile in reachableTiles)
        {
            int reaminingActionPointsAfterMove = controller.enemy.remainingActionPoints;
            reaminingActionPointsAfterMove -= controller.pathfinder.GetTilesInBetween(controller.enemy, controller.enemy.characterTile, tile).Count + 1;
            tilesChecked++;
           
            if (controller.skillForced == false) // default choosing state, there is no forced skill
            {
                foreach (var skill in controller.skillContainer.skillsList)
                {
                    result = CheckForEachTile(controller, reaminingActionPointsAfterMove, reachableTiles, tile, tilesChecked, skill, tileScore, prevTileScore, tilesWithoutCover, result);
                }
            }
            else // a forced skill is set, check if it can be used
            {
                result = CheckForEachTile(controller, reaminingActionPointsAfterMove, reachableTiles, tile, tilesChecked, controller.forcedSkillToUse, tileScore, prevTileScore, tilesWithoutCover, result);
            }
            
        }
        
        prevTileScore = 0;

        //Debug.Log($"RETURNED AT THE END, move : {result},  tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
        return result;
    }

    private bool CheckForEachTile(StateController controller, int reaminingActionPointsAfterMove, List<Tile> reachableTiles, Tile tile, int tilesChecked, SkillContainer.Skills skill,
        int tileScore, int prevTileScore, int tilesWithoutCover, bool result )
    {
        if (skill.skillReadyToUse && reaminingActionPointsAfterMove >= skill.actionPointUse)
        {
            Action oncompleted;
            foreach (var attackTile in controller.pathfinder.GetAttackableTiles(controller.enemy ,skill, tile,tile, out oncompleted)) // target tile is not set yet but its not important until UseSkillAction
            {
                if (attackTile.occupiedByPlayer && !attackTile.occupyingPlayer.isDead && attackTile.occupyingPlayer.inCombat)
                {
                    //Debug.Log($"enemy is in cover");
                    /*tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverDamageDebuff(tile, attackTile, skill) /*- controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill)#1#
                        + controller.pathfinder.GetTilesInBetween(controller.enemy, tile, attackTile, true).Count * 10;*/
                    /*
                    Debug.Log($"remaining ap after move: {reaminingActionPointsAfterMove}");
                    Debug.Log(
                        $"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}");
                        */
                    
                    if (skill.skillData.skillType == SkillsData.SkillType.Melee || controller.enemy.characterClass == Character.CharacterClass.Melee)
                    {
                        tileScore = 50 + skill.damage;
                    }
                    else
                    {
                        // Debug.Log($"enemy is in cover");
                        tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverDamageDebuff(tile, attackTile, skill) /*- controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill)*/;
                    }

                    if ( tileScore > this.prevTileScore)
                    {
                        controller.decidedAttackSkill = skill;
                        controller.decidedMoveTile = tile;
                        controller.targetPlayerTile = controller.forcedTargetPlayerTile == null ? attackTile : controller.forcedTargetPlayerTile; //if forced target is set, use it, otherwise use the attack tile
                        // Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                        result = true; //this means move found 
                        
                        this.prevTileScore = tileScore;
                    }
                    
                    if (tilesChecked >= reachableTiles.Count)
                    {
                      // Debug.Log($"move= {result} tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                        return result;
                    }
                    
                }
                
            }
        }
    
        return result;
    }
}

