using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/LineHitAll", order = 2)]

public class LineHitAll : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null)
    {
        everythingUseful.Interact.GetComponent<MonoBehaviour>().StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, multipliars, OnComplete));
    }

    private IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null)
    {
       
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        if (skillStartVFX != null && !useParabolla) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand);

        foreach (var tile in ActivaterCharacter.SkillContainer.effectedTiles)
        {
            if (tile == ActivaterCharacter.characterTile && ActivaterCharacter.allCharacterTiles.Contains(tile)) continue;
            if (base.TryHit(Skill, ActivaterCharacter, tile, OnComplete))
            {
                base.DoDamage(Skill, ActivaterCharacter, tile, multipliars, OnComplete);
                //base.DoKnockBack(Skill, ActivaterCharacter, tile, OnComplete);
            }
            else
            {
                base.OnMiss(Skill, ActivaterCharacter, tile, OnComplete);
            }
        }
        
        OnComplete?.Invoke();
    }
    
}
