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
    
    [BoxGroup("UI")][SerializeField] public GameObject HitChanceUIGameObject;
    [BoxGroup("UI")][SerializeField] public TextMeshProUGUI HitChanceText;
    
    [BoxGroup("Free Roam")][SerializeField]
    public LayerMask freeRoamMask;
    [BoxGroup("Free Roam")][SerializeField]
    //public Transform freeRoamTarget;
    IAstarAI[] ais;
    
    List<Tile> reachableTiles = new List<Tile>();
    List<Tile> attackableTiles = new List<Tile>();
    [SerializeField]
    AudioClip click, pop;
    [SerializeField]LayerMask interactMask;
    [SerializeField]LayerMask UILayerMask;
    public Action<Tile> CurrentTileChangedAction;
    public Action<Tile> CharacterSelectedAction;
    //Debug purposes only
    [SerializeField]
    bool debug;
    
    Path Lastpath;
    Pathfinder pathfinder;
    [Tooltip("This is a null check for selectedCharacter")]
    public bool characterSelected = false;
    public SkillContainer selectedCharacterSkillContainer;
    [SerializeField] public bool isMouseOverUI = false;
    
   /*
   public Dictionary<int, Color> bulkHighlightColorsMap = new Dictionary<int, Color>()
    {
        {0, new Color(0.3f, 0.8f, 0.3f, 1) }, //Moveable
        {1, new Color(1, 0.5f, 0.1f, 1) },//Attackable
    };
    */
    
    /*Dictionary<int, Color> tileInspectColors = new Dictionary<int, Color>()
    {
        {0, new Color(0f, 0.38f, 1f, 0.3f) }, //Blue -- Default
        {1, new Color(0f, 1, 0, 0.3f) },//Green -- Player
        {2, new Color(1f, 0, 0, 0.3f) },//Red -- Enemy
        {3, new Color(1f, 1, 0, 0.3f) },//Yellow -- Cover

    };*/
    [SerializeField] private static float intensity = 1;

    
    
    Dictionary<int, Color> tileInspectColors = new Dictionary<int, Color>()
    {
        {0, new Color(0.000f * intensity, 6.334f * intensity, 12.996f * intensity, 0.719f) }, //Blue -- Default
        {1, new Color(0f, 12.99604f, 0.9965293f, 0.7176471f) },//Green -- Player
        {2, new Color(12.99604f, 0.3817299f, 0, 0.7176471f) },//Red -- Enemy
        {3, new Color(12.99604f, 12.99604f, 0, 0.7176471f) },//Yellow -- Cover

    };
    
    
   /*public Dictionary<int, Color> tileHighligthColors = new Dictionary<int, Color>()
    {
        {0, new Color(0.3f, 0.8f, 0.3f, 0.4f) },//Green -- Movable
        {1, new Color(1, 0.5f, 0.1f, 0.4f) },//Orange -- Attackable
    };*/
   
   public Dictionary<int, Color> tileHighligthColors = new Dictionary<int, Color>()
   {
       {0, new Color(1, 1, 1, 0.7176471f) }, //white -- Movable
       {1, new Color(12.99604f, 1.903436f, 0f, 0.7176471f) },//Orange -- Attackable
       {2, new Color(0.3f, 0.8f, 0.3f, 0.4f) },//Green -- Movable
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
        
        ais = FindObjectsOfType<MonoBehaviour>().OfType<IAstarAI>().ToArray();
    }

    private void Update()
    {
        Clear();
        MouseUpdate();
        CheckMouseOverUI();
        CheckCharacterInputs();
        //UpdateFreeRoamTargetPosition();
        
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

    public void CheckCharacterInputs()
    {
        if (characterSelected) //eselect character //todo skill selectedi variable olarak tut
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (selectedCharacterSkillContainer.skillSelected == false)
                {
                    selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false);
                    selectedCharacter.GetComponent<QuickOutline>().enabled = false;
                    selectedCharacter = null;
                    characterSelected = false;
                    ClearHighlightReachableTiles();
                    pathfinder.EnableIllustratePath(false);
                    selectedCharacterSkillContainer.skillSelected = false;
                    selectedCharacterSkillContainer = null;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (selectedCharacter.GetComponent<Inventory>().GetSpawnedInventoryUIScript().GetSkipButton().interactable)
                {
                    selectedCharacter.GetComponent<Inventory>().GetSpawnedInventoryUIScript().GetSkipButton().onClick.Invoke();
                }
                
            }
        }
    }
    
    public void CheckMouseOverUI()
    {
        //check if mouse is over ui but exclude some game objects
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject(); //is this expensive
    }
    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask) || isMouseOverUI)
            return;

       // if(lastTile != null) lastTile.ClearHighlight();
        currentTile = hit.transform.GetComponent<Tile>();
        if(lastTile != currentTile) CurrentTileChangedAction?.Invoke(currentTile);//check if the currentTile value has changed
        lastTile = currentTile;
        characterUnderMouse = currentTile.occupyingCharacter;
        InspectTileHighlight();
        InspectTile();
    }

    public void InspectTileHighlight()
    {
        //make the this method work every 15 frames
        /*if (Time.frameCount % 15 != 0)
            return;*/
        
        /*if (InspectTileGO.transform.position != currentTile.transform.position)
        {
            CurrentTileChangedAction?.Invoke();
        }*/
        
        
        
        InspectTileGO.transform.position = currentTile.transform.position;
        
        if (currentTile.Occupied)
        {
            if (currentTile.OccupiedByCoverPoint)
            {
                // inspectTileMeshRenderer.material.color = tileInspectColors[3];
                inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[3]);
                return;
            }
            
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
        else
        {
            // inspectTileMeshRenderer.material.color = tileInspectColors[0];
             inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[0]);

             if (currentTile.isCoveredByCoverPoint)
             {
                 inspectTileMeshRenderer.material.SetColor("_RimColor",tileInspectColors[3]);
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
            switch (selectedCharacter.GetComponent<Character>().characterState)
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

                    /*if (currentTile.occupiedByEnemy)
                    {
                        if (Pathfinder.Instance.CheckCoverPoint(selectedCharacter, currentTile.occupyingCharacter))
                        {
                            
                        }
                    }*/
                    
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
            TrySelectPlayer(currentTile.occupyingCharacter);
        }
            
    }
    
   
    public void TrySelectPlayer(Character chaaracter)
    {
        //CharacterSelectedAction?.Invoke(chaaracter.characterTile);
        if (characterSelected == false)
        {
            SelectPlayer(chaaracter);
            characterSelected = true;
            selectedCharacterSkillContainer = selectedCharacter.GetComponent<SkillContainer>();
        }
        else
        {
            switch (lastSelectedCharacter.GetComponent<Character>().characterState)
            {
                case Character.CharacterState.Idle:
                    break;
           
                case Character.CharacterState.WaitingTurn:
                    lastSelectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false); //close skills ui
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.GetComponent<SkillContainer>();
                    break;
                
                case Character.CharacterState.WaitingNextRound:
                    lastSelectedCharacter.GetComponent<Inventory>().ShowSkillsUI(false); //close skills ui
                    //lastSelectedCharacter.GetComponent<InventoryUI>().SetSkipTurnButtonInteractable(false); //disable skip turn button
                    SelectPlayer(chaaracter);
                    selectedCharacterSkillContainer = selectedCharacter.GetComponent<SkillContainer>();
                    break;
           
                case Character.CharacterState.Attacking:
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    //selectedCharacter = lastSelectedCharacter;
                    //lastSelectedCharacter.GetComponent<Character>().Rotate(lastSelectedCharacter.transform.position, currentTile.transform.position);
                    break;
               
            }
        }
        
    }

    public void SelectPlayer(Character character)
    {
        if (lastSelectedCharacter != null) lastSelectedCharacter.GetComponent<QuickOutline>().enabled = false;
        
        selectedCharacter = character; //select character
        
        selectedCharacter.GetComponent<QuickOutline>().enabled = true;
        
        lastSelectedCharacter = selectedCharacter;
        if (selectedCharacter.characterState == Character.CharacterState.WaitingTurn)
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
       // CharacterSelectedAction?.Invoke(selectedEnemy.characterTile);
    }

    private void NavigateToTile(Character activator )
    {
        if (selectedCharacter == null || selectedCharacter.Moving == true)
            return;

        if (RetrievePath(activator, out Path newPath))
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

   public bool RetrievePath(Character activator, out Path path)
    {
      

        if (selectedCharacter != null)
        {
            if (pathfinder.GetReachableTiles(selectedCharacter.characterTile, selectedCharacter.remainingActionPoints/*, selectedCharacter.characterTile*/).Contains(currentTile))
            {
                path = pathfinder.FindPath(activator, selectedCharacter.characterTile, currentTile);
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
        //currentTile.ClearHighlight();
        currentTile = null;
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
        attackableTiles = pathfinder.GetAttackableTiles(selectedCharacter.characterTile, selectedCharacter.GetComponent<SkillContainer>().selectedSkill/*, selectedCharacter.characterTile*/);
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

    public void CurrentTileChangedFunc(Tile newTile)//this calculates the accuracy before shooting, adds rotation to player
    {
        //print(newTile);
        if (characterSelected && selectedCharacter.characterState == Character.CharacterState.Attacking)//todo add or FreeRoam
        {
            selectedCharacter.GetComponent<SkillContainer>().ResetCoverAccruacyDebuff();
            selectedCharacter.GetComponent<SkillContainer>().ResetCoverdamageDebuff();
            if (isMouseOverUI == false)
            {
                selectedCharacter.Rotate(currentTile.transform.position);
            }
            
            if (newTile.occupiedByEnemy && attackableTiles.Contains(newTile))
            {
                if (Pathfinder.Instance.CheckCoverPoint(selectedCharacter.characterTile, newTile, true))
                {
                    selectedCharacter.GetComponent<SkillContainer>().ApplyCoverAccuracyDebuff();
                    selectedCharacter.GetComponent<SkillContainer>().ApplyCoverDamageDebuff();
                    EnableDisableHitChanceUI(true);
                    /*HitChanceUIGameObject.SetActive(true);
                    StartCoroutine(HitChanceUIToMousePos());  
                    HitChanceText.text = selectedCharacter.GetComponent<SkillContainer>().selectedSkill.accuracy.ToString() + "%";*/
                    //newTile.occupyingEnemy.GetComponent<TooltipTrigger>().AddToContent(selectedCharacter.GetComponent<SkillContainer>().selectedSkill.accuracy.ToString());
                }
                else
                {
                    selectedCharacter.GetComponent<SkillContainer>().ResetCoverAccruacyDebuff();
                    selectedCharacter.GetComponent<SkillContainer>().ResetCoverdamageDebuff();
                    EnableDisableHitChanceUI(true);
                    /*HitChanceUIGameObject.SetActive(true);
                    StartCoroutine(HitChanceUIToMousePos());
                    HitChanceText.text = selectedCharacter.GetComponent<SkillContainer>().selectedSkill.accuracy.ToString() + "%";*/
                }
            }
            else
            {
                selectedCharacter.GetComponent<SkillContainer>().ResetCoverAccruacyDebuff();
                selectedCharacter.GetComponent<SkillContainer>().ResetCoverdamageDebuff();
                EnableDisableHitChanceUI(false);
                /*StopCoroutine(HitChanceUIToMousePos());
                HitChanceUIGameObject.SetActive(false);*/
                //lastTile.occupyingEnemy.GetComponent<TooltipTrigger>().AddToContent(selectedCharacter.GetComponent<SkillContainer>().selectedSkill.accuracy.ToString());
            }
            
        }
    }

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
            HitChanceText.text = selectedCharacter.GetComponent<SkillContainer>().selectedSkill.accuracy.ToString() + "%";
        }
        else
        {
            StopCoroutine(HitChanceUIToMousePos()); 
        }
        HitChanceUIGameObject.SetActive(value);
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