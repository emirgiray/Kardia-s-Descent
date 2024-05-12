using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMelee", menuName = "ScriptableObjects/Skills/BasicMelee", order = 0)]
public class BasicMelee : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null) //skill logic goes here
    {
        if (skillStartVFX == null)
        {
            ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
                .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
        }
        else //if there is a start vfx
        {
            ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
                .StartCoroutine(SkillStartVFXDelay(Skill, ActivaterCharacter, selectedTile, OnComplete));
        }
        
        /*ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
            .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));*/
    }
    
    private IEnumerator SkillStartVFXDelay(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        if (skillStartVFX != null)
        {
            // spawn the vfx
            yield return new WaitForSeconds(0.5f);
            GameObject temp = skillStartVFX.SpawnVFXWithReturn(ActivaterCharacter.Hand);
            temp.TryGetComponent(out ProjectileMove projectileMove);
            yield return new WaitForSeconds(0.1f);
            
            // activate the parabola and on its end activate the skill
            projectileMove.parabolaController.OnParabolaEnd.AddListener(() =>
            {
                ActivaterCharacter.SkillContainer.SetImpact(true);
                ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
                    .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
            });
            projectileMove.SetAndStartParabolaStraight(selectedTile.transform.position + new Vector3(0, 2, 0));
        }
    }

    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        //Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        //Debug.Log($"Wait finished");
        
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        if (skillStartVFX != null) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand);
        
        if (skillTargetType == SkillTargetType.Cleave)
        {
            List<Tile> effectedTiles = ActivaterCharacter.SkillContainer.effectedTiles;
            foreach (var tile in effectedTiles)
            {
                if (base.TryHit(Skill, ActivaterCharacter, tile, OnComplete))
                {
                    base.DoDamage(Skill, ActivaterCharacter, tile, ActivaterCharacter.SkillContainer.coverDamageMultiplier * ActivaterCharacter.SkillContainer.otherDamageMultiplier, OnComplete);
                    switch (skillEffect)
                    {
                        case SkillEffect.None:
                            break;
                        case SkillEffect.Stun:
                            base.DoStun(Skill, ActivaterCharacter, tile);
                            break;
                    }
                }
                else
                {
                    base.OnMiss(Skill, ActivaterCharacter, tile, OnComplete);
                }
            }
        }
        else
        {
            if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
            {
                base.DoDamage(Skill, ActivaterCharacter, selectedTile, ActivaterCharacter.SkillContainer.coverDamageMultiplier * ActivaterCharacter.SkillContainer.otherDamageMultiplier, OnComplete); 
            
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
        }
        
       

        //if the current anim name contains idle, then invoke oncomplete
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
 
        OnComplete?.Invoke();
    }
}