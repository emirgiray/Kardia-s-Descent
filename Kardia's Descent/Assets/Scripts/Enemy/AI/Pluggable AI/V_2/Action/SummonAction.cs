using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonAction", menuName = "MyPluggableAI_V2/Actions/SummonAction", order = 0)]

public class SummonAction : ActionAI
{
   
    public override void Act(StateController controller)
    {
        controller.canExitState = false;
        Action OnCompleteAddedAction;
        controller.pathfinder.GetAttackableTiles(controller.enemy, controller.skillContainer.selectedSkill, controller.enemy.characterTile, controller.targetPlayerTile, out OnCompleteAddedAction);

        List<Tile> AvailableSpawnPoints = new();
        
        foreach (var waypoint in controller.waypoints) // we use waypoints as spawn points for summoned units
        {
            if (!waypoint.Occupied && waypoint.selectable) 
            {
                AvailableSpawnPoints.Add(waypoint);
               // Debug.Log($"expression");
            }
            
        }

        int availableSpawnPointsCount = AvailableSpawnPoints.Count;
        int maxSpawnCount = 6; //todo this will be in the skill data
        int minSpawnCount = 4; //todo this will be in the skill data
        int spawnedCount = 0;
       
        int decidedSpawnCount = UnityEngine.Random.Range(minSpawnCount, maxSpawnCount);
       // controller.enemy.remainingActionPoints += controller.skillContainer.selectedSkill.actionPointUse * decidedSpawnCount;
       int prevAPUse = controller.skillContainer.selectedSkill.actionPointUse;
       controller.skillContainer.selectedSkill.actionPointUse = 0;
        controller.lastUsedSkill = controller.decidedAttackSkill;

        for (int i = 0; i < decidedSpawnCount; i++)
        {
            if (spawnedCount >= availableSpawnPointsCount) break;
            
            Action onComplete = null;
           // Debug.Log($"{spawnedCount < maxSpawnCount - 1}");
            if (spawnedCount < decidedSpawnCount - 1)
            {
                onComplete = () =>
                {
                    if (OnCompleteAddedAction != null) OnCompleteAddedAction();
                    
                };
                /*controller.skillContainer.UseSkill(controller.skillContainer.selectedSkill, controller.targetPlayerTile, controller.enemy, ()=>
                {
                    if (OnCompleteAddedAction != null) OnCompleteAddedAction();
                });*/
            }
            else //last spawn
            {
                onComplete = () =>
                {
                    controller.canExitState = true;
                    if (OnCompleteAddedAction != null) OnCompleteAddedAction();
            
                    //if forced skill is the same as selected skill, then reset forced skill
                    if (controller.forcedSkillToUse == controller.lastUsedSkill)
                    {
                        // controller.forcedSkillToUse.skillData = null;
                        controller.skillForced = false;
                    }
                    //controller.lastUsedSkill.damage += cachedDamageDebuff;
                    controller.forcedTargetPlayerTile = null;
                    
                    controller.enemy.everythingUseful.TurnSystem.DecideEnemyTurnOrder();
                    controller.skillContainer.selectedSkill.actionPointUse = prevAPUse;
                };
                
                /*controller.skillContainer.UseSkill(controller.skillContainer.selectedSkill, controller.targetPlayerTile, controller.enemy, ()=>
                {
                    controller.canExitState = true;
                    if (OnCompleteAddedAction != null) OnCompleteAddedAction();
            
                    //if forced skill is the same as selected skill, then reset forced skill
                    if (controller.forcedSkillToUse == controller.lastUsedSkill)
                    {
                        controller.forcedSkillToUse.skillData = null;
                    }
                    //controller.lastUsedSkill.damage += cachedDamageDebuff;
                    controller.forcedTargetPlayerTile = null;
                });*/
            }
            
            int randomIndex = UnityEngine.Random.Range(0, AvailableSpawnPoints.Count);
           // Debug.Log($"AvailableSpawnPoints.Count: {AvailableSpawnPoints.Count} , {decidedSpawnCount}");
            var targetTile = AvailableSpawnPoints[randomIndex];
            AvailableSpawnPoints.RemoveAt(randomIndex);
            
            controller.skillContainer.UseSkill(controller.skillContainer.selectedSkill, targetTile, controller.enemy, ()=>
            {
                onComplete?.Invoke();
            });
            spawnedCount++;
        }
        
       
        
        
    }
}
