using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer : MonoBehaviour
{
    [SerializeField] private Transform hand;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] public Weapon currentWeapon;


    public void AddWeapon(WeaponData weaponDataIn, Weapon currentWeaponIn)
    {
        weaponData = weaponDataIn;
        currentWeapon = currentWeaponIn;
    }
}
