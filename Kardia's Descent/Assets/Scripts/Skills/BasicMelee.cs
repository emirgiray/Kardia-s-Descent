using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMelee", menuName = "ScriptableObjects/Skills/BasicMelee", order = 0)]
public class BasicMelee : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null) //skill logic goes here
    {
        ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
            .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
    }

    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        //Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        //Debug.Log($"Wait finished");
        
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        if (skillStartVFX != null) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand, selectedTile.transform.position);
        
        if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
        {
            base.DoDamage(Skill, ActivaterCharacter, selectedTile, 1, OnComplete); 
            
            switch(skillEffect)
            {
                case SkillEffect.None:
                    break;
                case SkillEffect.Stun:
                    base.DoStun(Skill, ActivaterCharacter, selectedTile);
                    break;
            }
        }
        else
        {
            base.OnMiss(Skill, ActivaterCharacter, selectedTile, OnComplete);
        }

        //if the current anim name contains idle, then invoke oncomplete
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
 
        OnComplete?.Invoke();
    }
}