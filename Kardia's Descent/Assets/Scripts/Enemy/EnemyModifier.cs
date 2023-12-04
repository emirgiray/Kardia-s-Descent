using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyModifier : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private SkillContainer skillContainer;
    
    [SerializeField] private SGT_Health health;
    
    [SerializeField] private int healthThreshold = 10;
    [SerializeField] private float damageModifier = 2;
    [SerializeField] private AnimatorOverrideController overrideController;
    
    public UnityEvent onHealthThresholdReached;
    private bool doOnce = true; 
    public void TryIncreaseDamage()
    {
        if (health._Health <= healthThreshold)
        {
            if (doOnce)
            {
                doOnce = false;
                skillContainer.SetAnimAtorOverrides(overrideController);
                onHealthThresholdReached.Invoke();
            
                for (int i = 0; i < skillContainer.skillsList.Count; i++)
                {
                    /*if (skillContainer.skillsList[i].skillData.skillClass)*/
                    {
                        float damage = skillContainer.skillsList[i].damage;
                        damage *= damageModifier;
                        skillContainer.skillsList[i].damage = Mathf.CeilToInt(damage);
                    }
                }
                
                
            }
        }

    }

}
