using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/Enemy/EnemyStats", order = 0)]
public class EnemyStatsData : ScriptableObject
{
    public int moveRange = 3;
    public int RemainingmoveRange = 3;
    public int moveRangeModifier = 0;
    
    public int attackRange = 1;
    public int attackRangeModifier = 0;
    
    public int initiative = 1;
    public int initiativeModifier = 0;

    [Button, GUIColor(1,0,0)]
    public void ResetModifiers()
    {
        moveRangeModifier = 0;
        attackRangeModifier = 0;
        initiativeModifier = 0;
    }
}
