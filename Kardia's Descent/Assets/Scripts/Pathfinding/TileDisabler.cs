using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileDisabler : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float rayOriginOffset = 100f;
    [SerializeField] private float rayLength = 9999;
    [SerializeField] private bool active = false;
    [SerializeField] [ShowIf("active")] private bool rendererChildren = true;
    [SerializeField] [ShowIf("active")] private bool emptyChildren = false;

    [SerializeField] private List<Tile> disabledTiles = new List<Tile>();
    
    private void Awake()
    {
        if (active)
        {
            if (rendererChildren)
            {
                DisableWithRendererChildren();
            }

            if (emptyChildren)
            {
                DisableWithEmptyChildren();
            }
            
            
        }
    }

    private void DisableWithRendererChildren()
    {
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

    private void DisableWithEmptyChildren()
    {
        foreach (var child in transform)
        {
            if (Physics.Raycast(((Transform) child).position + Vector3.up * rayOriginOffset, Vector3.down, out RaycastHit hit, rayLength, groundLayerMask))
            {
                disabledTiles.Add(hit.transform.gameObject.GetComponent<Tile>());
                hit.transform.gameObject.GetComponent<Tile>().selectable = false;
                //Debug.Log($"disabler: {((Transform) child).name}   disabled tile: {hit.transform.gameObject.name}");
            }
        }
    }

    public void ReEnableTiles()
    {
        foreach (var tile in disabledTiles)
        {
            tile.selectable = true;
        }

    }

    public void ReDeisableTiles()
    {
        foreach (var tile in disabledTiles)
        {
            tile.selectable = false;
        }
    }
    

}


