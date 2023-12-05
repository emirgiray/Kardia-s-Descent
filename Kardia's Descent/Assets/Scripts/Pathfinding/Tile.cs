using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Tile : MonoBehaviour
{
    [BoxGroup("Tile")] public bool selectable = true;
    [BoxGroup("Tile")] public Tile parent;
    [BoxGroup("Tile")] public Tile connectedTile;
    [BoxGroup("Tile")] public bool Occupied { get; set; } = false;
    [BoxGroup("Tile")] public Character occupyingCharacter;
    [BoxGroup("Tile")] public GameObject occupyingGO;
    [BoxGroup("Tile")] public MeshRenderer meshRenderer;
    [BoxGroup("Tile")] public GameObject TileHighlightGO;
    [BoxGroup("Tile")] public MeshRenderer TileHighlightMeshRenderer;
    
    [BoxGroup("Player")] public bool occupiedByPlayer = false;
    [BoxGroup("Player")] public Player occupyingPlayer;
    
    [BoxGroup("Enemy")] public bool occupiedByEnemy = false;
    [BoxGroup("Enemy")] public Enemy occupyingEnemy;
    
    [BoxGroup("Cover Point")] public bool OccupiedByCoverPoint = false;
    [BoxGroup("Cover Point")] public CoverPoint occupyingCoverPoint;
    [BoxGroup("Cover Point")] public bool isCoveredByCoverPoint = false;

    [HideInInspector] [BoxGroup("Tile")] public float costFromOrigin = 0;
    [HideInInspector] [BoxGroup("Tile")]  public float costToDestination = 0;
    [HideInInspector] [BoxGroup("Tile")] public int terrainCost = 0;
    [HideInInspector] [BoxGroup("Tile")] public float TotalCost { get { return costFromOrigin + costToDestination + terrainCost; } }
    
    Dictionary<int, Color> costMap = new Dictionary<int, Color>()
    {
        {0, new Color(0.42f, 0.38f, 0.38f) }, //Gray
        {1, new Color(0.45f, 0.23f, 0.15f) },//Red
        {2, new Color(0.3f, 0.1f, 0f) } //Dark red
    };

    
    
    /*private void Start()
    {
        if (TileHighlightGO != null)
        {
            TileHighlightMeshRenderer = TileHighlightGO.GetComponent<MeshRenderer>();
        }
    }*/

    /*private void Update()
    {
        if (meshRenderer != null)
        {
            Debug.Log($"meshRenderer.material.color.a: {meshRenderer.material.GetFloat("_Alpha")}");
        }
        
    }*/

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    public void Highlight(Color color)
    {
        SetColor(color);
    }
    
    public void HighlightRed()
    {
        SetColor(Color.red);
    }

    public void HighlightMoveable()
    {
        if (TileHighlightGO != null)
        {
            TileHighlightGO.SetActive(true);
            // TileHighlightMeshRenderer.material.color = Interact.Instance.tileHighligthColors[0];
            TileHighlightMeshRenderer.material.SetColor("_RimColor", Interact.Instance.tileHighligthColors[0]);
        }
        else
        {
            SetColor(new Color(0.3f, 0.8f, 0.3f, 1));
        }
        
    }

    public void HighlightAttackable()
    {
        if (TileHighlightGO != null)
        {
            TileHighlightGO.SetActive(true);
            // TileHighlightMeshRenderer.material.color = Interact.Instance.tileHighligthColors[1];
            TileHighlightMeshRenderer.material.SetColor("_RimColor", Interact.Instance.tileHighligthColors[1]);
        }
        else
        {
            SetColor(new Color(1, 0.5f, 0.1f, 1));
        }
        
    }
    public void ClearHighlight()
    {
        if (TileHighlightGO != null)
        {
            TileHighlightGO.SetActive(false);
        }
        else
        {
            SetColor(costMap[terrainCost]);
        }
        
    }

    /// <summary>
    /// This is called when right clicking a tile to increase its cost
    /// </summary>
    /// <param name="value"></param>
    public void ModifyCost()
    {
        terrainCost++;
        if (terrainCost > costMap.Count-1)
            terrainCost = 0;

        if (costMap.TryGetValue(terrainCost, out Color col))
        {
            SetColor(col);
        }
        else
        {
            Debug.Log("Invalid terraincost or mapping" + terrainCost);
        }
    }

    private void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public void ResetOcupying()
    {
        occupyingCharacter = null;
        occupyingPlayer = null;
        occupyingEnemy = null;
        occupyingGO = null;
    }
    
    /*public void DebugCostText()
    {
        costText.text = TotalCost.ToString("F1");
    }

    public void ClearText()
    {
        costText.text = "";
    }*/
}
