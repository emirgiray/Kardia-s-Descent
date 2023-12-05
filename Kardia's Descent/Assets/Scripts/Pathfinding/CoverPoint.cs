using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private Tile objectTile;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private float yOffset = 0.18f;
    [SerializeField] private List<Tile> coveringTiles = new List<Tile>();
    

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
        
        Debug.Log("No tile found for cover point: " + gameObject.name + " at position: " + transform.position + "");
    }

    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0,yOffset,0);
        objectTile = tile;
        tile.Occupied = true;
        tile.OccupiedByCoverPoint = true;
        tile.occupyingCoverPoint = this;
        tile.occupyingGO = this.gameObject;
        coveringTiles = Pathfinder.Instance.NeighborTiles(tile, true);
        for (int i = 0; i < coveringTiles.Count; i++)
        {
            coveringTiles[i].isCoveredByCoverPoint = true;
        }
        
    }

    public void OnCoverDestroyed()
    {
        for (int i = 0; i < coveringTiles.Count; i++)
        {
            coveringTiles[i].isCoveredByCoverPoint = false;
        }
        coveringTiles.Clear();
        objectTile.Occupied = false;
        objectTile.OccupiedByCoverPoint = false;
        objectTile.occupyingCoverPoint = null;
        objectTile.occupyingGO = null;
        objectTile = null;

        StartCoroutine(DeathDelay());
    }

    public IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y - 1.5f, 0.5f);
    }
}
