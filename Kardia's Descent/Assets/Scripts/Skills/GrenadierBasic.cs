using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadierBasic", menuName = "ScriptableObjects/Skills/GrenadierBasic", order = 0)]
public class GrenadierBasic : SkillsData
{
    
     public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null) //skill logic goes here
    {
        ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
            .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
    }

    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        if (skillStartVFX != null) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand, selectedTile.transform.position);
        
        if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
        {
            List<Tile> effectedTiles = ActivaterCharacter.SkillContainer.effectedTiles;
            List<Tile> innerEffectedTiles = ActivaterCharacter.SkillContainer.innerEffectedTiles;
            List<Tile> outerEffectedTiles = ActivaterCharacter.SkillContainer.outerEffectedTiles;
            
            
            if (skillTargetType == SkillTargetType.AreaAroundTarget)
            {
                foreach (var tile in effectedTiles)
                {
                    float multipliar = 1;
                    if (innerEffectedTiles.Contains(tile))
                    {
                        multipliar = 1;
                    }
                    else if (outerEffectedTiles.Contains(tile))
                    {
                        multipliar = 0.5f;
                    }
                    
                    base.DoDamage(Skill, ActivaterCharacter, tile, multipliar, OnComplete); 
                }
                
            }
            else if (skillTargetType == SkillTargetType.AreaAroundSelf)
            {
                foreach (var tile in effectedTiles)
                {
                    float multipliar = 1;
                    if (innerEffectedTiles.Contains(tile))
                    {
                        multipliar = 1;
                    }
                    else if (outerEffectedTiles.Contains(tile))
                    {
                        multipliar = 0.5f;
                    }
                    
                    base.DoDamage(Skill, ActivaterCharacter, tile, multipliar, OnComplete); 
                }
            }
            
            
            
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
        
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
        
        OnComplete?.Invoke();
    }
}
