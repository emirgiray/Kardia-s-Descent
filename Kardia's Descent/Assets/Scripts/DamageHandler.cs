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
    
    public void TakeDamage(int value, Character attacker)
    {
        sgtHealth.HealthDecrease(value);
        character.OnCharacterRecieveDamageFunc(value);
        
        if (attacker is Player)
        {
            character.everythingUseful.GameManager.totalDamageDealt += value;
        }
        if (attacker is Enemy)
        {
            character.everythingUseful.GameManager.totalDamageTaken += value;
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
        }
    }

}
