using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckMoveToCoverDecision", menuName = "MyPluggableAI_V2/Decisions/CheckMoveToCoverDecision", order = 5)]
public class CheckMoveToCoverDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckMoveToCover(controller);
    }

    private bool CheckMoveToCover(StateController controller)
    {
        int tilesInCover = 0;
        int tilesChecked = 0;
        bool result = false;
        int tileScore = 0;
        int prevTileScore = 0;
        
        List<Tile> reachableTiles = controller.GetReachableTiles();//this already includes the current tile
        
        foreach (var tile in reachableTiles)
        {
            tilesChecked++;
            int inCover = 0;
            int playersChecked = 0;
            
            foreach (var player in controller.players)
            {
                playersChecked++;
                
                if (Pathfinder.Instance.CheckCoverPoint(player.characterTile, tile))
                {
                    inCover++;
                    
                    /*Debug.Log($"players checked: {playersChecked}, in cover: {inCover}");
                
                    if (playersChecked >= controller.players.Count)
                    {
                        if (inCover >= controller.players.Count / 2)
                        {
                            Debug.Log($"in cover: true");
                            return true;
                        }
                    }*/

                }
                
                tileScore = 100 + (inCover * 10 ) - Pathfinder.Instance.GetTilesInBetween(tile, player.characterTile, true).Count * 2;
                if (inCover == 0)
                {
                    tileScore = -1000;
                }
                if ( tileScore > prevTileScore)
                {
                    result = true; //this means cover found    
                    Debug.Log($"BIGGER THAN PREVIOUS, tile: {tile}, curent score: {tileScore}, prev score: {prevTileScore}");
                    controller.decidedMoveTile = tile;
                    prevTileScore = tileScore;
                }


            }
        }

        if (result == false)//if no cover found, then select the closest player to move close to it
        {
            Player closestPlayer = null;
            int characterScore = 0;
            int prevCharacterScore = 10000;

            foreach (var player in controller.players)  
            {
                characterScore = Pathfinder.Instance.GetTilesInBetween(controller.enemy.characterTile, player.characterTile, true).Count;
                if (characterScore < prevCharacterScore)
                {
                    controller.decidedMoveTile = player.characterTile;
                    prevCharacterScore = characterScore;
                }

            }
        }


        Debug.Log($"result: {result} tiles checked: {tilesChecked} / {reachableTiles.Count}");
        return result;

    }
}

