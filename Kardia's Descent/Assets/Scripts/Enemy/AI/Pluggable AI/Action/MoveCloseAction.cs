using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveCloseAction", menuName = "MyPluggableAI/Actions/MoveCloseAction", order = 0)]
public class MoveCloseAction : ActionAI
{
    public override void Act(StateController controller)
    {
        Move(controller);
    }
 bool doonce = true;
    private void Move(StateController controller)
    {
        
        
        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && controller.enemy.canMove/* && controller.RandomWait()*/)//todo seems like adding delay fixes problems like stuttering and going back to first tile
        {
            if (controller.enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder && doonce)
            {
               // controller.enemy.StartMove(Pathfinder.Instance.GetPathBetween(controller.enemy.characterTile, controller.targetPlayer.characterTile, true));// 1
                 controller.enemy.StartMove(FindPathToTargetPlayer(controller, controller.targetPlayer), () => doonce = true);
                 doonce = false;
               // controller.enemy.canMove = false;
                //controller.enemy.remainingMoveRange = 0;
            }
        }
    }

    // List<Tile> reachableTiles = new List<Tile>();
    // List<Tile> attackableTiles = new List<Tile>();
    // List<Tile> tilesBetwen = new List<Tile>();
    
    private Path FindPathToTargetPlayer(StateController controller, Player targetPlayer)
    {
        // reachableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.enemy.remainingMoveRange);
        // attackableTiles = Pathfinder.Instance.GetReachableTiles(controller.enemy.characterTile, controller.selectedSkill.skillRange);
        //tilesBetwen = Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, targetPlayer.characterTile, true);
        //move until target player is in skillRange

         //return Pathfinder.Instance.FindPath(controller.enemy.characterTile, Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, targetPlayer.characterTile, true)[controller.enemy.remainingMoveRange - 1]);// 1
        //return Pathfinder.Instance.GetPathBetween(controller.enemy.characterTile, targetPlayer.characterTile);

      // return Pathfinder.Instance.PathBetween(targetPlayer.characterTile, controller.enemy.characterTile);
      //make a path between enemy and player using the tiles in between

      /*foreach (var tile in Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, targetPlayer.characterTile, true))
      {
          Path path = Pathfinder.Instance.PathBetween(targetPlayer.characterTile, tile);
          return  path;
      }
      return  null;*/
      /*return Pathfinder.Instance.FindPath(controller.enemy.characterTile,
          Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, 
              targetPlayer.characterTile, true)[Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, 
              targetPlayer.characterTile, true).Count - 1]);*///this gets the tiles between, then gets the last tile in the list, which is the tile that is closest to the player
        
      
      return Pathfinder.Instance. GetPathBetween(controller.enemy.characterTile, 
          controller.decidedMoveTile, true);
        /*foreach (var tile in attackableTiles)//maybe do this in a decision
        {
            if (tile == targetPlayer.characterTile)
            {
                return null;
            }

        }
        */
        
        /*foreach (var tile in reachableTiles)
        {
            if (tile == targetPlayer.characterTile)
            {
                return Pathfinder.Instance.FindPath(controller.enemy.characterTile, targetPlayer.characterTile);
            }
        }*/
        
        /*foreach (var tile in tilesBetwen)
        {
            foreach (var reachableTile in reachableTiles)
            {
                if (tile == reachableTile)
                {
                    return Pathfinder.Instance.FindPath(controller.enemy.characterTile, tile);
                }
            }
        }*/
        
        // return null;
        
    }
}
