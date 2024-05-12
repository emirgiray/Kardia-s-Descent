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
    public Vector3 scale = Vector3.one;
    public Vector3 rotationOffset = Vector3.zero;
    
    
    public void SpawnVFX(Transform spawnPosition/*, Vector3 spawnOffset, float VFXDuration*/)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, spawnPosition.rotation * Quaternion.Euler(rotationOffset));
        vfx.transform.localScale = scale;
        if (VFXDuration > 0)
        {
            Destroy(vfx, VFXDuration);
        }
        //spawnOffset = Vector3.zero;
    }
    
    public void SpawnVFX(Transform spawnPosition, Transform parent)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, spawnPosition.rotation * Quaternion.Euler(rotationOffset), parent);
        vfx.transform.localScale = scale;
        if (VFXDuration > 0)
        {
            Destroy(vfx, VFXDuration);
        }
    }
    public void SpawnVFX(Transform spawnPosition, Vector3 rotationTowards)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, Quaternion.LookRotation(rotationTowards, Vector3.up) * Quaternion.Euler(rotationOffset));
        vfx.transform.localScale = scale;
        if (VFXDuration > 0)
        {
            Destroy(vfx, VFXDuration);
        }
    }
    public GameObject SpawnVFXWithReturn(Transform spawnPosition/*, Vector3 spawnOffset, float VFXDuration*/)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, spawnPosition.rotation * Quaternion.Euler(rotationOffset));
        vfx.transform.localScale = scale;
        if (VFXDuration > 0)
        {
            Destroy(vfx, VFXDuration);
        }

        return vfx;
        //spawnOffset = Vector3.zero;
    }
    
    public GameObject SpawnVFXWithReturn(Transform spawnPosition, Transform parent)
    {
        GameObject vfx = Instantiate(VFXPrefab, spawnPosition.position + spawnOffset, spawnPosition.rotation * Quaternion.Euler(rotationOffset), parent);
        vfx.transform.localScale = scale;
        if (VFXDuration > 0)
        {
            Destroy(vfx, VFXDuration);
        }

        return vfx;
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
