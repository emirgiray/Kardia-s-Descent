using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectTargetPlayerAction", menuName = "MyPluggableAI/Actions/SelectTargetPlayerAction", order = 0)]
public class SelectTargetPlayerAction : ActionAI
{
    public override void Act(StateController controller)
    {
        controller.targetPlayer = SelectClosestPlayer(controller);
    }

    public Player SelectClosestPlayer(StateController controller)
    {
        //select the player on the reachable tile
        foreach (var tile in controller.GetReachableTiles())
        {
            if (tile.occupyingPlayer)
            {
                return tile.occupyingPlayer;
            }
        }
        
        
        //select the closest player //todo maybe do this another way
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

        return closestPlayer;
    }

}
