using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    [SerializeField] private Tile objectTile;
    [SerializeField] private bool findTileAtStart = false;
    
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float yOffset = 0.18f;
    [SerializeField] private List<Tile> coveringTiles = new List<Tile>();
    [SerializeField] private GameObject HitTextGameObject;
    [SerializeField] private SGT_Health health;

    [SerializeField] private bool randomizeAppereance = false;
    [ShowIf("randomizeAppereance")]
    [SerializeField] private GameObject randomParent;
    [ShowIf("randomizeAppereance")]
    [SerializeField] private List<GameObject> randomAppereancesList = new List<GameObject>();
    
    
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

        List<GameObject> children = new();
        if (randomizeAppereance)
        {
            Destroy(randomParent);
            
            /*for (int i = 0; i < randomParent.transform.childCount; i++)
            {
                children.Add(randomParent.transform.GetChild(i).gameObject);
            }
            
            children.ForEach(x => Destroy(x));*/
            
            int randomIndex = Random.Range(0, randomAppereancesList.Count);
            GameObject randomAppereance = Instantiate(randomAppereancesList[randomIndex], this.transform);
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
    
    /// <summary>
    /// Used for spawning miss text on the cover point
    /// </summary>
    /// <param name="value"></param>
    public void SpawnText(string value)
    {
        if (value == "MISS")
        {
            everythingUseful.SpawnText(value, value != "MISS" ? Color.red : Color.white, transform, 4,1f, health);
        }
    }

    public void SpawnText(int baseValue, float damageMultipliar)
    {
        int totalDamage = Mathf.RoundToInt(baseValue * damageMultipliar);
        int extraDamage = totalDamage - baseValue;
        
        everythingUseful.SpawnText(totalDamage, extraDamage, Color.red,Color.white ,transform, 4, 1f, health);

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

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y - 5f, 1.5f).OnComplete(()=>Destroy(gameObject));
    }
}
