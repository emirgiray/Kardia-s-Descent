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

        foreach (var tile in controller.GetReachableTiles())
        {
            int reaminingActionPointsAfterMove = controller.enemy.remainingActionPoints;
            reaminingActionPointsAfterMove -= Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, tile).Count;
            tilesChecked++;
            //if (/*tile.isCoveredByCoverPoint*/ Pathfinder.Instance.CheckCoverPoint())
            {
                foreach (var skill in controller.skillContainer.skillsList)
                {
                    if (skill.skillReadyToUse && reaminingActionPointsAfterMove >= skill.actionPointUse)
                    {
                        //Debug.Log($"{skill.skillData}");
                        //Debug.Log($"skill ready to use and enough action points after move");
                        foreach (var attackTile in Pathfinder.Instance.GetAttackableTiles(tile, skill.skillData.skillRange))
                        {
                            if (attackTile.occupiedByPlayer)
                            {
                                if (Pathfinder.Instance.CheckCoverPoint(attackTile, tile ))
                                {
                                    Debug.Log($"enemy is in cover");
                                    tileScore = skill.damage - controller.skillContainer.CalculateCoverAccuracyDebuff(attackTile.occupyingPlayer);
                                    Debug.Log($"tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {controller.GetReachableTiles().Count}");

                                    if ( tileScore > prevTileScore)
                                    {
                                        /*controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.enemy.StartMove(Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile));*/
                                        //controller.skillContainer.SelectSkill(skill, controller.enemy);
                                        controller.decidedAttackSkill = skill;
                                        controller.decidedMoveTile = tile;
                                        controller.targetPlayer = attackTile.occupyingPlayer;
                                        Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore} skill: {skill.skillData}, tiles checked: {tilesChecked} / {controller.GetReachableTiles().Count}");
                                        result = true; //this means cover found 
                                    }
                                    prevTileScore = tileScore;
                                    
                                    if (tilesChecked >= controller.GetReachableTiles().Count)
                                    {
                                        Debug.Log($"cover= {result} tiles checked: {tilesChecked} / {controller.GetReachableTiles().Count}");
                                        return result;
                                    }
                                }
                                else
                                {
                                    tilesWithoutCover++;
                                    if (tilesWithoutCover >= controller.GetReachableTiles().Count)
                                    {
                                        Debug.Log("cover = "+ result + " no cover found, proceeding to move close");
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
        
        
        Debug.Log($"RETURNED AT THE END, cover : {result},  tiles checked: {tilesChecked} / {controller.GetReachableTiles().Count}");
        return result;
    }
}