using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        character.OnCharacterRecieveDamageFunc();
        
        if (sgtHealth.isDead)
        {
            Dead(attacker);
        }
        else
        {
            if (!character.inCombat)
            {
                character.StartCombat();
            }

            if (!attacker.inCombat)
            {
                attacker.StartCombat();
                if (TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn)
                {
                    TurnSystem.Instance.CombatStarted();
                }
            }
        }


    }

    public void Dead(Character attacker)
    {
        if (attacker is Player)
        {
            attacker.GetComponent<Player>().IncreaseKills();
        }
    }

}
