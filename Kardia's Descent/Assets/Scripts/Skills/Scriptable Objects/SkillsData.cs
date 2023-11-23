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
    public int accuracy = 80;
    public int coverAccuracyDebuff = 20;
    public int skillAnimOverrideIndex = 0;
    public AnimatorOverrideController animatorOverrideController;
    public enum SkillType
    {
        Ranged, Melee
    }
    public SkillType skillType;
    public enum SkillClass
    {
        Pistol, Rifle, Shotgun, Sniper, SMG, LMG, Axe, Sword, Dagger, Active, Passive
    }
    public SkillClass skillClass;
    public enum DamageType
    {
        Physical, Random, Fire, Ice, Poison, Electric, Explosive,
    }
    public DamageType damageType;
    public enum SkillTarget
    {
         Enemy, Ally, Self, AllEnemies, AllAllies, All
    }
    public SkillTarget skillTarget;
    
    public enum SkillHitType
    {
        Single, Area, Line, Cone
    }
    public SkillHitType skillHitType;
    
    public enum SkillEffect
    {
        None, Bleed, Burn, Freeze, Poison, Shock, Stun, Slow, Blind, Confuse, Sleep, Charm, Fear, Taunt, Silence, Curse, Buff, Debuff, Heal, Revive, Shield, Summon
    }
    public SkillEffect skillEffect;

    public virtual void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent,  Action OnComplete = null)
    {
        
    }
    
}
