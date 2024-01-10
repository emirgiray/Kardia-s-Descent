using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public Tile waypointTile;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private float yOffset = 1;
    
    void Start()
    {
        FindNewTile();
    }

    [Button, GUIColor(1f, 1f, 0.1f)]
    public void GoToTile()
    {
        if (waypointTile != null)
        { 
            FinalizePosition(waypointTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
        
        Debug.Log("No tile found for waypoint: " + gameObject.name + " at position: " + transform.position + "");
    }

    [Button, GUIColor(0f, 1f, 0.5f)]
    public void FindNewTile()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
        
        Debug.Log("No tile found for waypoint: " + gameObject.name + " at position: " + transform.position + "");
    }

    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0,yOffset,0);
        waypointTile = tile;
        
    }
}
