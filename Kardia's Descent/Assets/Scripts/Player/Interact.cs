using System;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

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
    [SerializeField]
    LayerMask interactMask;

    //Debug purposes only
    [SerializeField]
    bool debug;
    
    Path Lastpath;
    Pathfinder pathfinder;
    [Tooltip("This is a null check for selectedCharacter")]
    bool characterSelected = false;
    
    Dictionary<int, Color> tileInspectColors = new Dictionary<int, Color>()
    {
        {0, new Color(0f, 0.38f, 1f, 0.3f) }, //Blue -- Default
        {1, new Color(0f, 1, 0, 0.3f) },//Green -- Player
        {2, new Color(1f, 0, 0, 0.3f) },//Red -- Enemy

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

    public void EnableAttackableTiles(bool value)
    {
        if (characterSelected)
        {
            if (value)
            {
                if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == true)
                {
                    selectedCharacter.canAttack = value;
                    HighlightAttackableTiles();
                }
            }
            else
            {
                if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == true)
                {
                    selectedCharacter.canAttack = value;
                    ClearHighlightAttackableTiles();
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedCharacter.GetComponent<SkillContainer>().skillSelected == false && characterSelected)//deselect character //todo skill selectedi variable olarak tut
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

    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
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
                        selectedCharacter.GetComponent<SkillContainer>().UseSkill(selectedCharacter.GetComponent<SkillContainer>().selectedSkill, currentTile);
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
        if (selectedCharacter.remainingMoveRange > 0)
        {
            EnableMovement(true);
        }
        HighlightReachableTiles(); //highlight tiles within range
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
        //If a character is selected, only tiles within range can be selected
        /*if (selectedCharacter != null)
        {
            path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
            if (path == null || path == Lastpath)
                return false;

            //If the path is too long, do not allow it
            if (path.tiles.Length < selectedCharacter.moveRange)
            {
                path = null; 
                return false;
            }
            else
            {
                path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
                return true;
            }
                
            

            /#1#/If the path is too short, do not allow it
            if (path.tiles.Length < 2)
                return false;#1#

            //If the path is obstructed, do not allow it
            foreach (Tile tile in path.tiles)
            {
                if (tile.Occupied)
                    path = null;
                    return false;
            }
        }
        else
        {
            path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
        
            if (path == null || path == Lastpath)
                return false;

            //Debug only
            if (debug)
            {
                ClearLastPath();
                DebugNewPath(path);
                Lastpath = path;
            }
            return true;
        }*/
        /*if(selectedCharacter != null) //If a character is selected, only tiles within range can be selected
        {
            if (selectedCharacter.moveRange + 2 <= Vector3.Distance(selectedCharacter.transform.position, currentTile.transform.position))
            {
                path = null;
                
                return false;
            }
            else
            {
                
                lastSelectedTile = currentTile;
                currentTile = lastSelectedTile;
                
                path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
            }
        }*/


        if (selectedCharacter != null)
        {
            if (pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingMoveRange/*, selectedCharacter.characterTile*/).Contains(currentTile))
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
            
        /*path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);
        
        if (path == null || path == Lastpath)
            return false;

        //Debug only
        if (debug)
        {
            ClearLastPath();
            DebugNewPath(path);
            Lastpath = path;
        }
        return true;*/
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
        reachableTiles = pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingMoveRange/*, selectedCharacter.characterTile*/);
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
        attackableTiles = pathfinder.GetAttackableTiles(selectedCharacter.characterTile, selectedCharacter.GetComponent<SkillContainer>().selectedSkill.skillRange/*, selectedCharacter.characterTile*/);
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