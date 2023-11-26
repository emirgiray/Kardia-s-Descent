using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/SkillsData", order = 1)]
public class SkillsData : ScriptableObject
{
    [PreviewField(Height= 200,Alignment =ObjectFieldAlignment.Left)]
    public Sprite skillSprite;
    [Space]
    public string skillName = "";
    [Multiline] public string skillDescription = "";
    public string skillID = "";
    public int skillDamage = 1;
    public int skillRange= 3;
    public int skillCooldown = 1;
    public int actionPointUse = 1;
    public int skillEffectDuration = 1;
    public int skillBuffDebuffAmount = 1;
    public int accuracy = 80;
    public int coverAccuracyDebuff = 20;
    public AnimatorOverrideController animatorOverrideController;
    public enum SkillType
    {
        Ranged, Melee
    }
    public SkillType skillType;
    public enum SkillClass
    {
        Pistol, Rifle, Shotgun, Sniper, SMG, LMG, Axe, Sword, Dagger, Active, Passive, Buff
    }
    public SkillClass skillClass;
    public enum DamageType
    {
        Physical, Random, Fire, Ice, Poison, Electric, Explosive, None
    }
    public DamageType damageType;
    public enum SkillTarget
    {
         Enemy, Ally, Self,MultipleEnemies, MultipleAllies,  AllEnemies, AllAllies, All
    }
    public SkillTarget skillTarget;
    
    public enum SkillHitType
    {
        Single, Area, Line, Cone, AllInRange, AllInLine, AllInCone, All
    }
    public SkillHitType skillHitType;
    
    [EnumToggleButtons]
    public enum SkillEffect
    {
        None, Buff, Debuff, Bleed, Burn, Freeze, Poison, Shock, Stun, Slow, Blind, Confuse, Sleep, Charm, Fear, Taunt, Silence, Curse, Heal, Revive, Shield, Summon
    }
    public SkillEffect skillEffect;
    
    [EnumToggleButtons]
    public enum BuffDebuffType
    {
        None, AP, HP, Damage, Accuracy, Dodge, Cover, Movement, Range, SkillCooldown
    }
    public BuffDebuffType buffDebuffType;
    
   // [PropertySpace(SpaceAfter = 500)]
    

    public virtual void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent,  Action OnComplete = null)
    {
        
    }

    //tried to move the skill activation to the base class, but it might get too complicated
    
    public bool TryHit(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent,  Action OnComplete = null)
    {
        int random = UnityEngine.Random.Range(1, 101);
        if (random <= Skill.accuracy || Skill.accuracy == 100) //hit
        {
            Debug.Log($"HIT: {random} < {Skill.accuracy}");
            OnHit(Skill, ActivaterCharacter, selectedTile, parent, OnComplete);
            return true;
        }
        else
        {
            Debug.Log($"MISSED: {random} > {Skill.accuracy}");
            OnMiss();
            return false;
        }
    }

    public void OnHit(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent,  Action OnComplete = null)
    {
        if (ActivaterCharacter is Player)
        {
            if (selectedTile.occupiedByEnemy)
            {
                selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                selectedTile.occupyingEnemy.Stun(true, skillEffectDuration);
            }

            if (selectedTile.OccupiedByCoverPoint)
            {
                selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
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
    }
    
    public void OnMiss()
    {
        //Debug.Log($"on hit 2");
    }

    public void DoDamage()
    {
        
    }
    
}
