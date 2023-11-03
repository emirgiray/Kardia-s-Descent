using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGT_Tools.AI;

public class SGT_WeaponSelectCenter : MonoBehaviour
{

    //When you want to control all childs Weapons can give damage or not. You can change all weapons from here.

    public List<SGT_WeaponSelect> _WeaponSelects = new List<SGT_WeaponSelect>();
    public List<SGT_Damageble> _Damagable = new List<SGT_Damageble>();

    /// <summary>
    /// You can disable in start
    /// </summary>
    public bool IgnoreInStart = false;

    void Start()
    {
        if (!IgnoreInStart)
        {
            SGT_WeaponSelect[] weapons = gameObject.GetComponentsInChildren<SGT_WeaponSelect>();

            for (int i = 0; i < weapons.Length; i++)
            {
                _WeaponSelects.Add(weapons[i]);
            }

            SGT_Damageble[] _Damager = gameObject.GetComponentsInChildren<SGT_Damageble>();

            for (int i = 0; i < _Damager.Length; i++)
            {
                _Damagable.Add(_Damager[i]);
            }
        }




        

    }


    /// <summary>
    /// Weapons can give damage or not
    /// </summary>
    /// <param name="Value"></param>
    public void WeaponsCanGiveDamageOrNot(bool Value)
    {

        for (int i = 0; i < _WeaponSelects.Count; i++)
        {
            _WeaponSelects[i].CanGiveDamage = Value;
        }

    }


    /// <summary>
    /// Damager can give damage or not
    /// </summary>
    /// <param name="Value"></param>
    public void DamagebleCanGiveDamageOrNot(bool Value)
    {

        for (int i = 0; i < _Damagable.Count; i++)
        {
            _Damagable[i].CanGetDamageControl(Value);
        }

    }




    public void FindWeaponSelectManuel()
    {

        SGT_WeaponSelect[] weapons = gameObject.GetComponentsInChildren<SGT_WeaponSelect>();

        for (int i = 0; i < weapons.Length; i++)
        {
            _WeaponSelects.Add(weapons[i]);
        }

    }


    public void FindDamagerSelectManuel()
    {

        SGT_Damageble[] _Damager = gameObject.GetComponentsInChildren<SGT_Damageble>();

        for (int i = 0; i < _Damager.Length; i++)
        {
            _Damagable.Add(_Damager[i]);
        }

    }







}
