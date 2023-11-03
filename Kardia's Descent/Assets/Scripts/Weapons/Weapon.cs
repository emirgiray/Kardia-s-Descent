using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    
    public Transform barrelFirePoint;
    public Transform gripPoint;
    [SerializeField] private GameObject Bullet;
    

    public void Shoot()
    {
        Instantiate(Bullet, barrelFirePoint.position, barrelFirePoint.rotation);
    }
}
