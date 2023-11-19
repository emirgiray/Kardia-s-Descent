using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatStartDecision", menuName = "MyPluggableAI/Decisions/CombatStartDecision", order = 0)]
public class CombatStartDecision : DecisionAI
{
    [Multiline] [SerializeField] public string DeveloperDescription = "CANT BE USED!!!!!";

    public override bool Decide(StateController controller)
    {
        return DecideBestTileAndSkill(controller);
    }

    private bool DecideBestTileAndSkill(StateController controller)//todo these are all different states, make them into different decisions :)
    {
        int tileScore = 0;
        int prevTileScore = 0;
        bool result = false; //if this method returns false, then enemy's best tile is the current tile
        
        
        if (result == false)
        {
            foreach (var skill in controller.skillContainer.skillsList)//state 1
            {
                foreach (var attackTile in Pathfinder.Instance.GetAttackableTiles(controller.enemy.characterTile, skill))
                {
                    if (attackTile.occupyingPlayer != null) //todo null check can be too expensive
                    {
                        tileScore = skill.skillData.skillDamage;
                        if (tileScore > prevTileScore)
                        {
                            /*controller.skillContainer.SelectSkill(skill, controller.enemy);
                            controller.enemy.StartMove(Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile));*/
                            controller.skillContainer.SelectSkill(skill, controller.enemy);// this can be added at the end
                            controller.decidedAttackSkill = skill;
                            controller.decidedMoveTile = controller.enemy.characterTile;
                            controller.targetPlayer = attackTile.occupyingPlayer;
                        }

                        prevTileScore = tileScore;


                    }

                    if (attackTile.occupyingPlayer != null)
                    {
                        result = false;
                        Debug.Log( "move = "+ result + " player in range of skill");
                        return result;
                    }
                    
                }
            }

        }
        
        foreach (var tile in controller.GetReachableTiles())//state 2
        {
            foreach (var skill in controller.skillContainer.skillsList)
            {
                foreach (var attackTile in Pathfinder.Instance.GetAttackableTiles(tile, skill))
                {
                    if (attackTile.occupyingPlayer != null)//todo null check can be too expensive
                    {
                        tileScore = Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, tile, true).Count
                            + skill.skillData.skillDamage;
                        if ( tileScore > prevTileScore)
                        {
                            /*controller.skillContainer.SelectSkill(skill, controller.enemy);
                            controller.enemy.StartMove(Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile));*/
                            controller.skillContainer.SelectSkill(skill, controller.enemy);
                            controller.decidedAttackSkill = skill;
                            controller.decidedMoveTile = tile;
                            controller.targetPlayer = attackTile.occupyingPlayer;
                            Debug.Log(tile + " cuurent score: " + tileScore + " prev score: " + prevTileScore);
                            result = true; //this means move 
                        }
                        prevTileScore = tileScore;
                        
                        
                    }
                    /*else//state 3
                    {
                        //select the closest player
                        Player closestPlayer = null;
                        float smallestDistance = Mathf.Infinity;
                        foreach (Player player in controller.players)
                        {
                            float distance = Vector3.Distance(controller.transform.position, player.transform.position);
                            if (distance < smallestDistance)
                            {
                                smallestDistance = distance;
                                closestPlayer = player;
                            }
                        }
                        
                        controller.targetPlayer = closestPlayer;
                        controller.decidedMoveTile = closestPlayer.characterTile;
                        
                        result = true; //this means move
                    }*/
                }
                
            }
            
        }

        
        
        Debug.Log("move = "+ result + " player NOT in range of skill, proceeding to move");
        return result;
    }
}