using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGT_WeaponSelect : MonoBehaviour
{

    public SGT_Weapon Weapon;
    public bool CanGiveDamage = true;


    public void SetCanGiveDamage(bool Value)
    {

        CanGiveDamage = Value;
    }


}
