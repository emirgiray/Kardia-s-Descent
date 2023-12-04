using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "ScriptableObjects/Skills/BuffSkill", order = 3)]
public class BuffSkill : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent, Action OnComplete = null)
    {
        bool isBuff = base.skillEffect == SkillEffect.Buff; //if not buff, then it's debuff
        
        if (ActivaterCharacter is Player)
        {
            switch (buffDebuffType)
            {
                case BuffDebuffType.AP:
                    switch (skillTarget)
                    {
                        case SkillTarget.Self:
                            // Handle buffing AP to self
                            break;
                        case SkillTarget.MultipleAllies:
                            // Handle buffing AP to multiple allies
                            Pathfinder.Instance.GetAttackableTiles(ActivaterCharacter.characterTile, Skill).ForEach(tile =>
                                {
                                    if (tile.occupiedByPlayer)
                                    {
                                        ChangeAP(tile.occupyingPlayer, isBuff, Skill.skillBuffDebuffAmount);
                                        skillVFX.SpawnVFX(tile.occupyingPlayer.transform);
                                    }
                                });
                            break;
                        case SkillTarget.All:
                            // Handle buffing AP to all
                            break;
                        // More cases for other SkillTargets...
                    }
                    break;
                // More cases for other BuffDebuffTypes...
            }
        }
        else if (ActivaterCharacter is Enemy)
        {
            switch (buffDebuffType)
            {
                case BuffDebuffType.AP:
                    switch (skillTarget) {
                        case SkillTarget.Enemy:
                            // Handle debuffing AP to an enemy
                            break;
                        case SkillTarget.MultipleEnemies:
                            // Handle debuffing AP to multiple enemies
                            break;
                        case SkillTarget.AllEnemies:
                            // Handle debuffing AP to all enemies
                            break;
                        // More cases for other SkillTargets...
                    }
                    break;
                // More cases for other BuffDebuffTypes...
            }
        }
        
        OnComplete?.Invoke();
        
        /*switch (base.skillTarget)
        {
            case SkillTarget.MultipleAllies:
            {
                if (ActivaterCharacter is Player)
                {
                    foreach (var tile in Pathfinder.Instance.GetAttackableTiles(ActivaterCharacter.characterTile, Skill))
                    {
                        if (tile.occupiedByPlayer)
                        {
                            
                        }

                    }
                }

                if (ActivaterCharacter is Enemy)
                {
                    if (selectedTile.occupiedByPlayer)
                    {
                        selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        selectedTile.occupyingPlayer.Stun(true, skillEffectDuration);
                    }

                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    }
                }

                OnComplete?.Invoke();
                break;
            }
        }*/
    }

    public void ChangeAP(Character character ,bool increase, int amount)
    {
        if (increase)
        {
            //increase AP
            character.remainingActionPoints += amount;
            character.OnActionPointsChangeEvent?.Invoke(character.remainingActionPoints, "+");
        }
        else
        {
            //decrease AP
            character.remainingActionPoints -= amount;
            character.OnActionPointsChangeEvent?.Invoke(character.remainingActionPoints, "-");
        }
    }


}