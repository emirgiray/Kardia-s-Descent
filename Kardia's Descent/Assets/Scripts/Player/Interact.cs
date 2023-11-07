using System;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Examples;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Interact : MonoBehaviour
{
    public static Interact Instance { get; private set; }
    public Action SkillHighlighted;
    public Action SkillSelected;
    public Action SkillDeselected;
    [BoxGroup("Objects")][SerializeField] private GameObject InspectTileGO;
    MeshRenderer inspectTileMeshRenderer;
    [BoxGroup("Objects")][SerializeField] public Camera mainCam;
    [BoxGroup("Objects")][SerializeField] public Tile currentTile;
    [BoxGroup("Objects")][SerializeField] Tile lastTile;
    [BoxGroup("Objects")][SerializeField] public Character characterUnderMouse;
    [BoxGroup("Objects")][SerializeField] public Character selectedCharacter;
    [BoxGroup("Objects")][SerializeField] public Character lastSelectedCharacter;
    [BoxGroup("Objects")][SerializeField] public Character selectedEnemy;
    
    List<Tile> reachableTiles = new List<Tile>();
    List<Tile> attackableTiles = new List<Tile>();
    [SerializeField]
    AudioClip click, pop;
    [SerializeField]LayerMask interactMask;
    [SerializeField]LayerMask UILayerMask;
    

    //Debug purposes only
    [SerializeField]
    bool debug;
    
    Path Lastpath;
    Pathfinder pathfinder;
    [Tooltip("This is a null check for selectedCharacter")]
    bool characterSelected = false;
    [SerializeField] bool isMouseOverUI = false;
    
    Dictionary<int, Color> tileInspectColors = new Dictionary<int, Color>()
    {
        {0, new Color(0f, 0.38f, 1f, 0.3f) }, //Blue -- Default
        {1, new Color(0f, 1, 0, 0.3f) },//Green -- Player
        {2, new Color(1f, 0, 0, 0.3f) },//Red -- Enemy
        {3, new Color(1f, 1, 0, 0.3f) },//Yellow -- Cover

    };

    private void OnEnable()
    {
        SkillSelected += Clear;
        SkillSelected += ClearHighlightReachableTiles;
        SkillSelected += () => EnableMovement(false);
        SkillDeselected += () => EnableMovement(true);
    }

    private void OnDisable()
    {
        SkillSelected -= Clear;
        SkillSelected -= ClearHighlightReachableTiles;
        SkillSelected -= () => EnableMovement(false);
        SkillDeselected -= () => EnableMovement(true);
    }

    public void EnableMovement(bool value)
    {
        if (selectedCharacter.characterState == Character.CharacterState.WaitingTurn || selectedCharacter.characterState == Character.CharacterState.Idle)
        {
            if (characterSelected)
            {
                if (value == true)
                {
                    if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == false)
                    {
                        selectedCharacter.canMove = value;
                        HighlightReachableTiles();
                        pathfinder.EnableIllustratePath(value);
                    }
                }
                else
                {
                    if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == false)
                    {
                        selectedCharacter.canMove = value;
                        Clear();
                        ClearHighlightReachableTiles();
                        pathfinder.EnableIllustratePath(value);
                    }
                
                }
            }
        }
    }

    public void EnableAttackableTiles(bool value)
    {
        if (selectedCharacter.characterState == Character.CharacterState.Attacking || selectedCharacter.characterState == Character.CharacterState.Idle)
        {
            if (characterSelected)
            {
                if (value)
                {
                    if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == true)
                    {
                        //selectedCharacter.canAttack = value;//todo dont change this value, instead block all interactions when player hovers over skills, tooltips etc
                        HighlightAttackableTiles();
                    }
                }
                else
                {
                    if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == true)
                    {
                        //selectedCharacter.canAttack = value;
                        ClearHighlightAttackableTiles();
                    }
                }
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();
        inspectTileMeshRenderer = InspectTileGO.GetComponentInChildren<MeshRenderer>();
        if (pathfinder == null)
            pathfinder = Pathfinder.Instance;
    }

    private void Update()
    {
        Clear();
        MouseUpdate();
        CheckMouseOverUI();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ( characterSelected)//deselect character //todo skill selectedi variable olarak tut
            {
                if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == false)
                {
                    selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false);
                    selectedCharacter = null;
                    characterSelected = false;
                    ClearHighlightReachableTiles();
                    pathfinder.EnableIllustratePath(false);
                }
                else//deselect skill
                {
                    selectedCharacter.GetComponent<SkillContainer>().DeslectSkill(selectedCharacter.GetComponent<SkillContainer>().selectedSkill);
                    EnableMovement(true);
                }
            }
            
        }
        
    }

    public void CheckMouseOverUI()
    {
        //check if mouse is over ui
        if (EventSystem.current.IsPointerOverGameObject())//is this expensive
        {
            isMouseOverUI = true;
        }
        else
        {
            isMouseOverUI = false;
        }
    }
    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask) || isMouseOverUI)
            return;

       // if(lastTile != null) lastTile.ClearHighlight();
        currentTile = hit.transform.GetComponent<Tile>();
        lastTile = currentTile;
        characterUnderMouse = currentTile.occupyingCharacter;
        InspectTileHighlight();
        InspectTile();
    }

    public void InspectTileHighlight()
    {
        //make the this method work every 15 frames
        if (Time.frameCount % 15 != 0)
            return;

        InspectTileGO.transform.position = currentTile.transform.position;
        if (currentTile.Occupied)
        {
            if (currentTile.OccupiedByCoverPoint)
            {
                inspectTileMeshRenderer.material.color = tileInspectColors[3];
                return;
            }
            
            if (characterUnderMouse.gameObject.tag == "Player")
            {
                inspectTileMeshRenderer.material.color = tileInspectColors[1];
            }
            else if (characterUnderMouse.gameObject.tag == "Enemy")
            {
                inspectTileMeshRenderer.material.color = tileInspectColors[2];
            }
        }
        else
        {
            inspectTileMeshRenderer.material.color = tileInspectColors[0];
        }
    }
    private void InspectTile()
    {
        //Alter cost by right clicking
        if (Input.GetMouseButtonUp(1))
        {
            currentTile.ModifyCost();
            return;
        }

        if (characterSelected == false)
        {
            if (currentTile.Occupied)
            {
                InspectSomeone();
            }
        }
        else
        {
            switch (selectedCharacter.GetComponent<Character>().characterState)
            {
                case Character.CharacterState.Idle:
                    break;
           
                case Character.CharacterState.WaitingTurn:
                    if (currentTile.Occupied)
                    {
                        InspectSomeone();
                    }
                    else
                    {
                        NavigateToTile();
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
                    if (Input.GetMouseButtonDown(0) && selectedCharacter.canAttack)
                    {
                        if (attackableTiles.Contains(currentTile))
                        {
                            selectedCharacter.GetComponent<SkillContainer>().UseSkill(selectedCharacter.GetComponent<SkillContainer>().selectedSkill, currentTile);
                            
                        }
                    }
                    break;
               
            }
        }
        
        
        /*
        if (currentTile.Occupied)
        {
            if (currentTile.occupyingCharacter.gameObject.tag == "Player")
            {
                InspectCharacter();
            }
            else if (currentTile.occupyingCharacter.gameObject.tag == "Enemy")
            {
                InspectEnemy();
            }

            //if (selectedCharacter.characterState )
            {
                
            }
        }
        else
        {
            //currentTile.Highlight(new Color(0,0,1,0.5f));
            if (characterSelected)
            {
                switch (selectedCharacter.GetComponent<Character>().characterState)
                {
                    case Character.CharacterState.Idle:
                        break;
           
                    case Character.CharacterState.WaitingTurn:
                        NavigateToTile();
                        break;
 
                    case Character.CharacterState.Attacking:
                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedCharacter.GetComponent<SkillContainer>().UseSkill(selectedCharacter.GetComponent<SkillContainer>().selectedSkill, currentTile);
                        }
                        break;
               
                }
            }
            
        }
        */
            
    }

    public void InspectSomeone()
    {
        if (currentTile.OccupiedByCoverPoint)return;
        
        if (currentTile.occupyingCharacter.gameObject.tag == "Player")
        {
            InspectCharacter();
        }
        else if (currentTile.occupyingCharacter.gameObject.tag == "Enemy")
        {
            InspectEnemy();
        }
    }

    private void InspectCharacter()
    {
        if (currentTile.occupyingCharacter.Moving)
            return;

        //currentTile.Highlight(Color.white);

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectPlayer();
        }
            
    }
    
   
    private void TrySelectPlayer()
    {
        if (characterSelected == false)
        {
            SelectPlayer();
            characterSelected = true;
        }
        else
        {
            switch (lastSelectedCharacter.GetComponent<Character>().characterState)
            {
                case Character.CharacterState.Idle:
                    break;
           
                case Character.CharacterState.WaitingTurn:
                    lastSelectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false); //close skills ui
                    SelectPlayer();
                    break;
                
                case Character.CharacterState.WaitingNextRound:
                    lastSelectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false); //close skills ui
                    //lastSelectedCharacter.GetComponent<InventoryUI>().SetSkipTurnButtonInteractable(false); //disable skip turn button
                    SelectPlayer();
                    break;
           
                case Character.CharacterState.Attacking:
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    //selectedCharacter = lastSelectedCharacter;
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    break;
               
            }
        }
        
    }

    public void SelectPlayer()
    {
        selectedCharacter = currentTile.occupyingCharacter; //select character
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
    
    public void InspectEnemy()
    {
        //currentTile.HighlightRed();

        if (Input.GetMouseButtonDown(0))
            SelectEnemy();
    }
    private void SelectEnemy()
    {
        selectedEnemy = currentTile.occupyingCharacter;
 
    }

    private void NavigateToTile()
    {
        if (selectedCharacter == null || selectedCharacter.Moving == true)
            return;

        if (RetrievePath(out Path newPath))
        {
            if (Input.GetMouseButtonDown(0))
            {
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

    bool RetrievePath(out Path path)
    {
      

        if (selectedCharacter != null)
        {
            if (pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingActionPoints/*, selectedCharacter.characterTile*/).Contains(currentTile))
            {
                path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
                return true;
            }
            else
            {
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
        pathfinder.ClearIllustratedPath();
        if (currentTile == null  || currentTile.Occupied == false)
            return;

        //currentTile.ModifyCost(currentTile.terrainCost-1);//Reverses to previous cost and color after being highlighted
        currentTile.ClearHighlight();
        currentTile = null;
    }
    

    public void ClearHighlightReachableTiles()
    {
        foreach (Tile tile in reachableTiles)
        {
            tile.ClearHighlight();
        }
    }
    private void HighlightReachableTiles()
    { 
        ClearHighlightReachableTiles();
        reachableTiles = pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingActionPoints/*, selectedCharacter.characterTile*/);
        foreach (Tile tile in reachableTiles)
        {
            tile.HighlightMoveable();
            
            if (tile == selectedCharacter.characterTile)
            {
                tile.ClearHighlight();
            }
        }
    }
    public void HighlightAttackableTiles()
    { 
        ClearHighlightAttackableTiles();
        attackableTiles = pathfinder.GetAttackableTiles(selectedCharacter.characterTile, selectedCharacter.GetComponent<SkillContainer>().selectedSkill.skillData.skillRange/*, selectedCharacter.characterTile*/);
        foreach (Tile tile in attackableTiles)
        {
            tile.HighlightAttackable();

            if (tile == selectedCharacter.characterTile)
            {
                tile.ClearHighlight();
            }
        }
    }
    public void ClearHighlightAttackableTiles()
    {
        foreach (Tile tile in attackableTiles)
        {
            tile.ClearHighlight();
        }
    }

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