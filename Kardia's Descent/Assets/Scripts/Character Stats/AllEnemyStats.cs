using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllEnemyStats", menuName = "ScriptableObjects/AllEnemy Stats", order = 0)]
public class AllEnemyStats : ScriptableObject
{
    public List<CharacterStats> allEnemyStats = new();

    public void IncreaseStats()
    {
        foreach (var var in allEnemyStats)
        {
            var.AddStrength(1);
            var.AddDexterity(1);
            var.AddConstitution(1);
            var.AddAiming(1);
        }
    }

    public void ResetToDefault()
    {
        foreach (var var in allEnemyStats)
        {
            var.ResetValues();
        }
    }
}
