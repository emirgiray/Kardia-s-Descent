using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "MyPluggableAI/Actions/PatrolAction", order = 0)]
public class PatrolAction : ActionAI
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        
        
        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove && controller.RandomWait())
        {
            if (controller.enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder)
            {
                controller.enemy.StartMove(FindRandomPath(controller));
                controller.enemy.canMove = false;
                //controller.enemy.remainingMoveRange = 0;
            }
        }
        
         
        
        
        
        /*controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
        controller.navMeshAgent.isStopped = false;

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
        {
            controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
        }*/
    }

    List<Tile> reachableTiles = new List<Tile>();
    
    private Path FindRandomPath(StateController controller)
    {
        reachableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.enemy.remainingActionPoints);
        Tile randomTile = reachableTiles[Random.Range(0, reachableTiles.Count)];
        return Pathfinder.Instance.FindPath(controller.enemy, controller.enemy.characterTile, randomTile);
    }
}
