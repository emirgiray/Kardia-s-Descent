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
        
        if (controller.enemy.canMove && controller.RandomWait2())
        {
            controller.canExitState = false;
            //controller.enemy.StartMove(FindRandomPath(controller), true, ()=>  controller.canExitState = true, false);
            controller.enemy.StartMove(FindPathBetweenRandomWaypoints(controller), true, ()=>  controller.canExitState = true, false);
            
        }
    }
    
    private Path FindRandomPath(StateController controller)
    {
        List<Tile>  reachableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.enemy.remainingActionPoints);
        Tile randomTile = reachableTiles[Random.Range(0, reachableTiles.Count)];
        return Pathfinder.Instance.FindPath(controller.enemy, controller.enemy.characterTile, randomTile);
    }

    public Path FindPathBetweenRandomWaypoints(StateController controller)
    {
        Tile randomTile = controller.waypoints[Random.Range(0, controller.waypoints.Count)].GetComponent<Waypoints>().waypointTile;
        controller.decidedMoveTile = randomTile;
        return Pathfinder.Instance.FindPath(controller.enemy, controller.enemy.characterTile, randomTile);
    }
}
