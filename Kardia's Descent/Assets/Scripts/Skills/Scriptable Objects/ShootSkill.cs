using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/ShootSkill", order = 1)]
public class ShootSkill : SkillsData
{
    public override void ActivateSkill(GameObject parent, Tile selectedTile, Action OnComplete = null)//skill logic goes here
    {
        //base.ActivateSkill();
        WeaponContainer weaponContainer = parent.GetComponentInChildren<WeaponContainer>();

        switch (base.skillTarget)
        {
            case SkillTarget.Enemy:
                weaponContainer.currentWeapon.Shoot();
                if(selectedTile.occupyingEnemy != null) selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(skillDamage);
                //add delay of 0.1f seconds here
                OnComplete?.Invoke();
                break;
        }
        
        
    }
  
}
