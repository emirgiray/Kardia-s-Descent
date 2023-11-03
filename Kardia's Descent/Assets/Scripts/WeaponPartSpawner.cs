using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPartSpawner : MonoBehaviour
{
    [SerializeField] private WeaponPartDataCenter weaponPartDataCenter;

    public void SpawnRandomPart()
    {
        Instantiate( weaponPartDataCenter.weaponPartData[Random.Range(0, weaponPartDataCenter.weaponPartData.Count)]);
    }
    
}
