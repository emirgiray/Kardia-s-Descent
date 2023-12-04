using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXSpawner", menuName = "ScriptableObjects/VFXSpawner", order = 0)]

public class VFXSpawner : ScriptableObject
{
    public GameObject VFXPrefab;
    public float VFXDuration;
    public Transform spawnPosition;
    public Vector3 spawnOffset;
    
    
    public void SpawnVFX(Transform spawnPosition/*, Vector3 spawnOffset, float VFXDuration*/)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, Quaternion.identity);
        Destroy(vfx, VFXDuration);
        //spawnOffset = Vector3.zero;
    }

    public void SetSpawnOffset(Vector3 spawnOffset)
    {
        this.spawnOffset = spawnOffset;
    }

    public void SetDuration(float VFXDuration)
    {
        this.VFXDuration = VFXDuration;
    }
}
