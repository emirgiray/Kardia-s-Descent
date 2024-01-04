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
    public VFXSpawner skillStartVFX;
    public VFXSpawner[] skillHitVFX;
    public VFXSpawner skillMissVFX;
    public SGT_AudioEvent skillAudioEvent;
    [EnumPaging]
    public enum SkillType
    {
        Ranged, Melee
    }
    public SkillType skillType;
    
    [EnumPaging]
    public enum PassiveOrActive
    {
        Active, Passive 
    }

    public PassiveOrActive passiveOrActive;
    [EnumPaging]
    public enum SkillClass
    {
        Pistol, Rifle, Shotgun, Sniper, SMG, LMG, Axe, Sword, Dagger, Active, Passive, Buff
    }
    public SkillClass skillClass;
    [EnumPaging]
    public enum DamageType
    {
        Physical, Random, Fire, Ice, Poison, Electric, Explosive, None
    }
    public DamageType damageType;
    [EnumPaging]
    public enum SkillTarget
    {
         Enemy, Ally, Self,MultipleEnemies, MultipleAllies,  AllEnemies, AllAllies, All
    }
    public SkillTarget skillTarget;
    
    [EnumPaging]
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
    

    public virtual void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        
    }

    //tried to move the skill activation to the base class, but it might get too complicated
    
    public virtual bool TryHit(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        int random = UnityEngine.Random.Range(1, 101);
        if (random <= Skill.accuracy || Skill.accuracy == 100 || Skill.accuracy > 100) //hit
        {
            Debug.Log($"HIT: {random} < {Skill.accuracy}");
            //OnHit(Skill, ActivaterCharacter, selectedTile, OnComplete);
            return true;
        }
        else
        {
            Debug.Log($"MISSED: {random} > {Skill.accuracy}");
            //OnMiss(Skill, ActivaterCharacter, selectedTile, OnComplete);
            return false;
        }
    }

    /*
    public virtual void OnHit(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        if (ActivaterCharacter is Player)
        {
            if (selectedTile.occupiedByEnemy)
            {
                foreach (var fx in skillHitVFX)
                {
                    fx.SpawnVFX(selectedTile.occupyingEnemy.transform);
                }
                
                selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                //selectedTile.occupyingEnemy.Stun(true, skillEffectDuration);
            }

            if (selectedTile.OccupiedByCoverPoint)
            {
                foreach (var fx in skillHitVFX)
                {
                    fx.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                }
                
                selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
            }
        }

        if (ActivaterCharacter is Enemy)
        {
            if (selectedTile.occupiedByPlayer)
            {
                foreach (var fx in skillHitVFX)
                {
                    fx.SpawnVFX(selectedTile.occupyingPlayer.transform);
                }
                
                selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                //selectedTile.occupyingPlayer.Stun(true, skillEffectDuration);
            }

            if (selectedTile.OccupiedByCoverPoint)
            {
                foreach (var fx in skillHitVFX)
                {
                    fx.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                }
                
                selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
            }
        }
    }
    */
    
    public void OnMiss(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        if (ActivaterCharacter is Player && selectedTile.occupiedByEnemy)
        {
            skillMissVFX.SpawnVFX(selectedTile.occupyingEnemy.transform);
            selectedTile.occupyingEnemy.GetComponent<SGT_Health>().Miss();
        }
        if (ActivaterCharacter is Enemy && selectedTile.occupiedByPlayer)
        {
            skillMissVFX.SpawnVFX(selectedTile.occupyingPlayer.transform);
            selectedTile.occupyingPlayer.GetComponent<SGT_Health>().Miss();
        }
        
        //Debug.Log($"on hit 2");
    }

    public virtual void DoSomething(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        
    }

    public virtual void DoDamage(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        if (ActivaterCharacter is Player && selectedTile.occupiedByEnemy)
        {
            foreach (var fx in skillHitVFX)
            {
                fx.SpawnVFX(selectedTile.occupyingEnemy.transform);
            }

            selectedTile.occupyingEnemy.GetComponent<DamageHandler>().TakeDamage(Skill.damage, ActivaterCharacter);
            
        }
        if (ActivaterCharacter is Enemy && selectedTile.occupiedByPlayer)
        {
            foreach (var fx in skillHitVFX)
            {
                fx.SpawnVFX(selectedTile.occupyingPlayer.transform);
            }

            selectedTile.occupyingPlayer.GetComponent<DamageHandler>().TakeDamage(Skill.damage, ActivaterCharacter);
        }
        
        if (selectedTile.OccupiedByCoverPoint)
        {
            foreach (var fx in skillHitVFX)
            {
                fx.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
            }

            if (selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>() != null) selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
        }
    }
    public void DoStun(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        if (ActivaterCharacter is Player && selectedTile.occupiedByEnemy)
        {
            selectedTile.occupyingEnemy.Stun(true, skillEffectDuration);
        }
        if (ActivaterCharacter is Enemy && selectedTile.occupiedByPlayer)
        {
            selectedTile.occupyingPlayer.Stun(true, skillEffectDuration);
        }

    }

    public void DoKnockBack(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,  Action OnComplete = null)
    {
        Vector3 dir = selectedTile.transform.position - ActivaterCharacter.transform.position;
        dir.y = 0;
        dir.Normalize();
        
        Vector3 newPos = selectedTile.transform.position + dir * Pathfinder.Instance.HEXAGONAL_OFFSET * skillBuffDebuffAmount;

        newPos = new Vector3(newPos.x, 5, newPos.z);
        
        if (Physics.Raycast(newPos, Vector3.down, out RaycastHit hit, 50f, ActivaterCharacter.GroundLayerMask))
        {
            var newTile = hit.transform.GetComponent<Tile>();

            if (!newTile.Occupied)
            {
                
                Debug.Log($"not occupied starting move to {newTile.name}"); 
                // Path newPath = Pathfinder.Instance.PathBetween(ActivaterCharacter, newTile, selectedTile);
                Path newPath = Pathfinder.Instance.MakePath(newTile, selectedTile);
                
                if (ActivaterCharacter is Player && selectedTile.occupiedByEnemy)
                {
                    Character defender = selectedTile.occupyingEnemy;
                    // defender.canMove = true;
                    // selectedTile.occupyingEnemy.StartMove(newPath, false, ()=> defender.canMove = false, false);
                    defender.Rotate(ActivaterCharacter.transform.position, 0.5f);
                    selectedTile.occupyingEnemy.StartKnockbackMove(newPath, false, null, false);
                    
                }
                if (ActivaterCharacter is Enemy && selectedTile.occupiedByPlayer)
                {
                    Character defender = selectedTile.occupyingPlayer;
                    // defender.canMove = true;
                    // selectedTile.occupyingPlayer.StartMove(newPath, false, ()=> defender.canMove = false, false);
                    defender.Rotate(ActivaterCharacter.transform.position, 0.5f);
                    selectedTile.occupyingPlayer.StartKnockbackMove(newPath, false, null, false);
                }
                
            }
            else
            {
                Debug.Log($"ocuppied");
            }
        }
        else
        {
            Debug.Log($"no hit");
        }
        
    }
    
    
}
