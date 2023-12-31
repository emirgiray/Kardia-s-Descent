using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPartData", menuName = "ScriptableObjects/Weapons/WeaponPartData", order = 1)]
public class WeaponPartData : ScriptableObject
{
    [PreviewField(Height= 200,Alignment =ObjectFieldAlignment.Left)]
    public GameObject PartPrefab;
    [Space]
    public string partName = "";
    public enum PartType
    {
        Ranged, Melee,
    }
    [BoxGroup("Part Info")][EnumPaging]
    public PartType partType;
    public enum PartClass
    {
        Pistol, Rifle, Shotgun, Sniper, SMG, LMG, Axe, Sword, Dagger
    }
    [BoxGroup("Part Info")]
    public PartClass partClass;

    public enum DamageType
    {
        Random, Physical, Fire, Ice, Poison, Electric, Explosive,
    }
    [BoxGroup("Part Info")]
    public DamageType damageType;
}
