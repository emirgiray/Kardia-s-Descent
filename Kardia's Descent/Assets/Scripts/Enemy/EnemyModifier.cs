using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class EnemyModifier : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private SkillContainer skillContainer;
    [SerializeField] private SGT_Health health;
    
    [FoldoutGroup("When Health Gets To X")] [SerializeField] private int healthThreshold = 10;
    [FoldoutGroup("When Health Gets To X")] [SerializeField] private float damageModifier = 2;
    [FoldoutGroup("When Health Gets To X")] [SerializeField] private AnimatorOverrideController overrideController;
    [FoldoutGroup("When Health Gets To X")] public UnityEvent onHealthThresholdReached;
    
    [FoldoutGroup("On Death")] [SerializeField] private Transform objectToBeMoved;
    //[FoldoutGroup("On Death")] [SerializeField] private Transform transformToMoveTo;
    [FoldoutGroup("On Death")] [SerializeField] private Vector3 directionToMoveTo;
    [FoldoutGroup("On Death")] [SerializeField] private float moveTime = 1;
    [FoldoutGroup("On Death")] [SerializeField] private float delay = 1;
    [FoldoutGroup("On Death")] [SerializeField] private Ease ease = Ease.Linear;
    Tween tween;
    
    private bool doOnce = true; 
    public void TryIncreaseDamage()
    {
        if (health._Health <= healthThreshold && health._Health > 0)
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

    public void MoveOnDeath()
    {
        if (objectToBeMoved != null)
        {
            //objectToBeMoved.transform.Translate(directionToMoveTo);
            objectToBeMoved.transform.DOLocalMove(directionToMoveTo, moveTime).SetDelay(delay).SetEase(ease).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        else
        {
            transform.DOLocalMove(directionToMoveTo, moveTime).SetDelay(delay).SetEase(ease).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        

    }

}
