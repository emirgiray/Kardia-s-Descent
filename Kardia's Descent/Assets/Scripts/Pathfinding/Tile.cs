using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Tile : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    [BoxGroup("Tile")] [OnValueChanged("DeactivateVisualForEdior")]
    public bool selectable = true;
    [BoxGroup("Tile")] [SerializeField] 
    private bool disableMeshOnStart = true;
    [BoxGroup("Tile")] 
    public bool Occupied  = false;
    [BoxGroup("Tile")] 
    public Tile parent;
    [BoxGroup("Tile")] 
    public Tile connectedTile;
    [BoxGroup("Tile")] 
    public Character occupyingCharacter;
    [BoxGroup("Tile")] 
    public GameObject occupyingGO;
    [BoxGroup("Tile")] 
    public MeshRenderer meshRenderer;
    [BoxGroup("Tile")] 
    public GameObject TileHighlightGO;
    [BoxGroup("Tile")] [SerializeField] 
    private GameObject ShieldIcon;
    [BoxGroup("Tile")] 
    public MeshRenderer TileHighlightMeshRenderer;
    [BoxGroup("Tile")] 
    public QuickOutline quickOutline;
    
    [BoxGroup("Editor")] [SerializeField] 
    private Material defaultMat;
    [BoxGroup("Editor")] [SerializeField] 
    private Material disabledMat;
    
    [BoxGroup("Player")] [ShowIf("Occupied")]
    public bool occupiedByPlayer = false; 
    [BoxGroup("Player")] [ShowIf("occupiedByPlayer")]
    public Player occupyingPlayer;
    
    [BoxGroup("Enemy")] [ShowIf("Occupied")]
    public bool occupiedByEnemy = false;
    [BoxGroup("Enemy")] [ShowIf("occupiedByEnemy")]
    public Enemy occupyingEnemy;
    
    [BoxGroup("Cover Point")] [ShowIf("Occupied")]
    public bool OccupiedByCoverPoint = false;
    [BoxGroup("Cover Point")] [ShowIf("OccupiedByCoverPoint")]
    public CoverPoint occupyingCoverPoint;
    [BoxGroup("Cover Point")] 
    public bool isCoveredByCoverPoint = false;
    
    [BoxGroup("Interactable")] [ShowIf("Occupied")]
    public bool occupiedByInteractable = false;
    [BoxGroup("Interactable")] [ShowIf("occupiedByInteractable")]
    public Interactable occupyingInteractable;

    [HideInInspector] [BoxGroup("Tile")] 
    public float costFromOrigin = 0;
    [HideInInspector] [BoxGroup("Tile")] 
    public float costToDestination = 0;
    [HideInInspector] [BoxGroup("Tile")] 
    public int terrainCost = 0;
    [HideInInspector] [BoxGroup("Tile")] public float TotalCost { get { return costFromOrigin + costToDestination + terrainCost; } }
    
    Dictionary<int, Color> costMap = new Dictionary<int, Color>()
    {
        {0, new Color(0.42f, 0.38f, 0.38f) }, //Gray
        {1, new Color(0.45f, 0.23f, 0.15f) },//Red
        {2, new Color(0.3f, 0.1f, 0f) } //Dark red
    };

    [Button, GUIColor(1f, 0.1f, 0.1f)]
    public void DeactivateVisualForEdior()
    {
        if (selectable)
        {
            GetComponent<MeshRenderer>().material = defaultMat;
        }
        else
        {
            GetComponent<MeshRenderer>().material = disabledMat;
        }
    }
    
    private void Start()
    {
        /*if (TileHighlightGO != null)
        {
            TileHighlightMeshRenderer = TileHighlightGO.GetComponent<MeshRenderer>();
        }*/

        if (disableMeshOnStart)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

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
        TileHighlightGO.SetActive(true);
        // TileHighlightMeshRenderer.material.color = Interact.Instance.tileHighligthColors[0];

        if (isCoveredByCoverPoint)
        {
            // default
            //TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[3]); 
                
                
            // quick outline
            //TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthNormalColors[5]); //disable the color
            quickOutline.enabled = true;
            quickOutline.OutlineColor = everythingUseful.Interact.tileHighligthNormalColors[0];
             
            TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[5]); // less alpha 
        }
        else if (occupiedByInteractable)
        {
            quickOutline.enabled = true;
            quickOutline.OutlineColor = everythingUseful.Interact.tileHighligthNormalColors[0];
             
            TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[4]); 
        }
        else if (OccupiedByCoverPoint)
        {
            quickOutline.enabled = true;
            quickOutline.OutlineColor = everythingUseful.Interact.tileHighligthNormalColors[3];
             
            TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[6]);
        }
        else
        {
            // default
            //TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[0]); 
                 
            // quick outline
            //TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthNormalColors[5]); //disable the color
            quickOutline.enabled = true;
            quickOutline.OutlineColor = everythingUseful.Interact.tileHighligthNormalColors[0];
             
            TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[5]); // less alpha
        }
    }

    public void HighlightAttackable()
    {
        TileHighlightGO.SetActive(true);
        
        // default
        //TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[1]);
        
        // quick outline
        TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthNormalColors[5]);
        quickOutline.enabled = true;
        quickOutline.OutlineColor = everythingUseful.Interact.tileHighligthNormalColors[1];
    }

    public void Hig()
    {
        
    }
    
    public void ClearHighlight()
    {
        if (TileHighlightGO != null)
        {
            TileHighlightGO.SetActive(false);
            quickOutline.enabled = false;
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
        occupyingInteractable = null;
        occupyingCoverPoint = null;
        occupyingGO = null;
        occupiedByInteractable = false;
    }

    /*public void SwitchShieldIcon(bool value)
    {
        ShieldIcon.SetActive(value);
    }*/
    
    /*public void DebugCostText()
    {
        costText.text = TotalCost.ToString("F1");
    }

    public void ClearText()
    {
        costText.text = "";
    }*/
}
