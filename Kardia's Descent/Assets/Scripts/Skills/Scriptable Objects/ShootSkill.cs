using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/ShootSkill", order = 1)]
public class ShootSkill : SkillsData
{
    public GameObject Bullet;
    public override void ActivateSkill(SkillContainer.Skills Skill ,GameObject parent, Tile selectedTile, Action OnComplete = null)//skill logic goes here
    {
        //base.ActivateSkill();
        WeaponContainer weaponContainer = parent.GetComponentInChildren<WeaponContainer>();

        switch (base.skillTarget)
        {
            case SkillTarget.Enemy:
                /*weaponContainer.currentWeapon.*/Shoot( weaponContainer);


                if (accuracy < 100) //todo do this in a different way, maybe a method in a class
                {
                    int random = UnityEngine.Random.Range(0, 101);
                    if (random <= Skill.accuracy)
                    {
                        Debug.Log("Hit: " + random + " < " + Skill.accuracy);
                        if (selectedTile.occupyingEnemy != null)
                        {
                            selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        }
                        if (selectedTile.occupyingPlayer != null)
                        {
                            selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        }
                    }
                    else
                    {
                        Debug.Log("Missed: " + random + " < " + Skill.accuracy);
                    }
                }
 
                //add delay of 0.1f seconds here
                OnComplete?.Invoke();
                break;
        }
        
        
    }
    
    public void Shoot(WeaponContainer weaponContainer)
    {
        Instantiate(Bullet, weaponContainer.currentWeapon.barrelFirePoint.position, weaponContainer.currentWeapon.barrelFirePoint.rotation);
    }
  
}
