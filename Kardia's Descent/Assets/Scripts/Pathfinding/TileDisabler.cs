using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDisabler : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float rayOriginOffset = 100f;
    [SerializeField] private float rayLength = 9999;
    
    
    private void Awake()
    {
        //get all children gameobjects
        
        /*MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();*/
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        foreach (Renderer child in renderers)
        {
            if (Physics.Raycast(child.transform.position + Vector3.up * rayOriginOffset, Vector3.down, out RaycastHit hit, rayLength, groundLayerMask))
            {
                hit.transform.gameObject.GetComponent<Tile>().selectable = false;
                //Debug.Log($"disabler: {child.name}   disabled tile: {hit.transform.gameObject.name}");
            }

        }
    }
}
