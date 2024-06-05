using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllEnemyStats", menuName = "ScriptableObjects/AllEnemy Stats", order = 0)]
public class AllEnemyStats : ScriptableObject
{
    public List<CharacterStats> allEnemyStats = new();

    public void IncreaseStats(int value)
    {
        foreach (var var in allEnemyStats)
        {
            var.AddStrength(value);
            var.AddDexterity(value);
            var.AddConstitution(value);
            var.AddAiming(value);
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
