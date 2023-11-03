using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPartDataCenter", menuName = "Weapons/WeaponPartDataCenter", order = 1)]
public class WeaponPartDataCenter : ScriptableObject
{
  public List<WeaponPartData> weaponPartData = new List<WeaponPartData>();
}
