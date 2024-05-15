using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadierBasic", menuName = "ScriptableObjects/Skills/GrenadierBasic", order = 0)]
public class GrenadierBasic : SkillsData
{
    [SerializeField] private float bombDelay = 0.5f;
     public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null) //skill logic goes here
    {
        
        if (!useParabolla)
        {
            ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
                .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
        }
        else //if there is a start vfx
        {
            ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
                .StartCoroutine(SkillStartVFXDelay(Skill, ActivaterCharacter, selectedTile, OnComplete));
        }
    }

    private IEnumerator SkillStartVFXDelay(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        //if (skillStartVFX != null)
        {
            // spawn the vfx
            yield return new WaitForSeconds(bombDelay);
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
            projectileMove.SetAndStartParabolaYOffset(selectedTile.transform);
        }
    }

    private IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        if (skillStartVFX != null && !useParabolla) skillStartVFX.SpawnVFX(ActivaterCharacter.Hand);
        
        List<Tile> effectedTiles = ActivaterCharacter.SkillContainer.effectedTiles;
        List<Tile> innerEffectedTiles = ActivaterCharacter.SkillContainer.innerEffectedTiles;
        List<Tile> outerEffectedTiles = ActivaterCharacter.SkillContainer.outerEffectedTiles;
        
       // if (skillTargetType == SkillTargetType.AreaAroundTarget || skillTargetType == SkillTargetType.AreaAroundSelf)
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
                
                if (base.TryHit(Skill, ActivaterCharacter, tile, OnComplete))
                {
                    base.DoDamage(Skill, ActivaterCharacter, tile, multipliar * ActivaterCharacter.SkillContainer.coverDamageMultiplier * ActivaterCharacter.SkillContainer.otherDamageMultiplier, OnComplete);

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
        
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
        
        OnComplete?.Invoke();
    }
}
