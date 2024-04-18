using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Pathfinding.Examples;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Interact : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    public Action SkillHighlighted;
    public Action SkillSelected;
    public Action SkillDeselected;
    [BoxGroup("Objects")][SerializeField] 
    private GameObject InspectTileGO;
    [BoxGroup("Objects")][SerializeField] 
    private GameObject InspectTileCoverIconGO;
    MeshRenderer inspectTileMeshRenderer;
    [BoxGroup("Objects")][SerializeField] 
    public Camera mainCam;
    [BoxGroup("Objects")][SerializeField] 
    private Tile currentTile;
    [BoxGroup("Objects")][SerializeField] 
    private Tile lastTile;
    [BoxGroup("Objects")][SerializeField] 
    public Tile lastAttackedTile;
    [BoxGroup("Objects")][SerializeField] 
    private Character characterUnderMouse;
    [BoxGroup("Objects")][SerializeField] 
    private Character lastCharacterUnderMouse;
    [BoxGroup("Objects")][SerializeField] 
    public Character selectedCharacter;
    [BoxGroup("Objects")][SerializeField] 
    private Character lastSelectedCharacter;
    // [BoxGroup("Objects")][SerializeField] private Character selectedEnemy;
    
    [BoxGroup("UI")]
    public GameObject HitChanceUIGameObject;
    [BoxGroup("UI")]
    public TextMeshProUGUI HitChanceText;
    
    [BoxGroup("Free Roam")][SerializeField]
    public LayerMask freeRoamMask;
    [BoxGroup("Free Roam")][SerializeField]
    //public Transform freeRoamTarget;
    IAstarAI[] ais;
    
    List<Tile> reachableTiles = new List<Tile>();
    [SerializeField]List<Tile> attackableTiles = new List<Tile>();
    [SerializeField]LayerMask interactMask;
    [SerializeField]LayerMask UILayerMask;
    public Action<Tile> CurrentTileChangedAction;
    public Action<Tile, float> CharacterSelectedAction;
    //Debug purposes only
    [SerializeField]
    bool debug;
    private Tile lastTile2;
    Path Lastpath;
    [Tooltip("This is a null check for selectedCharacter")]
    public bool characterSelected = false;
    public SkillContainer selectedCharacterSkillContainer;
    [SerializeField] public bool isMouseOverUI = false;
    [SerializeField] private static float intensity = 1;
    public bool isPaused = false;
    /// <summary>
    /// <param name="0">White -- Default</param>
    /// <param name="1">Green -- Player</param>
    /// <param name="2">Red -- Enemy</param>
    /// <param name="3">Blue -- Cover</param>
    /// </summary>
    Dictionary<int, Color> tileInspectColors = new Dictionary<int, Color>()
    {
        {0, new Color(12, 12, 12f, 0.7176471f) },//White -- Default
        {1, new Color(0f, 12.99604f, 0.9965293f, 0.7176471f) },//Green -- Player
        {2, new Color(12.99604f, 0.3817299f, 0, 0.7176471f) },//Red -- Enemy
        {3, new Color(0.000f, 6.334f, 12.996f, 0.719f) }, //Blue -- Cover
        {4, new Color(12.99604f, 12.99604f, 0, 0.7176471f) },//Yellow -- Interactable
        
        //old
        /*{0, new Color(0.000f, 6.334f, 12.996f, 0.719f) }, //Blue -- Default
        {1, new Color(0f, 12.99604f, 0.9965293f, 0.7176471f) },//Green -- Player
        {2, new Color(12.99604f, 0.3817299f, 0, 0.7176471f) },//Red -- Enemy
        {3, new Color(12.99604f, 12.99604f, 0, 0.7176471f) },//Yellow -- Cover*/
    };
    
    /// <summary>
    /// <param name="0">Green -- Movable</param>
    /// <param name="1">Orange -- Attackable</param>
    /// <param name="2">NOT IMPLEMENTED</param>
    /// <param name="3">Blue -- Cover</param>
    /// <param name="4">Yellow -- Interactable</param>
    /// </summary>
   public Dictionary<int, Color> tileHighligthColors = new Dictionary<int, Color>()
   {
       {0, new Color(0.3f, 6f, 0.3f, 0.7176471f) },//Green -- Movable
       {1, new Color(12.99604f, 1.903436f, 0f, 0.7176471f) },//Orange -- Attackable
       
       {3, new Color(0.000f, 3.334f, 6.996f, 0.7176471f) }, //Blue -- Cover
       {4, new Color(12.99604f, 6.903436f, 0f, 0.7176471f) },//Yellow -- Interactable
       //old
       /*{0, new Color(1, 1, 1, 0.7176471f) }, //white -- Movable
       {1, new Color(12.99604f, 1.903436f, 0f, 0.7176471f) },//Orange -- Attackable
       {2, new Color(0.3f, 0.8f, 0.3f, 0.4f) },//Green -- Movable
       {3, new Color(1f, 0.1f, 0.1f, 0.9f) },//Red -- Not selectable*/
       
   };
    
    public Dictionary<int, Color> tileHighligthNormalColors = new Dictionary<int, Color>()
    {
        {0, new Color(0.3f * 1.2f, 1f * 1.2f, 0.3f * 1.2f, 0.7176471f) },//Green -- Movable
        {1, new Color(1f * 1.5f, 0.3f * 1.5f, 0f, 0.7176471f) },//Orange -- Attackable
       
        {3, new Color(0.000f, 0.6f * 1.5f, 1f * 1.5f, 0.7176471f) }, //Blue -- Cover
        {4, new Color(1f, 1f, 0f, 0.7176471f) },//Yellow -- Interactable
        {5, new Color(1f, 1f, 1f, 0.0f) },//White -- Empty
        
        
       
    };
   
   public Dictionary<int, Color> skillColors = new Dictionary<int, Color>()
   {
       {0, new Color(0.9921569f, 0.7359219f, 0.5176471f, 1) },//Red -- Attack
       {1, new Color(0.525716f, 0.9921569f, 0.5176471f, 1) },//Green -- Buff
   };

    private void OnEnable()
    {
        SkillSelected += Clear;
        SkillSelected += ClearHighlightReachableTiles;
        SkillSelected += () => EnableMovement(false);
        SkillDeselected += () => EnableMovement(true);
        CurrentTileChangedAction += CurrentTileChangedFunc;
    }

    private void OnDisable()
    {
        SkillSelected -= Clear;
        SkillSelected -= ClearHighlightReachableTiles;
        SkillSelected -= () => EnableMovement(false);
        SkillDeselected -= () => EnableMovement(true);
        CurrentTileChangedAction += CurrentTileChangedFunc;
    }

    private TurnSystem TurnSystem;
    private LevelManager LevelManager;
    private GameManager GameManager;
   
    private void Awake()
    {
        TurnSystem = everythingUseful.TurnSystem;
        LevelManager = everythingUseful.LevelManager;
        GameManager = everythingUseful.GameManager;
    }

    private void Start()
    {
        inspectTileMeshRenderer = InspectTileGO.GetComponentInChildren<MeshRenderer>();
        
        ais = FindObjectsOfType<MonoBehaviour>().OfType<IAstarAI>().ToArray();
    }

    private void Update()
    {
        //Clear();
        MouseUpdate();
        CheckMouseOverUI();
        CheckCharacterInputs();
        CheckDebugInputs();
        //UpdateFreeRoamTargetPosition();
    }

    #region Inputs

    private void CheckDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TurnSystem.NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            TurnSystem.NextRound();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if(characterSelected)
            {
                selectedCharacter.remainingActionPoints = 10;

                foreach (var skill in selectedCharacter.SkillContainer.skillsList)
                {
                    skill.skillButton.EnableDisableButton(true);
                    
                    skill.skillCooldown = 0;
                    skill.actionPointUse = 0;
                    skill.accuracy = 100;
                    selectedCharacter.SkillContainer.damageBeforeCoverDebuff = 100;
                }
            }
            
            
            /*foreach (var player in TurnSystem.Instance.players)
            {
                player.remainingActionPoints = 99999;
            }*/
        }
    }
    
    public void CheckCharacterInputs()
    {
        if (characterSelected) //eselect character //todo skill selectedi variable olarak tut
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (selectedCharacterSkillContainer.skillSelected == false)
                {
                    if (currentTile.isCoveredByCoverPoint) InspectTileCoverIconGO.SetActive(false);
                    selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false);
                    selectedCharacter.outline.enabled = false;
                    characterSelected = false;
                    ClearHighlightReachableTiles();
                    // selectedCharacter.pathfinder.EnableIllustratePath(false);
                    selectedCharacter.pathfinder.ClearIllustratedPath();
                    selectedCharacterSkillContainer.skillSelected = false;
                    selectedCharacterSkillContainer = null;
                    selectedCharacter = null;
    
                    TooltipSystem.Hide();
                }
                else //deselect skill
                {
                    selectedCharacterSkillContainer.DeselectSkill(selectedCharacterSkillContainer.selectedSkill);
                    EnableMovement(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedCharacterSkillContainer.TrySelectSkill(selectedCharacterSkillContainer.skillsList[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedCharacterSkillContainer.TrySelectSkill(selectedCharacterSkillContainer.skillsList[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedCharacterSkillContainer.TrySelectSkill(selectedCharacterSkillContainer.skillsList[2]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedCharacterSkillContainer.TrySelectSkill(selectedCharacterSkillContainer.skillsList[3]);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (selectedCharacter.inventory.GetSpawnedInventoryUIScript().GetSkipButton().interactable && selectedCharacter.inventory.GetSpawnedInventoryUIScript().GetSkipButton().gameObject.activeSelf)
                {
                    selectedCharacter.inventory.GetSpawnedInventoryUIScript().GetSkipButton().onClick.Invoke();
                }
            }
        }
        else
        { 
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(LevelManager.players.Count > 0 && LevelManager.players[0].isUnlocked) TrySelectPlayer(LevelManager.players[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(LevelManager.players.Count > 1 && LevelManager.players[1].isUnlocked) TrySelectPlayer(LevelManager.players[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if(LevelManager.players.Count > 2 && LevelManager.players[2].isUnlocked) TrySelectPlayer(LevelManager.players[2]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if(LevelManager.players.Count > 3 && LevelManager.players[3].isUnlocked) TrySelectPlayer(LevelManager.players[3]);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;
                GameManager.PauseGame(isPaused);
            }
        }
    }

    #endregion

    #region Mouse Update

    private void CheckMouseOverUI()
    {
        //check if mouse is over ui but exclude some game objects
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject(); //is this expensive
    }
    
    private void MouseUpdate()
    {
        if ((!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask) || isMouseOverUI) || isPaused)
            return;

        // if(lastTile != null) lastTile.ClearHighlight();
       
        lastTile2 = currentTile;
       
       
        currentTile = hit.transform.GetComponent<Tile>();

        {
            if(lastTile2 != currentTile)
            {
                lastTile = lastTile2;
                CurrentTileChangedAction?.Invoke(currentTile);//check if the currentTile value has changed
            }
            
        }
        
        /*characterUnderMouse = currentTile.occupyingCharacter;*/
        //InspectTileHighlight();
        InspectTile();
    }

    #endregion

    #region Inspect / Select Player

    private void InspectTileHighlight()
    {
        //make the this method work every 15 frames
        /*if (Time.frameCount % 15 != 0)
            return;*/
        
        /*if (InspectTileGO.transform.position != currentTile.transform.position)
        {
            CurrentTileChangedAction?.Invoke();
        }*/
        
        InspectTileGO.transform.position = currentTile.transform.position;
        InspectTileCoverIconGO.SetActive(false);

        if (currentTile.Occupied)
        {
            if (characterSelected)
            {
                selectedCharacter.pathfinder.ClearIllustratedPath();
            }
            
            if (currentTile.OccupiedByCoverPoint)
            {
                inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[0]);
            }
            if (currentTile.occupiedByInteractable)
            {
                inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[4]);
            }

            if (currentTile.occupyingCharacter)
            {
                if (characterUnderMouse.gameObject.tag == "Player")
                {
                    // inspectTileMeshRenderer.material.color = tileInspectColors[1];
                    inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[1]);
                
                }
                else if (characterUnderMouse.gameObject.tag == "Enemy")
                {
                    // inspectTileMeshRenderer.material.color = tileInspectColors[2];
                    inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[2]);
                }
            }
        }
        else
        {
            // inspectTileMeshRenderer.material.color = tileInspectColors[0];
            inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[0]);

            if (currentTile.isCoveredByCoverPoint)
            {
                inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[3]);

                if (reachableTiles.Contains(currentTile) && characterSelected && (selectedCharacter.characterState == Character.CharacterState.Idle || selectedCharacter.characterState == Character.CharacterState.WaitingTurn))
                {
                    InspectTileCoverIconGO.SetActive(true);
                }

            }
            // Debug.Log(inspectTileMeshRenderer.material.GetColor("_RimColor"));
        }
    }
    private void InspectTile()
    {
        //Alter cost by right clicking
        /*if (Input.GetMouseButtonUp(1))
        {
            currentTile.ModifyCost();
            return;
        }*/

        if (characterSelected == false)
        {
            if (currentTile.Occupied)
            {
                InspectSomeone();
            }
        }
        else
        {
            switch (selectedCharacter.characterState)
            {
                case Character.CharacterState.Idle:
                    if (currentTile.Occupied)
                    {
                        InspectSomeone();
                    }
                    else
                    {
                        NavigateToTile(selectedCharacter);
                    }
                    break;
           
                case Character.CharacterState.WaitingTurn:
                    if (currentTile.Occupied)
                    {
                        InspectSomeone();
                    }
                    else
                    {
                        NavigateToTile(selectedCharacter);
                    }
                    break;
                case Character.CharacterState.WaitingNextRound:
                    if (currentTile.Occupied)
                    {
                        InspectSomeone();
                    }
                    /*else
                    {
                        NavigateToTile();
                    }*/
                    break;
                //TODO do it in a better way; get the selected tile then do the logic there (range, accuracy damage)
                case Character.CharacterState.Attacking:
                    
                    if (selectedCharacter.canAttack)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (attackableTiles.Contains(currentTile))
                            {
                                selectedCharacter.SkillContainer.UseSkill(selectedCharacter.SkillContainer.selectedSkill, currentTile);
                                lastAttackedTile = currentTile;
                            
                            }
                        }
                    }
                    break;
                
                case Character.CharacterState.Moving:
                    if (currentTile.Occupied)
                    {
                        InspectSomeone();
                    }
                    break;
            }
        }
            
    }

    private void InspectSomeone()
    {
        if (currentTile.OccupiedByCoverPoint)return;

        if (currentTile.occupiedByInteractable)
        {
            InspectInteractable(currentTile);
            return;
        }
        
        if (currentTile.occupyingCharacter.gameObject.tag == "Player")
        {
            InspectCharacter();
        }
        /*else if (currentTile.occupyingCharacter.gameObject.tag == "Enemy")
        {
            InspectEnemy();
        }*/
        
    }

    private void InspectInteractable(Tile tile)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RetrievePath(selectedCharacter, out Path newPath))
            {
                // newPath.tiles.Remove(newPath.tiles[newPath.tiles.Count - 1]);
                Clear();
                ClearHighlightReachableTiles();
                selectedCharacter.StartMove(newPath, true, () =>
                {
                    selectedCharacter.canMove = false;
                    selectedCharacter.Rotate(tile.transform.position, 0.75f, () =>
                    {
                        tile.occupyingInteractable.OnInteract(selectedCharacter);
                        selectedCharacter.canMove = true;
                    });
                });
            }
        }
            
    }
    
    private void InspectCharacter()
    {
        if (currentTile.occupyingCharacter.Moving)
            return;
       
        //currentTile.Highlight(Color.white);

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectPlayer(currentTile.occupyingCharacter);
        }
            
    }
    
    public void TrySelectPlayer(Character chaaracter)
    {
        Clear();
        //CharacterSelectedAction?.Invoke(chaaracter.characterTile);
        if (characterSelected == false)
        {
            SelectPlayer(chaaracter);
            characterSelected = true;
            selectedCharacterSkillContainer = selectedCharacter.SkillContainer;
        }
        else
        {
            switch (lastSelectedCharacter.characterState)
            {
                case Character.CharacterState.Idle:
                    lastSelectedCharacter.inventory.ShowSkillsUI(false); //close skills ui
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.SkillContainer;
                    break;
           
                case Character.CharacterState.WaitingTurn:
                    lastSelectedCharacter.inventory.ShowSkillsUI(false); //close skills ui
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.SkillContainer;
                    break;
                
                case Character.CharacterState.WaitingNextRound:
                    lastSelectedCharacter.inventory.ShowSkillsUI(false); //close skills ui
                    //lastSelectedCharacter.GetComponent<InventoryUI>().SetSkipTurnButtonInteractable(false); //disable skip turn button
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.SkillContainer;
                    break;
           
                case Character.CharacterState.Attacking:
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    //selectedCharacter = lastSelectedCharacter;
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    break;
                
                case Character.CharacterState.Moving:
                    lastSelectedCharacter.inventory.ShowSkillsUI(false); //close skills ui
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.SkillContainer;
                    break;
               
            }
        }
        
    }

    private void SelectPlayer(Character character)
    {
        if (lastSelectedCharacter != null) lastSelectedCharacter.outline.enabled = false;
        
        selectedCharacter = character; //select character
        characterSelected = true;
        selectedCharacter.outline.enabled = true;
        
        lastSelectedCharacter = selectedCharacter;
        if (selectedCharacter.characterState == Character.CharacterState.WaitingTurn || selectedCharacter.characterState == Character.CharacterState.Idle)
        {
            if (selectedCharacter.remainingActionPoints > 0)
            {
                EnableMovement(true);
            }
            HighlightReachableTiles(); //highlight tiles within range
        }
        selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(true); //show skills ui
    }

    /*public void InspectEnemy()
    {
        //currentTile.HighlightRed();

        if (Input.GetMouseButtonDown(0))
            SelectEnemy();
    }*/
    /*private void SelectEnemy()
    {
        selectedEnemy = currentTile.occupyingCharacter;
       // CharacterSelectedAction?.Invoke(selectedEnemy.characterTile);
    }*/
    
    #endregion

    #region Tile

    private void NavigateToTile(Character activator )
    {
        if (selectedCharacter == null || selectedCharacter.Moving == true)
            return;
    
        if (RetrievePath(activator, out Path newPath))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Clear();
                ClearHighlightReachableTiles();
                // GetComponent<AudioSource>().PlayOneShot(click);
                selectedCharacter.StartMove(newPath);
                /*foreach (var VARIABLE in newPath.tiles)
                {
                    VARIABLE.Highlight(Color.white);
                }*/
                lastSelectedCharacter = selectedCharacter;
                //selectedCharacter = null;
            }
        }
    }

    private bool RetrievePath(Character activator, out Path path)
    {
        if (selectedCharacter != null)
        {
            if (selectedCharacter.pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingActionPoints).Contains(currentTile))
            {
                path = selectedCharacter.pathfinder.FindPath(activator, selectedCharacter.characterTile, currentTile);
                selectedCharacter.pathfinder.illustrator.IllustratePath(path);
                return true;
            }
            else
            {
                if (currentTile.occupiedByInteractable)//todo burda kaldın
                {
                    path = selectedCharacter.pathfinder.FindPath(activator, selectedCharacter.characterTile, currentTile);
                    List<Tile> tiles = path.tiles ;
                    
                    selectedCharacter.pathfinder.illustrator.IllustratePath(path);
                    return true;
                }
                else
                {
                    path = null;
                    return false;
                }

                
                path = null;
                return false;
            }


            if (path == null || path == Lastpath)
                return false;
        }
        else
        {
            path = null;
            return false;
        }
            
   
    }
    private void Clear()
    {
        if (characterSelected)
        {
            selectedCharacter.pathfinder.ClearIllustratedPath();
            if (currentTile == null  || currentTile.Occupied == false)
                return;

            //currentTile.ModifyCost(currentTile.terrainCost-1);//Reverses to previous cost and color after being highlighted
            //currentTile.ClearHighlight();
            //currentTile = null;
        }
    }
    
    public void ClearHighlightReachableTiles()
    {
        foreach (Tile tile in reachableTiles)
        {
            tile.ClearHighlight();
        }
    }
    public void HighlightReachableTiles()
    {
        ClearHighlightReachableTiles();
        if (characterSelected)
        {
            reachableTiles = selectedCharacter.pathfinder.GetReachableTiles(selectedCharacter.characterTile,
                selectedCharacter.remainingActionPoints /*, selectedCharacter.characterTile*/);
            foreach (Tile tile in reachableTiles)
            {
                tile.HighlightMoveable();

                if (tile == selectedCharacter.characterTile)
                {
                    tile.ClearHighlight();
                }
            }
            
            selectedCharacter.characterTile.HighlightMoveable();
        }
    }

    private List<Tile> effectedTiles;
    private List<Tile> innerEffectedTiles;
    private List<Tile> outerEffectedTiles;
    public void HighlightAttackableTiles(SkillContainer.Skills selectedSkill)
    { 
        if (!selectedCharacter.isInAttackTileSelection) return; // the attacking tiles were able to be changed while anim was playing so i added this
        ClearHighlightAttackableTiles();
        
        switch (selectedSkill.skillData.skillTargetType)
        {
            case SkillsData.SkillTargetType.AreaAroundSelf: //default
                attackableTiles = selectedCharacter.pathfinder.GetAttackableTiles(selectedCharacter.characterTile, selectedCharacter.SkillContainer.selectedSkill/*, selectedCharacter.characterTile*/);
                foreach (Tile tile in attackableTiles)
                {
                    tile.HighlightAttackable();

                    if (tile == selectedCharacter.characterTile)
                    {
                        tile.ClearHighlight();
                    }
                }
                break;
            
            case SkillsData.SkillTargetType.AreaAroundTarget:
                
                //attackableTiles = selectedCharacter.pathfinder.GetAttackableTiles(selectedCharacter.characterTile, selectedCharacter.SkillContainer.selectedSkill/*, selectedCharacter.characterTile*/);
                attackableTiles = selectedCharacter.pathfinder.GetAttackableTilesArea(selectedCharacter.characterTile, selectedCharacter.SkillContainer.selectedSkill, 
                    currentTile, out effectedTiles, out innerEffectedTiles, out outerEffectedTiles);
                
                foreach (Tile tile in attackableTiles)
                {
                    tile.HighlightAttackable();

                    if (tile == selectedCharacter.characterTile)
                    {
                        tile.ClearHighlight();
                    }
                }
                
                // other than the origin tile keep effected tiles around the target in a list
                selectedCharacter.SkillContainer.effectedTiles = effectedTiles;
                selectedCharacter.SkillContainer.innerEffectedTiles = innerEffectedTiles;
                selectedCharacter.SkillContainer.outerEffectedTiles = outerEffectedTiles;
                
                //highlight effected tiles if they are in attack range
                if (attackableTiles.Contains(currentTile))
                {
                    foreach (Tile tile in effectedTiles)
                    {
                        tile.TileHighlightGO.SetActive(true);
                        tile.TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[1]);
                    }
                }
                
                break;
            
            case SkillsData.SkillTargetType.Line:
                if (currentTile != null && currentTile.selectable)
                {
                    attackableTiles = selectedCharacter.pathfinder.GetAttackableTilesLine(selectedCharacter, selectedCharacter.characterTile, currentTile, selectedCharacter.SkillContainer.selectedSkill);
                    if (attackableTiles != null)
                    {
                        foreach (Tile tile in attackableTiles)
                        {
                            tile.HighlightAttackable();

                            if (tile == selectedCharacter.characterTile)
                            {
                                tile.ClearHighlight();
                            }
                        }
                    }
                }
                
                break;
            
            case SkillsData.SkillTargetType.Cone:
                throw new NotImplementedException();
                break;
            
            case SkillsData.SkillTargetType.Cleave:
                
                attackableTiles = selectedCharacter.pathfinder.GetAttackableTilesCleave(selectedCharacter.characterTile, selectedCharacter.SkillContainer.selectedSkill, currentTile, 
                    selectedSkill.skillData.cleaveStartLeft, selectedSkill.skillData.cleaveTimes, out effectedTiles);
                
                foreach (Tile tile in attackableTiles)
                {
                    tile.HighlightAttackable();

                    if (tile == selectedCharacter.characterTile)
                    {
                        tile.ClearHighlight();
                    }
                }
                selectedCharacter.SkillContainer.effectedTiles = effectedTiles;
                //highlight effected tiles if they are in attack range
                if (attackableTiles.Contains(currentTile))
                {
                    foreach (Tile tile in effectedTiles)
                    {
                        tile.TileHighlightGO.SetActive(true);
                        tile.TileHighlightMeshRenderer.material.SetColor("_RimColor", everythingUseful.Interact.tileHighligthColors[1]);
                    }
                }

                break;
        }
        
        
    }
    public void ClearHighlightAttackableTiles()
    {
        foreach (Tile tile in attackableTiles)
        {
            tile.ClearHighlight();
        }
        
        if (effectedTiles != null)
        {
            foreach (Tile tile in effectedTiles)
            {
                tile.ClearHighlight();
            }
        }
    }

    private void CurrentTileChangedFunc(Tile newTile)//this calculates the accuracy before shooting, adds rotation to player
    {
        characterUnderMouse = currentTile.occupyingCharacter;
        
        //print(newTile);
        if (characterSelected && selectedCharacter.characterState == Character.CharacterState.Attacking)//todo add or FreeRoam
        {
            selectedCharacter.SkillContainer.ResetCoverAccruacyDebuff();
            selectedCharacter.SkillContainer.ResetCoverdamageDebuff();
            if (isMouseOverUI == false)
            {
                selectedCharacter.Rotate(selectedCharacter.characterTile != currentTile ? currentTile.transform.position : lastTile.transform.position);
            }
            
            HighlightAttackableTiles(selectedCharacter.SkillContainer.selectedSkill);
            
            if (newTile.occupiedByEnemy && attackableTiles.Contains(newTile))
            {
                if (selectedCharacter.pathfinder.CheckCoverPoint(selectedCharacter.characterTile, newTile, true))
                {
                    if (selectedCharacter.SkillContainer.selectedSkill.skillData.skillType == SkillsData.SkillType.Ranged)
                    {
                        // selectedCharacter.SkillContainer.ApplyCoverAccuracyDebuff();
                        selectedCharacter.SkillContainer.ApplyCoverDamageDebuff();
                    }
                    
                    EnableDisableHitChanceUI(true);

                }
                else
                {
                    if (selectedCharacter.SkillContainer.selectedSkill.skillData.skillType == SkillsData.SkillType.Ranged)
                    {
                        // selectedCharacter.SkillContainer.ResetCoverAccruacyDebuff();
                        selectedCharacter.SkillContainer.ResetCoverdamageDebuff();
                    }
                    
                    EnableDisableHitChanceUI(true);

                }
            }
            else
            {
                if (selectedCharacter.SkillContainer.selectedSkill.skillData.skillType == SkillsData.SkillType.Ranged)
                {
                    // selectedCharacter.SkillContainer.ResetCoverAccruacyDebuff();
                    selectedCharacter.SkillContainer.ResetCoverdamageDebuff();
                }
                
                EnableDisableHitChanceUI(false);

            }
            
        }

        if (newTile.occupiedByEnemy)
        {
            newTile.occupyingEnemy.outline.enabled = true;
            lastCharacterUnderMouse = newTile.occupyingEnemy;
        }
        else
        {
            if (lastCharacterUnderMouse != null)
            {
                lastCharacterUnderMouse.outline.enabled = false;
                lastCharacterUnderMouse = null;
            }
        }
        
        InspectTileHighlight();
    }

    public List<Tile> GetReachableTiles()
    {
        return reachableTiles;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }
    public Tile GetLastTile()
    {
        return lastTile;
    }
    public void EnableAttackableTiles(bool value)
    {
        if (selectedCharacter.characterState == Character.CharacterState.Attacking || selectedCharacter.characterState == Character.CharacterState.Idle)
        {
            if (characterSelected)
            {
                if (value)
                {
                    if (selectedCharacter.SkillContainer.skillSelected == true)
                    {
                        //selectedCharacter.canAttack = value;//todo dont change this value, instead block all interactions when player hovers over skills, tooltips etc
                        HighlightAttackableTiles(selectedCharacter.SkillContainer.selectedSkill);
                    }
                }
                else
                {
                    if (selectedCharacter.SkillContainer.skillSelected == true)
                    {
                        //selectedCharacter.canAttack = value;
                        ClearHighlightAttackableTiles();
                    }
                }
            }
        }
    }

    #endregion
    public IEnumerator HitChanceUIToMousePos()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            HitChanceUIGameObject.transform.position = Input.mousePosition;
        }
    }

    public void EnableDisableHitChanceUI(bool value)
    {

        if (value)
        {
            StartCoroutine(HitChanceUIToMousePos()); 
            HitChanceText.text = selectedCharacter.SkillContainer.selectedSkill.accuracy.ToString() + "%";
        }
        else
        {
            StopCoroutine(HitChanceUIToMousePos()); 
        }
        HitChanceUIGameObject.SetActive(value);
    }
    
    
     public void EnableMovement(bool value)
    {
        StartCoroutine(EnableMovementDelay(value));
    }

    private IEnumerator EnableMovementDelay(bool value)
    {
        yield return new WaitForEndOfFrame();
        if (selectedCharacter.characterState == Character.CharacterState.WaitingTurn || selectedCharacter.characterState == Character.CharacterState.Idle)
        {
            if (characterSelected)
            {
                if (value == true)
                {
                    if (selectedCharacter.SkillContainer.skillSelected == false)
                    {
                        selectedCharacter.canMove = value;
                        HighlightReachableTiles();
                        selectedCharacter.pathfinder.EnableIllustratePath(value);
                    }
                }
                else
                {
                    if (selectedCharacter.SkillContainer.skillSelected == false)
                    {
                        selectedCharacter.canMove = value;
                        Clear();
                        ClearHighlightReachableTiles();
                        selectedCharacter.pathfinder.EnableIllustratePath(value);
                    }
                
                }
            }
        }
    }

    public void ResetToDefault()
    {
        Clear();
        if (currentTile != null && currentTile.isCoveredByCoverPoint) InspectTileCoverIconGO.SetActive(false);
        if (selectedCharacter != null)
        {
            selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false);
            selectedCharacter.outline.enabled = false;
            ClearHighlightReachableTiles();
            // selectedCharacter.pathfinder.EnableIllustratePath(false);
            selectedCharacter.pathfinder.ClearIllustratedPath();
            selectedCharacterSkillContainer.skillSelected = false;
            selectedCharacterSkillContainer = null;
        }

        selectedCharacter = null;
        characterSelected = false;
        TooltipSystem.Hide();
        lastSelectedCharacter = null;
        reachableTiles.Clear();
    }
    /*public void UpdateFreeRoamTargetPosition()
    {
        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn && characterSelected && Input.GetMouseButtonDown(0))
        {
            Vector3 newPosition = Vector3.zero;
            bool positionFound = false;

            // Fire a ray through the scene at the mouse position and place the target where it hits
            RaycastHit hit;
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, freeRoamMask))
            {
                newPosition = hit.point;
                positionFound = true;
            }

            if (positionFound && newPosition != freeRoamTarget.position)
            {
                freeRoamTarget.position = newPosition;
            }
        }
    }*/
    /*//Debug only
    void ClearLastPath()
    {
        if (Lastpath == null)
            return;

        foreach (Tile tile in Lastpath.tiles)
        {
            tile.ClearText();
        }
    }

    //Debug only
    void DebugNewPath(Path path)
    {
        foreach (Tile t in path.tiles)
        {
            t.DebugCostText();
        }
    }*/
}