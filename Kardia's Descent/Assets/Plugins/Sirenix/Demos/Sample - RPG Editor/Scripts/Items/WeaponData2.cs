using System.Collections.Generic;
using UnityEngine;



namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    [CreateAssetMenu(fileName = "WeaponPartData", menuName = "Weapons/WeaponPartData2", order = 1)]
    public class WeaponData2 : ScriptableObject
    {
       [PreviewField(Height= 200,Alignment =ObjectFieldAlignment.Left)]
    public GameObject WeaponPrefab;
    [Space]
    public string weaponName = "";
    
    public enum WeaponRarity
    {
        Common, Uncommon, Rare, Epic, Legendary
    }

    [BoxGroup("Weapon Info")]
    public WeaponRarity weaponRarity;
    public enum WeaponType
    {
        Ranged, Melee,
    }
    [Space]
    [BoxGroup("Weapon Info")][EnumPaging]
    public WeaponType weaponType;
    public enum WeaponClass
    { 
        Pistol, Rifle, Shotgun, Sniper, SMG, LMG, Axe, Sword, Dagger
    }
    [BoxGroup("Weapon Info")]
    public WeaponClass weaponClass;
    public enum WeaponHand
    {
        OneHand, TwoHand,
    }
    [BoxGroup("Weapon Info")][EnumPaging]
    public WeaponHand weaponHand;
    public enum DamageType
    {
        Random, Physical, Fire, Ice, Poison, Electric, Explosive 
    }
    [BoxGroup("Weapon Info")]
    public DamageType damageType;
    
    
    [BoxGroup("Weapon Stats")]
    [BoxGroup("Weapon Stats")] [ShowInInspector] private Vector2 DamageRandomRange = new Vector2(1,50);
    [MinMaxSlider("DamageRandomRange", true)]
    [BoxGroup("Weapon Stats")]public Vector2 damage = new Vector2(3, 5);
    [Space]
    [BoxGroup("Weapon Stats")]public float accuracy;
    [BoxGroup("Weapon Stats")]public float range;
    [BoxGroup("Weapon Stats")]public int fireAmount;
    [BoxGroup("Weapon Stats")]public float fireRate;
    [BoxGroup("Weapon Stats")] [ShowIf("weaponType", WeaponType.Ranged)] public float reloadTime;
    [BoxGroup("Weapon Stats")] [ShowIf("weaponType", WeaponType.Ranged)] public float bulletSpeed;
    [BoxGroup("Weapon Stats")] [ShowIf("weaponType", WeaponType.Ranged)] public float bulletLifeTime;
    [BoxGroup("Weapon Stats")] [ShowIf("weaponType", WeaponType.Ranged)] public float bulletSize;
    public bool starterWeapon;
    public bool selected = false;
    public bool unlocked = false;
    
    [BoxGroup("Weapon Parts")]public int collectedParts = 0;
    [BoxGroup("Weapon Parts")]public int totalParts = 3;
    [BoxGroup("Weapon Parts")] public List<WeaponPartData2> weaponParts= new List<WeaponPartData2>();

    public void ResizePartsList()
    {
        weaponParts= new List<WeaponPartData2>(totalParts);
    }

    [Button(ButtonSizes.Medium),GUIColor(0.4f,0.8f,1)][PropertyOrder(0)]
    public int DoDamage()
    {
        //Debug.Log("Damage : "+Mathf.RoundToInt(Random.Range(damage.x, damage.y)));
        return Mathf.RoundToInt(Random.Range(damage.x, damage.y));
        
    }



    [Button(ButtonSizes.Medium),GUIColor(1f,0f,0)][PropertyOrder(0)]
    public void ResetUnlock()
    {
        unlocked = false;
        collectedParts = 0;
    }

    [Button(ButtonSizes.Medium),GUIColor(0,1,0)][PropertyOrder(0)]
    public DamageType SelectRandomDamageType()
    {
        if (damageType == DamageType.Random)
        {
            damageType = (DamageType)Random.Range(1, damageType.ToString().Length); //start from 1 to not select random again
            return damageType;
        }
        return damageType;
    }
    }
}

