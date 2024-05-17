using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegularPassiveSkill", menuName = "ScriptableObjects/Skills/PassiveSkills/RegularPassiveSkill", order = 1)]
public class RegularPassiveSkill : SkillsData
{
    private int fireCount = 0;
    
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null) //skill logic goes here
    {
        fireCount = 0;
        
        MonoBehaviour mono = ActivaterCharacter.Interact.GetComponent<MonoBehaviour>();
            
        mono.StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, multipliars, OnComplete));
        
    }
    
    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null)
    {
        //Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        //Debug.Log($"Wait finished");
        
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);

        while (fireCount < 3)
        {
            Vector3 dir = selectedTile.transform.position - ActivaterCharacter.transform.position;
            if (skillStartVFX != null) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand, dir);
            if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
            {
                base.DoDamage(Skill, ActivaterCharacter, selectedTile, multipliars, OnComplete); 
            }
            else
            {
                base.OnMiss(Skill, ActivaterCharacter, selectedTile, OnComplete);
            }
            
            fireCount++;

            yield return new WaitForSeconds(0.33f);
        }
        
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
        
        OnComplete?.Invoke();
    }
}
