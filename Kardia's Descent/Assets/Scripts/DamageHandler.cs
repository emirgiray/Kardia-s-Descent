using System.Collections;
using System.Collections.Generic;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private SGT_Health sgtHealth;
    [SerializeField] private Character character;
    /*[DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public UnityEvent DeadEvent;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public UnityEvent TakeDamageEvent;*/
    
    public void TakeDamage(int baseValue, float damageMultipliar, Character attacker)
    {
        int totalDamage = Mathf.RoundToInt(baseValue * damageMultipliar);
        int extraDamage = totalDamage - baseValue;
        
        if (extraDamage > 0)
        {
            // this means the damage was increased
        }
        else if (extraDamage < 0)
        {
            // this means the damage was decreased
            
        }
        else
        {
            // this means the damage was not increased or decreased
        }
        
        character.everythingUseful.SpawnText(totalDamage, extraDamage, Color.red,Color.white ,character.Head, 3, 2f, character.health);
        
        sgtHealth.HealthDecrease(totalDamage);
        character.OnCharacterRecieveDamageFunc(totalDamage);
        
        if (attacker is Player)
        {
            character.everythingUseful.GameManager.totalDamageDealt += totalDamage;
            
        }
        if (attacker is Enemy)
        {
            character.everythingUseful.GameManager.totalDamageTaken += totalDamage;
        }
        
        if (sgtHealth.isDead)
        {
            Dead(attacker);
        }
        else
        {
            if (!character.inCombat)
            {
                character.StartCombat(attacker);
            }

            if (!attacker.inCombat)
            {
                attacker.StartCombat(attacker);
                if (character.TurnSystem.turnState == TurnSystem.TurnState.FreeRoamTurn)
                {
                    character.TurnSystem.CombatStarted();
                }
            }
        }


    }

    public void Dead(Character attacker)
    {
        if (attacker is Player)
        {
            attacker.GetComponent<Player>().IncreaseKills();
            character.everythingUseful.GameManager.totalKills++;
            attacker.OnKill();
        }
    }

}
