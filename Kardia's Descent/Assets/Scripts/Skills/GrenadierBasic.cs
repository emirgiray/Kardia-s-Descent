using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadierBasic", menuName = "ScriptableObjects/Skills/GrenadierBasic", order = 0)]
public class GrenadierBasic : SkillsData
{
    public int impactRange = 2;
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
            if (skillTargetType == SkillTargetType.AreaAroundTarget)
            {
                List<Tile> tiles = ActivaterCharacter.pathfinder.GetNeighbouringTiles(selectedTile, impactRange);
                foreach (var tile in tiles)
                {
                    base.DoDamage(Skill, ActivaterCharacter, tile, OnComplete); 
                }
                
            }
            else if (skillTargetType == SkillTargetType.AreaAroundSelf)
            {
                List<Tile> tiles = ActivaterCharacter.pathfinder.GetNeighbouringTiles(ActivaterCharacter.characterTile, impactRange);
                foreach (var tile in tiles)
                {
                    base.DoDamage(Skill, ActivaterCharacter, tile, OnComplete); 
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
