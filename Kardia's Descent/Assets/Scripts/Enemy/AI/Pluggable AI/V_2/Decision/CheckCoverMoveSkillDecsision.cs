using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckCoverMoveSkillDecsision",
    menuName = "MyPluggableAI_V2/Decisions/CheckCoverMoveSkillDecsision", order = 1)]
public class CheckCoverMoveSkillDecsision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckCoverMoveSkill(controller);
    }

    private bool CheckCoverMoveSkill(StateController controller)
    {
        int tilesChecked = 0;
        bool result = false;
        int tileScore = 0;
        int prevTileScore = 0;
        int tilesWithoutCover = 0;
        int tilesWithNoActionPointLeftAfterMove = 0;

        List<Tile> reachableTiles = controller.GetReachableTiles();//this already includes the current tile
        
        foreach (var tile in reachableTiles)
        {
            int reaminingActionPointsAfterMove = controller.enemy.remainingActionPoints;
            reaminingActionPointsAfterMove -= controller.pathfinder.GetTilesInBetween(controller.enemy, controller.enemy.characterTile, tile).Count + 1;//this is the cost of moving to the tile
            tilesChecked++;

            if (controller.forcedSkillToUse.skillData == null) // default choosing state, there is no forced skill
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
        
        return result;
    }

    private bool CheckForEachTile(StateController controller, int reaminingActionPointsAfterMove, List<Tile> reachableTiles, Tile tile, int tilesChecked, SkillContainer.Skills skill,
        int tileScore, int prevTileScore, int tilesWithoutCover, bool result )
    {
        //foreach (var skill in controller.skillContainer.skillsList)
        {
            if (skill.skillReadyToUse && reaminingActionPointsAfterMove >= skill.actionPointUse)
            {
                Action oncompleted;
                 foreach (var attackTile in controller.pathfinder.GetAttackableTiles(controller.enemy ,skill, tile,tile, out oncompleted)) // target tile is not set yet but its not important until UseSkillAction
                {
                    //Debug.Log($"{controller.attackableTiles.Count}");
                    if (attackTile.occupiedByPlayer && !attackTile.occupyingPlayer.isDead && attackTile.occupyingPlayer.inCombat)
                    {
                        if (controller.pathfinder.CheckCoverPoint(attackTile, tile, true )) //todo is this reverse?
                        {
                            if (skill.skillData.skillType == SkillsData.SkillType.Melee)
                            {
                                tileScore = 50 + skill.damage;
                            }
                            else
                            {
                             // Debug.Log($"enemy is in cover");
                                tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverDamageDebuff(tile, attackTile, skill) /*- controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill)*/;
                            }
                            //Debug.Log($"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                            if ( tileScore > prevTileScore)
                            {
                                controller.decidedAttackSkill = skill;
                                //Debug.Log($"decidedAttackSkill = {skill.skillData.name}");
                                controller.decidedMoveTile = tile;
                                controller.targetPlayerTile = controller.forcedTargetPlayerTile == null ? attackTile : controller.forcedTargetPlayerTile; //if forced target is set, use it, otherwise use the attack tile
                                // Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                result = true; //this means cover found 
                                prevTileScore = tileScore;
                            }
                            
                            if (tilesChecked >= reachableTiles.Count)
                            {
                                //Debug.Log($"cover= {result} tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                return result;
                            }
                        }
                        else
                        {
                            tilesWithoutCover++;
                            if (tilesWithoutCover >= reachableTiles.Count)
                            {
                               // Debug.Log("cover = "+ result + " no cover found, proceeding to move close");
                                return false;
                            }
                        }
                        
                        
                    }
                }
            }
        }

        return result;
    }
}