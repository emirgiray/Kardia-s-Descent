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
        int tileScore = 0;
        int prevTileScore = 0;
        bool result = false;

        foreach (var tile in controller.GetReachableTiles())
        {
            if (tile.isCoveredByCoverPoint)
            {
                foreach (var skill in controller.skillContainer.skillsList)
                {
                    foreach (var attackTile in Pathfinder.Instance.GetAttackableTiles(tile, skill.skillData.skillRange))
                    {
                        if (attackTile.occupiedByPlayer)
                        {
                            /*if (Pathfinder.Instance.CheckCoverPoint(controller.enemy, attackTile.occupyingPlayer))
                            {
                                
                            }*/
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        
        return false;
    }
}