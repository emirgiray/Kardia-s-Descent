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
    public string skillID = "";
    public int skillDamage = 1;
    public int skillRange= 3;
    public enum SkillType
    {
        Ranged, Melee, Magic
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
         Enemy, Self, Ally, AllEnemies, AllAllies, All
    }
    public SkillTarget skillTarget;
    public enum SkillEffect
    {
        None, Bleed, Burn, Freeze, Poison, Shock, Stun, Slow, Blind, Confuse, Sleep, Charm, Fear, Taunt, Silence, Curse, Buff, Debuff, Heal, Revive, Shield, Summon
    }
    public SkillEffect skillEffect;

    public virtual void ActivateSkill(GameObject parent, Tile selectedTile, Action OnComplete = null)
    {
        
    }
    
}
