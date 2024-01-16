using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private Tile objectTile;
    [SerializeField] private bool findTileAtStart = false;
    
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float yOffset = 0.18f;
    [SerializeField] private List<Tile> coveringTiles = new List<Tile>();
    

    void Start()
    {
        if (findTileAtStart)
        {
            FindNewTile();
        }
        else
        {
            if (objectTile == null)
            {
                Debug.LogError("Cover point: " + gameObject.name + " has no object tile !!! ");
            }
        }

        if (objectTile != null)
        {
            objectTile.Occupied = true;
            objectTile.OccupiedByCoverPoint = true;
            objectTile.occupyingCoverPoint = this;
            objectTile.occupyingGO = this.gameObject;
            coveringTiles = pathfinder.NeighborTiles(objectTile, true);
            for (int i = 0; i < coveringTiles.Count; i++)
            {
                coveringTiles[i].isCoveredByCoverPoint = true;
            }
        }

        
    }

    [Button, GUIColor(1f, 1f, 0.1f)]
    public void GoToTile()
    {
        if (objectTile != null)
        { 
            FinalizePosition(objectTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, PathfinderVariables.Instance.tileMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }
        
        Debug.Log("No tile found for cover point: " + gameObject.name + " at position: " + transform.position + "");
    }

    [Button, GUIColor(0f, 1f, 0.5f)]
    public void FindNewTile()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, PathfinderVariables.Instance.tileMask))
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
        coveringTiles = pathfinder.NeighborTiles(tile, true);
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
