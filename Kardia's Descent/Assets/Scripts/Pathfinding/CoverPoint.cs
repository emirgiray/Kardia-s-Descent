using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private Tile objectTile;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private float yOffset = 1f;


    void Start()
    {
        FindTileAtstart();
    }

    public void FindTileAtstart()
    {
        if (objectTile != null)
        { 
            FinalizePosition(objectTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
    }

    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0,yOffset,0);
        objectTile = tile;
        tile.Occupied = true;
        tile.OccupiedByCoverPoint = true;
        tile.occupyingCoverPoint = this;
    }

    public void OnCoverDestroyed()
    {
        objectTile.Occupied = false;
        objectTile.OccupiedByCoverPoint = false;
        objectTile.occupyingCoverPoint = null;
        objectTile = null;
    }
    
}
