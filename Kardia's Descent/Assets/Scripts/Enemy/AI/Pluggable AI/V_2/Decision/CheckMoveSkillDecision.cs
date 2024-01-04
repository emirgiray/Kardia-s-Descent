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

    private bool CheckMoveSkill(StateController controller)
    {
        int tilesChecked = 0;
        bool result = false;
        int tileScore = 0;
        int prevTileScore = 0;
        int tilesWithoutCover = 0;
        int tilesWithNoActionPointLeftAfterMove = 0;

        List<Tile> reachableTiles = controller.GetReachableTiles();
        //reachableTiles.Add(controller.enemy.characterTile);//include current tile
        
        foreach (var tile in /*controller.GetReachableTiles()*/ reachableTiles)
        {
            int reaminingActionPointsAfterMove = controller.enemy.remainingActionPoints;
            reaminingActionPointsAfterMove -= Pathfinder.Instance.GetTilesInBetween(controller.enemy, controller.enemy.characterTile, tile).Count + 1;
            tilesChecked++;
            //if (/*tile.isCoveredByCoverPoint*/ Pathfinder.Instance.CheckCoverPoint())
            {
                foreach (var skill in controller.skillContainer.skillsList)
                {
                    if (skill.skillReadyToUse && reaminingActionPointsAfterMove >= skill.actionPointUse)
                    {
                        //Debug.Log($"{skill.skillData}");
                        //Debug.Log($"skill ready to use and enough action points after move");
                        foreach (var attackTile in Pathfinder.Instance.GetAttackableTiles(tile, skill))
                        {
                            if (attackTile.occupiedByPlayer && !attackTile.occupyingPlayer.isDead)
                            {
                                //if (Pathfinder.Instance.CheckCoverPoint(attackTile, tile ))
                                {
                                    //Debug.Log($"enemy is in cover");
                                    tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverDamageDebuff(tile, attackTile, skill) - controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill)
                                        + Pathfinder.Instance.GetTilesInBetween(controller.enemy, tile, attackTile, true).Count * 10;

                                   // Debug.Log($"remaining ap after move: {reaminingActionPointsAfterMove}");
                                   // Debug.Log($"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, " +
                                    //          $"tiles between {Pathfinder.Instance.GetTilesInBetween(controller.enemy, tile, attackTile, true).Count}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");

                                    if ( tileScore > prevTileScore)
                                    {
                                        /*controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.enemy.StartMove(Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile));*/
                                        //controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.decidedAttackSkill = skill;
                                        controller.decidedMoveTile = tile;
                                        controller.targetPlayer = attackTile.occupyingPlayer;
                                      //  Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                        result = true; //this means move found 
                                        
                                        prevTileScore = tileScore;
                                    }
                                    
                                    if (tilesChecked >= /*controller.GetReachableTiles().Count*/ reachableTiles.Count)
                                    {
                                       // Debug.Log($"move= {result} tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                        return result;
                                    }
                                }
                                /*else
                                {
                                    tilesWithoutCover++;
                                    if (tilesWithoutCover >= /*controller.GetReachableTiles().Count#1# reachableTiles.Count)
                                    {
                                        Debug.Log("cover = "+ result + " no cover found, proceeding to move close");
                                        return false;
                                    }
                                }*/
                                
                                
                            }
                        }
                    }
                }
            }
            /*else
            {
                tilesWithoutCover++;
                if (tilesWithoutCover >= controller.GetReachableTiles().Count)
                {
                    return false;
                }
            }*/
        }
        
        
     //   Debug.Log($"RETURNED AT THE END, move : {result},  tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
        return result;
    }
}

