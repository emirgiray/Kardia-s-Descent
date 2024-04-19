using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "ScriptableObjects/Skills/BuffSkill", order = 3)]
public class BuffSkill : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
{
    bool isBuff = base.skillEffect == SkillEffect.Buff; //if not buff, then it's debuff

    if (ActivaterCharacter is Player)
    {
        if (buffDebuffType == BuffDebuffType.AP)
        {
            if (skillTarget == SkillTarget.Self)
            {
                // Handle buffing AP to self
                ChangeAP(ActivaterCharacter, isBuff, Skill.skillBuffDebuffAmount, permaBuff);
                foreach (var fx in skillHitVFX)
                {
                    fx.SpawnVFX(ActivaterCharacter.characterTile.transform);
                    if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
                }
            }
            else if (skillTarget == SkillTarget.MultipleAllies)
            {
                // Handle buffing AP to multiple allies
                ActivaterCharacter.pathfinder.GetAttackableTilesDefault(ActivaterCharacter.characterTile, Skill).ForEach(tile =>
                {
                    if (tile.occupiedByPlayer)
                    {
                        ChangeAP(tile.occupyingPlayer, isBuff, Skill.skillBuffDebuffAmount, permaBuff);
                        foreach (var fx in skillHitVFX)
                        {
                            fx.SpawnVFX(tile.occupyingPlayer.transform);
                            if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
                        }
                    }
                });
            }
            else if (skillTarget == SkillTarget.All)
            {
                // Handle buffing AP to all
            }
            // More if statements for other SkillTargets...
        }

        if (buffDebuffType == BuffDebuffType.HP)
        {
            if (skillEffect == SkillEffect.Heal)
            {
                if (skillTarget == SkillTarget.Self)
                {
                    // Handle buffing AP to self
                    Heal(ActivaterCharacter, Skill.skillBuffDebuffAmount);
                    foreach (var fx in skillHitVFX)
                    {
                        fx.SpawnVFX(ActivaterCharacter.characterTile.transform);
                        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
                    }
                }
            }
        }
        // More if statements for other BuffDebuffTypes...
    }
    else if (ActivaterCharacter is Enemy)
    {
        if (buffDebuffType == BuffDebuffType.AP)
        {
            if (skillTarget == SkillTarget.Enemy)
            {
                // Handle debuffing AP to an enemy
            }
            else if (skillTarget == SkillTarget.MultipleEnemies)
            {
                // Handle debuffing AP to multiple enemies
            }
            else if (skillTarget == SkillTarget.AllEnemies)
            {
                // Handle debuffing AP to all enemies
            }
            // More if statements for other SkillTargets...
        }
        // More if statements for other BuffDebuffTypes...
    }

    OnComplete?.Invoke();
}

    private void ChangeAP(Character character ,bool increase, int amount, bool perma)
    {
        if (perma)
        {
            if (increase)
            {
                //permanent increase AP
                character.remainingActionPoints += amount;
                character.OnActionPointsChangeEvent?.Invoke(character.remainingActionPoints, "+");
            }
            else
            {
                //permanent decrease AP
                character.remainingActionPoints -= amount;
                character.OnActionPointsChangeEvent?.Invoke(character.remainingActionPoints, "-");
            }
        }
        else
        {
            if (increase)
            {
                //temporary increase AP
                character.temporaryActionPointsToDecrease += amount;
                character.remainingActionPoints += amount;
                character.OnActionPointsChangeEvent?.Invoke(character.remainingActionPoints, "+");
            }
           
        }
    }

    public void Heal(Character character, int amount)
    {
        character.health.HealthAi += amount;
        character.OnHealthChangeEvent?.Invoke();
    }
    
    

}