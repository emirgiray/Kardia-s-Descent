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
        //reachableTiles.Add(controller.enemy.characterTile);//include current tile
        
        foreach (var tile in /*controller.GetReachableTiles()*/ reachableTiles)
        {
            int reaminingActionPointsAfterMove = controller.enemy.remainingActionPoints;
            reaminingActionPointsAfterMove -= Pathfinder.Instance.GetTilesInBetween(controller.enemy, controller.enemy.characterTile, tile).Count + 1;//this is the cost of moving to the tile
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
                                if (Pathfinder.Instance.CheckCoverPoint(attackTile, tile ))
                                {
                                    if (skill.skillData.skillType == SkillsData.SkillType.Melee)
                                    {
                                        tileScore = 50 + skill.damage;
                                    }
                                    else
                                    {
                                  //      Debug.Log($"enemy is in cover");
                                        tileScore = 50 + skill.damage - controller.skillContainer.CalculateCoverAccuracyDebuff(tile, attackTile, skill);
                                    }
                                    
                              //      Debug.Log($"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");

                                    if ( tileScore > prevTileScore)
                                    {
                                        /*controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.enemy.StartMove(Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile));*/
                                        //controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.decidedAttackSkill = skill;
                                        controller.decidedMoveTile = tile;
                                        controller.targetPlayer = attackTile.occupyingPlayer;
                           //             Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                        result = true; //this means cover found 
                                        prevTileScore = tileScore;
                                    }
                                    
                                    if (tilesChecked >= /*controller.GetReachableTiles().Count*/ reachableTiles.Count)
                                    {
                                        //Debug.Log($"cover= {result} tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
                                        return result;
                                    }
                                }
                                else
                                {
                                    tilesWithoutCover++;
                                    if (tilesWithoutCover >= /*controller.GetReachableTiles().Count*/ reachableTiles.Count)
                                    {
                                       // Debug.Log("cover = "+ result + " no cover found, proceeding to move close");
                                        return false;
                                    }
                                }
                                
                                
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
        
        
       // Debug.Log($"RETURNED AT THE END, cover : {result},  tiles checked: {tilesChecked} / {/*controller.GetReachableTiles().Count*/ reachableTiles.Count}");
        return result;
    }
}