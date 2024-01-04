using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckCurrentCoverDecision", menuName = "MyPluggableAI_V2/Decisions/CheckCurrentCoverDecision", order = 3)]
public class CheckCurrentCoverDecision : DecisionAI
{
    public override bool Decide(StateController controller)
    {
        return CheckCurrentCover(controller);
    }

    private bool CheckCurrentCover(StateController controller)
    {
        int inCover = 0;
        int playersChecked = 0;
        foreach (var player in controller.players)
        {
            if (Pathfinder.Instance.CheckCoverPoint(player.characterTile, controller.enemy.characterTile, true))
            {
                playersChecked++;
                inCover++;
                //Debug.Log($"players checked: {playersChecked}, in cover: {inCover}");
                
                if (playersChecked >= controller.players.Count)
                {
                    if (inCover >= controller.players.Count / 2)
                    {
                       // Debug.Log($"in cover: true");
                        return true;
                    }
                }

            }

        }
        //Debug.Log($"in cover: false");
        return false;

    }
}
