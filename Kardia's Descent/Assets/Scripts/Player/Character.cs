using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.CameraSystem;
using DG.Tweening;
using FischlWorks_FogWar;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public EverythingUseful everythingUseful;
    [BoxGroup("Character Info")]
    public Sprite characterSprite;
    [BoxGroup("Character Info")]
    public CharacterStats characterStats; 
    [BoxGroup("Character Info")]
    public SkillContainer SkillContainer;
    [BoxGroup("Character Info")]
    public HeartContainer heartContainer;
    [BoxGroup("Character Info")]
    public Inventory inventory;
    [BoxGroup("Character Info")]
    public InventoryUI inventoryUI;
    [BoxGroup("Character Info")]
    public Pathfinder pathfinder;
    [BoxGroup("Character Info")]
    public Tile characterTile;
    [BoxGroup("Character Info")] [SerializeField]
    private GameObject characterCard;
    [BoxGroup("Character Info")]
    public Animator animator;
    [BoxGroup("Character Info")]
    public QuickOutline outline;
    
    [BoxGroup("Stats")]
    public int actionPoints = 3;
    [BoxGroup("Stats")]
    public int remainingActionPoints;
    [BoxGroup("Stats")]
    public int maxActionPoints = 10;
    [BoxGroup("Stats")] [Tooltip("Used for decreasin at the end of the turn if there is a non perma ap buff")]
    public int temporaryActionPointsToDecrease = 0;
    [BoxGroup("Stats")]
    public int initiative = 1;//todo: this works in reverse!!!
    [BoxGroup("Stats")]
    public SGT_Health health;
    [BoxGroup("Stats")] [Tooltip("Like a vision cone, lower value means wider range")] [SerializeField]
    private float detectingThreshold = 0.45f;
    [BoxGroup("Stats")] [Tooltip("Kinda like a stealth value, in how many tiles away the enemy can detect this character")]
    public int detectionTile = 4;
    [BoxGroup("Stats")]
    public float moveSpeed = 1;
    [BoxGroup("Stats")] /*[HideInInspector]*/
    public bool canSpeedUp = true;

    [BoxGroup("Variables")] [SerializeField]
    public bool findTileAtAwake = true;
    [BoxGroup("Variables")] [SerializeField] 
    public float YOffset = 0;
    [BoxGroup("Variables")] [SerializeField] 
    public bool canMove = true;
    [BoxGroup("Variables")] [SerializeField] 
    public bool canAttack = true;
    [BoxGroup("Variables")] [SerializeField] 
    public bool isInAttackTileSelection = false; //used for knowing if its selecting where to attack, turns to false after attack is pressed to block changing attacking tile
    [BoxGroup("Variables")] [SerializeField] 
    public bool inCombat = false;
    [BoxGroup("Variables")] [SerializeField] 
    public bool isStunned = false;
    [BoxGroup("Variables")] [SerializeField] 
    private int remainingStunTurns = 0;
    [BoxGroup("Variables")] [SerializeField] 
    public bool isDead = false;
    [BoxGroup("Variables")] [SerializeField] 
    public bool diedOnSelfTurn = false;
    [BoxGroup("Variables")] [SerializeField] 
    public bool canRotate = true;
    [BoxGroup("Functions")] [SerializeField] 
    private bool dropItemsOnDeath = false;
    [BoxGroup("Functions")] [Sirenix.OdinInspector.ShowIf("dropItemsOnDeath")] [SerializeField] 
    private List<GameObject> itemsToDrop = new List<GameObject>();
    
    [BoxGroup("Transforms")] [SerializeField] 
    public Transform Head;
    [BoxGroup("Transforms")]
    public Transform Hand;
    public enum CharacterClass
    {
        None, Tank, Rogue, Sniper, Bombardier, TheRegular, Medic, Support, Bruiser, Melee, Ranged
    }

    [BoxGroup("Character Info")]
    public CharacterClass characterClass;
    
    public bool Moving  = false;
    
    public LayerMask GroundLayerMask;
    [SerializeField] LayerMask detectionLayerMask;
    
    public enum CharacterState
    {
        Idle, Moving, Attacking, Dead, WaitingTurn, WaitingNextRound
    }
    [BoxGroup("Character Info")]
    public CharacterState characterState;

    public GameObject HitTextGameObject;
    
    public  Action<int> OnActionPointsChange;
    public Action OnTurnStart;
    public Action CombatStartedAction;
    public Action CombatEndedAction;
    public Action<Character> OnCharacterDeath;
    public Action OnCharacterRecieveDamageAction;
    public Action<SkillContainer.Skills> OnAttackEndAction;
    
    [FoldoutGroup("Events")] public UnityEvent AwakeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int> OnMovePointsChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int, string> OnActionPointsChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int> OnActionPointsChangeEvent2;
    [FoldoutGroup("Events")] public UnityEvent OnHealthChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent PlayerTurnStart;
    [FoldoutGroup("Events")] public UnityEvent MoveStart;
    [FoldoutGroup("Events")] public UnityEvent MoveEnd;
    [FoldoutGroup("Events")] public UnityEvent MovePointsExhausted;
    [FoldoutGroup("Events")] public UnityEvent ActionPointsExhausted;

   public int extraMeleeDamage = 0;
   public int extraRangedAccuracy = 0;

   public TurnSystem TurnSystem;
   public Interact Interact;
   public CameraSystem CameraSystem;
   public LevelManager LevelManager;

   public void APTest(int a)
   {
      // Debug.Log($"ap test {remainingActionPoints}");
   }
   private void Awake()
    {
        OnActionPointsChange += APTest;
        TurnSystem = everythingUseful.TurnSystem;
        Interact = everythingUseful.Interact;
        CameraSystem = everythingUseful.CameraSystem;
        LevelManager = everythingUseful.LevelManager;
        //characterState = CharacterState.Idle;
        // ResetMovePoints();
        AssignSkillValues();
        ResetActionPoints();
        if (findTileAtAwake) FindTileAtStart();
        AwakeEvent?.Invoke();
    }

    public void AssignSkillValues()
    {
        if (characterStats != null)
        {
            //Strength
            extraMeleeDamage = characterStats.Strength * 5;
            //Dexterity
            actionPoints = characterStats.Dexterity;
            if (actionPoints > maxActionPoints)
            {
                actionPoints = maxActionPoints;
            }
            ResetActionPoints();
            //Constitution
            int prevMaxHealth = health.Max;
            
            health.Max = 100 + characterStats.Constitution * 10;
            health.HealthIncrease(health.Max - prevMaxHealth);
            health.UpdateHealthBar();
            if (inventoryUI != null) inventoryUI.UpdateHealth();
            
            //Aiming
            extraRangedAccuracy = characterStats.Aiming * 5;
        }
    }
    
    #region Movement

    /// <summary>
    /// If no starting tile has been manually assigned, we find one beneath us
    /// </summary>
    public void FindTileAtStart()
    {
        if (characterTile != null)
        { 
            FinalizePosition(characterTile, true);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>(), true);
            return;
        }

        /*if (!everythingUseful.SceneChanger.isOnMainMenu)
        {
            Debug.Log("Unable to find a start position");
        }*/

        
    }

    private IEnumerator moveCoroutine;
    public void StartMove(Path _path, bool rotate = true, Action OnComplete = null, bool spendActionPoints = true)
    {
        if (_path == null) return;
        
        if (canMove)
        {
            MoveStart.Invoke();
            characterState = CharacterState.Moving;
            /*Moving = true;*/
            characterTile.Occupied = false;
            characterTile.ResetOcupying();
            
            animator.SetBool("Walk", true);
            
            if (this is Player)
            {
                characterTile.occupiedByPlayer = false;
            }

            if (this is Enemy)
            {
                characterTile.occupiedByEnemy = false;

                if (Interact.GetReachableTiles().Contains(characterTile) && Interact.characterSelected && (Interact.selectedCharacter.characterState == CharacterState.WaitingTurn || Interact.selectedCharacter.characterState == CharacterState.Idle))
                {
                    characterTile.HighlightMoveable();
                    //Interact.HighlightReachableTiles();
                    // Debug.Log($"enemy start move");
                }

                //  print("enemy move start");
            }
            //lastTransform.position = _path.tiles[_path.tiles.Count -1 ].transform.position - _path.tiles[0].transform.position ;
            moveCoroutine = MoveAlongPath(_path, rotate, OnComplete, spendActionPoints);
            StartCoroutine(moveCoroutine);
        }
    }

    public void StartKnockbackMove(Path _path, bool rotate = true, Action OnComplete = null, bool spendActionPoints = true)
    {
        characterState = CharacterState.Moving;
        Moving = true;
        characterTile.Occupied = false;
        characterTile.ResetOcupying();
            
        animator.SetTrigger("Knockback");
        animator.ResetTrigger("Hit");
            
        if (this is Player)
        {
            characterTile.occupiedByPlayer = false;
        }
        if (this is Enemy)
        {
            characterTile.occupiedByEnemy = false;
        }
        

        StartCoroutine(MoveAlongPath(_path, rotate, OnComplete, spendActionPoints));
    }
    
    IEnumerator MoveAlongPath(Path path, bool rotate, Action OnComplete = null, bool spendActionPoints = true)
    {
        const float MIN_DISTANCE = 0.05f;
        const float TERRAIN_PENALTY = 0.5f;

        int currentStep = 0;
        int pathLength = path.tiles.Count-1;
        Tile currentTile = path.tiles[0];
        Tile targetTile = path.tiles[pathLength];
        float animationTime = 0f;

        while (currentStep <= pathLength )
        {
            
            if ((remainingActionPoints > 0 || !spendActionPoints || TurnSystem.turnState == TurnSystem.TurnState.FreeRoamTurn))
            {
                if (!inCombat &&  path.tiles[currentStep].Occupied )
                {
                    if (path.tiles[currentStep].occupyingCharacter != this)
                    {
                        // stop if there is someone in fron
                        StartCoroutine(WaitForMoveToEnd(false));
                    }
                }
                
                yield return null;
                canSpeedUp = false;
                Moving = true;
                //Move towards the next step in the path until we are closer than MIN_DIST
                Vector3 nextTilePosition = path.tiles[currentStep].transform.position;

                float movementTime = animationTime / (moveSpeed + path.tiles[currentStep].terrainCost * TERRAIN_PENALTY);
                /*if(!path.tiles[currentStep].Occupied) */

                if (moveAndRotateTween != null && moveAndRotateTween.IsActive())
                {
                    moveAndRotateTween.Kill(); // dotween optimization (?)
                }
                MoveAndRotate(currentTile, path.tiles[currentStep] /*nextTilePosition*/, movementTime , currentStep, pathLength, rotate);
                
                animationTime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextTilePosition) > MIN_DISTANCE)
                    continue;
                //decrease remainingMoveSteps if the value of currentTile is changed
                if (currentTile != path.tiles[currentStep])
                {
                    characterTile.Occupied = false;
                    characterTile.ResetOcupying();
                    characterTile.occupiedByEnemy = false;
                    characterTile.occupiedByPlayer = false;
                    
                    characterTile = path.tiles[currentStep];
                    characterTile.Occupied = true;
                    characterTile.occupyingCharacter = this;
                    characterTile.occupyingGO = this.gameObject;
                    if (this is Player)
                    {
                        characterTile.occupyingPlayer = this.GetComponent<Player>();
                        characterTile.occupiedByPlayer = true;
                    }
                    else if (this is Enemy)
                    {
                        characterTile.occupyingEnemy = this.GetComponent<Enemy>();
                        characterTile.occupiedByEnemy = true;
                    }
                    canSpeedUp = true;
                    Moving = false;
                    if (this is Enemy && !inCombat)
                    {
                        CheckToStartCombat();
                    }
                    if (this is Player && !inCombat)
                    {
                        foreach (var enemy in TurnSystem.enemiesInCombat)
                        {
                            if (enemy.inCombat)
                            {
                                enemy.CheckToStartCombat();
                            }
                        }
                        
                    }
                    
                    if (inCombat && spendActionPoints && TurnSystem.turnState != TurnSystem.TurnState.FreeRoamTurn)
                    {
                        remainingActionPoints-- ;
                        // OnActionPointsChange?.Invoke(remainingActionPoints);
                        if (OnActionPointsChange != null) OnActionPointsChange(remainingActionPoints);
                        OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "-");
                        // print(OnMovePointsChangeEvent);
                    }
                
                    CheckIfEndTurn();
                }
                //Min dist has been reached, look to next step in path
                /*if(!path.tiles[currentStep].Occupied)*/ currentTile = path.tiles[currentStep];
                currentStep++;
                //passingMoveTime = animationTime;
                animationTime = 0f;
            }
        }

        // FinalizePosition(path.tiles[pathLength], false);
        
        FinalizePosition(currentTile, false);
        OnComplete?.Invoke();
    }
    public bool isSpedUp = false;
    public void QueueSpeedUp()
    {
        StartCoroutine(QueueSpeedUpEnum());
       // isSpedUp = true; // if i remove this multiple speed ups can be queued which can be good(?)
    }
    private IEnumerator QueueSpeedUpEnum()
    {
        yield return new WaitUntil(() => canSpeedUp);
        moveSpeed /= 5;
        animator.speed *= 5;
        StartCoroutine(WaitForMoveEnd());
    }
    private IEnumerator WaitForMoveEnd()
    {
        yield return new WaitUntil(() => characterState != Character.CharacterState.Moving);
        moveSpeed *= 5;
        animator.speed /= 5;
        isSpedUp = false;
    }
    public void FinalizePosition(Tile tile, bool findTileAtStart)
    {
        transform.position = tile.transform.position + new Vector3(0, YOffset, 0);
        characterTile = tile;
        Moving = false;
        tile.Occupied = true;
        
        tile.occupyingCharacter = this;
        tile.occupyingGO = this.gameObject;
        if (this is Player)
        {
            tile.occupyingPlayer = this.GetComponent<Player>();
            tile.occupiedByPlayer = true;
        }
        else if (this is Enemy)
        {
            tile.occupyingEnemy = this.GetComponent<Enemy>();
            tile.occupiedByEnemy = true;
        }
        
        if (!findTileAtStart)//dont do this if we are just finding the tile at the start
        {
            if (characterState != CharacterState.WaitingNextRound)//this is for the character to not end turn after moving bc maybe it has action points left
            {
                characterState = CharacterState.WaitingTurn;
                
                if (this is Player)
                {
                    Interact.HighlightReachableTiles();
                }
            }

            if (this is Enemy) //this is to update the reachable tiles after enemy movement
            {
                if (Interact.GetReachableTiles().Contains(tile) && Interact.characterSelected && (Interact.selectedCharacter.characterState == CharacterState.WaitingTurn || Interact.selectedCharacter.characterState == CharacterState.Idle))
                { 
                    tile.ClearHighlight();
                    //Interact.HighlightReachableTiles();
                }
            }
            
            animator.SetBool("Walk", false);
            animator.ResetTrigger("Knockback");
            MoveEnd.Invoke();
            
            // CheckToStartCombat();
        }
    }

    float movementThreshold = 0.1f;

    Tween moveAndRotateTween;
    Tween rotateTween;
   void MoveAndRotate(Tile origin, Tile destination, float duration, float currentStep, float pathLength, bool rotate)
    {
        
        transform.position = Vector3.Lerp(origin.transform.position, destination.transform.position, duration);
        if (rotate)
        {
            if (Vector3.Distance(transform.position, destination.transform.position) > movementThreshold)
            {
                moveAndRotateTween = transform.DOLookAt(destination.transform.position, 0.75f, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack);
            }
        }
    }

    public void Rotate(Vector3 destination, float duration = 0.75f, Action OnComplete = null)
    {
        if (rotateTween != null /*&& moveAndRotateTween.IsActive()*/)
        {
            rotateTween.Kill(); // dotween optimization (?)
        }
        // transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
        if (canRotate)
        {
            rotateTween = transform.DOLookAt(destination, duration, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack).OnComplete(()=> OnComplete?.Invoke());
        }
 
        //StartCoroutine(RotateEnum(origin, destination));
    }
    

    #endregion
    
    #region Turn System

    bool doOnce = true;
    public void StartTurn()
    {
        if (TurnSystem.turnState != TurnSystem.TurnState.FreeRoamTurn && inCombat)
        {
            if (TurnSystem.IsThisCharactersTurn(this))
            {

                if (!isStunned)
                {
                    canMove = true;
                    canAttack = true;
                    characterState = CharacterState.WaitingTurn;//todo enemy turn does not change to waiting for turn
                    
                }
                
                // ResetActionPoints();

                //if (!doOnce) //bu niye var aq
                {
                    
                    AddActionPoints();
                    OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "+");
                }
                doOnce = false;
                
                if (isStunned)
                {
                    remainingActionPoints = 0;
                }

                if (this is Enemy)
                {
                    OnTurnStart?.Invoke();
                }
                
                if (this is Player)
                {
                    if(PlayerTurnStart != null) PlayerTurnStart.Invoke();
                    if (this == Interact.selectedCharacter)
                    {
                        Interact.HighlightReachableTiles();
                    }
                }
            }
            else
            {
                characterState = CharacterState.WaitingNextRound;
            }
        }
    }
    
    public void EndofMovePoints()
    {
        
        MovePointsExhausted.Invoke();
        characterState = CharacterState.WaitingTurn;
        CheckIfEndTurn();
        // TurnSystem.NextTurn();
    }

    public void CheckIfEndTurn()
    {
        if (remainingActionPoints <= 0)//todo maybe dont force the player to end turn if they have action points left
        {
            canMove = false;
            canAttack = false;
            characterState = CharacterState.WaitingNextRound;
            // EndTurn();
        }
    }
    private static LTDescr delay;
    public void EndTurn()
    {
        if(this is Player && (TurnSystem.turnState == TurnSystem.TurnState.FreeRoamTurn || characterState == CharacterState.Moving))
        {
            return;
        }

        if (this is Player)
        {
            if(SkillContainer.skillSelected) SkillContainer.DeselectSkill(SkillContainer.selectedSkill);
            pathfinder.ClearIllustratedPath();
        }
        remainingActionPoints -= temporaryActionPointsToDecrease;
        if (remainingActionPoints < 0)
        {
            remainingActionPoints = 0;
        }
        temporaryActionPointsToDecrease = 0;
        canMove = false;
        canAttack = false;
        characterState = CharacterState.WaitingNextRound;

        if (characterCard != null) characterCard.GetComponent<CustomEvent2>().CustomEmitterFunc2();

        if (this is Enemy)
        {
            GetComponent<StateController>().aiActive = false;
        }

        if (this is Player)
        {
            Interact.ClearHighlightAttackableTiles();
            Interact.ClearHighlightReachableTiles();
        }
        
        TurnSystem.NextTurn();

        if (this is Player)
        {
            foreach (var player in LevelManager.players)
            {
                if (player.isUnlocked && player.inCombat && player.characterState != CharacterState.Dead && player.characterState != CharacterState.WaitingNextRound)
                {
                    CameraSystem.OnCharacterSelected(player.characterTile);
                    Interact.TrySelectPlayer(player);
                    return;
                }
            }
        }
        
    }

    [Button]
    public void CheckToStartCombat()
    {
        Vector3 yOffSet = new Vector3(0, PathfinderVariables.Instance.characterYOffset, 0);
        if (this is Enemy)
        {
            var playersInCombat = LevelManager.players;
            for (int i = 0; i < playersInCombat.Count; i++)
            {
                if (Vector3.Distance(characterTile.transform.position, playersInCombat[i].transform.position) < 10 && playersInCombat[i].gameObject.activeInHierarchy && playersInCombat[i].isUnlocked &&
                    pathfinder.GetTilesInBetween(this, characterTile, playersInCombat[i].characterTile, true).Count <= playersInCombat[i].detectionTile
                    && playersInCombat[i].GetUnlocked())
                {
                    Ray ray = new Ray(this.transform.position, this.transform.forward);
                    Vector3 dir1 = ray.direction;
                    Vector3 dir2 = playersInCombat[i].transform.position - ray.origin;
                    
                    var lookAngle = Vector3.Dot(dir1.normalized, dir2.normalized);
                    
                    if (lookAngle > detectingThreshold)
                    {
                        if (!Physics.Linecast(this.transform.position +yOffSet, playersInCombat[i].transform.position+ yOffSet, detectionLayerMask))
                        {
                            if(!inCombat) StartCombat(playersInCombat[i]);
                            if(!playersInCombat[i].inCombat) playersInCombat[i].StartCombat(this);
                        
                            if (TurnSystem.turnState == TurnSystem.TurnState.FreeRoamTurn)
                            {
                                TurnSystem.CombatStarted();
                            }
                            //Debug.Log($"Combat started by {TurnSystem.enemies[i].name}");
                        }
                    }
                    
                    
                }
                
            }
        }
    }
    
    [Button, GUIColor(0f, 1f, 1f)]
    public void CheckDotDebug()
    {
        if (this is Enemy)
        {
            var playersInCombat = TurnSystem.playersInCombat;
            for (int i = 0; i < playersInCombat.Count; i++)
            {
                //if (pathfinder.GetTilesInBetween(this, characterTile, playersInCombat[i].characterTile, true).Count <= 4)
                {
                    Ray ray = new Ray(this.transform.position, this.transform.forward);
                    Vector3 dir1 = ray.direction;
                    Vector3 dir2 = playersInCombat[i].transform.position - ray.origin;
                    
                    var lookAngle = Vector3.Dot(dir1.normalized, dir2.normalized);
                    
                    Debug.Log($"lookangle = {lookAngle}");
                    if (lookAngle > detectingThreshold)
                    {
                        if (!Physics.Linecast(this.transform.position, playersInCombat[i].transform.position, detectionLayerMask))
                        {
                            Debug.Log($"Combat started ");
                        }
                    }
                    
                    
                }
                
            }
        }
    }
    #endregion

    #region Combat

    [Button, GUIColor(1f, 1f, 1f)]
    public void StartCombat(Character combatStarter)
    {
        //FinalizePosition(characterTile, false);
        // StopCoroutine(moveCoroutine);
        StartCoroutine(WaitForMoveToEnd(true));
        CombatStartedAction?.Invoke();
        Rotate(combatStarter.transform.position);
        if (this is Player)
        {
            LevelManager.AddPlayerToCombat(GetComponent<Player>());
        }
        if (this is Enemy)
        {
            LevelManager.AddEnemyToCombat(GetComponent<Enemy>());
            GetComponent<StateController>().StartCombat();
            remainingActionPoints = 0; // enemies were having max ap on their first turn
        }
    }

    private IEnumerator WaitForMoveToEnd(bool startCombat)
    {
        yield return new WaitUntil(() => Moving == false);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (this is Player && !isInAttackTileSelection)
        {
            FinalizePosition(characterTile, false); // on combat start attack was cancelling so i did this
        }
        
        if (this is Enemy && !isInAttackTileSelection)
        {
            FinalizePosition(characterTile, false);
            GetComponent<StateController>().canExitState = true;
        }
        if(startCombat) inCombat = true;
    }

    public void ExitCombat()
    {
        inCombat = false;
        canAttack = true;
        CombatEndedAction?.Invoke();
        ResetActionPoints();
        SkillContainer.ForceResetSkillCooldowns();
        characterState = CharacterState.WaitingTurn;
        TurnSystem.PlayerExitedCombat(GetComponent<Player>());
    }
    
        
    public void ResetActionPoints()
    {
        remainingActionPoints = actionPoints;
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "+");
    }

    private void AddActionPoints()
    {
        remainingActionPoints += actionPoints;
        
        if (remainingActionPoints > maxActionPoints)
        {
            remainingActionPoints = maxActionPoints;
        }
        
    }


    public void AttackStart()
    {
        isInAttackTileSelection = true;
        canRotate = true;
        characterState = CharacterState.Attacking;
        
        animator.SetTrigger("AttackStart");
    }

    
    public void AttackCancel()
    {
        isInAttackTileSelection = false;
        canRotate = true;
        characterState = CharacterState.WaitingTurn;

        if (this is Player)
        {
            Interact.EnableDisableHitChanceUI(false);
        }
        
        //animator.ResetTrigger("Attack");
        // animator.SetTrigger("AttackCancel");
    }

    public void Attack()
    {
        isInAttackTileSelection = false;
        canRotate = false;
        if (this is Player)
        {
            Interact.EnableDisableHitChanceUI(false);
        }
        
        animator.ResetTrigger("AttackCancel");
        animator.SetTrigger("Attack");
    }

    public void AttackEnd(SkillContainer.Skills skill)
    {
        isInAttackTileSelection = false;
        OnAttackEndAction?.Invoke(skill);
        characterState = CharacterState.WaitingTurn;
        remainingActionPoints -= skill.actionPointUse;//maybe add a - or + variable to skill use
        OnActionPointsChange?.Invoke(remainingActionPoints);
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "-");
        CheckIfEndTurn();

        if (!inCombat)
        {
            ResetActionPoints();
        }
    }

    public void OnCharacterDeathFunc()
    {
        animator.SetBool("Dead", true);
        isDead = true;
        characterState = CharacterState.Dead;
        if (characterCard != null) characterCard.SetActive(false);
        //gameObject.transform.DOMoveY(gameObject.transform.position.y - 0.5f, 0.5f);
        Stun(false, 0);
        OnCharacterDeath?.Invoke(this);
        if (this is Player)
        {
            characterTile.occupiedByPlayer = false;
            characterTile.occupyingPlayer = null;
            TurnSystem.PlayerDied(GetComponent<Player>());
        }
        if (this is Enemy)
        {
            characterTile.occupiedByEnemy = false;
            characterTile.occupyingEnemy = null;
            GetComponent<StateController>().aiActive = false;
            TurnSystem.EnemyDied(GetComponent<Enemy>());

            foreach (var var in GetComponentsInChildren<csFogVisibilityAgent>())
            {
                Destroy(var);
            }
        }
        characterTile.occupyingGO = null;
        characterTile.Occupied = false;

        if (dropItemsOnDeath)
        {
            // DropAllItemsOnDeath();
            DropRandomItemOnDeath();
        }
        //characterTile = null;
        
    }

    private void DropAllItemsOnDeath()
    {
        Vector3 dropPos = transform.position + new Vector3(0, 1, 0);
        foreach (var item in itemsToDrop)
        {
            Instantiate(item, dropPos, Quaternion.identity);
        }
    }

    private void DropRandomItemOnDeath()
    {
        Vector3 dropPos = transform.position + new Vector3(0, 1, 0);
        int randomIndex = UnityEngine.Random.Range(0, itemsToDrop.Count);
        Instantiate(itemsToDrop[randomIndex], dropPos, Quaternion.identity);
    }
    
    public void OnCharacterRecieveDamageFunc(int damageValue)
    {
        //everythingUseful.SpawnText(damageValue.ToString(), blockedDamage.ToString(), Color.red,Color.white , Head, 3, 1f, health);
        
        animator.SetTrigger("Hit");
        OnCharacterRecieveDamageAction?.Invoke();
        OnHealthChangeEvent?.Invoke();
    }

    public void OnCharacterMiss()
    {
        everythingUseful.SpawnText("MISS", Color.white, Head, 2,1f, health);
    }

    public void OnCharacterHealed(int value)
    {
        everythingUseful.SpawnText(value.ToString(),Color.green, Head,2 ,2, health);
    }

    GameObject stunVFX = null;

    public void Stun(bool value, int turns)
    {
        canMove = !value;
        canAttack = !value;
        
        isStunned = value;
        remainingStunTurns += turns;
        
        if (value)
        {
            if (stunVFX == null)
            {
                stunVFX = VFXManager.Instance.stunVFX.SpawnVFXWithReturn(Head);
                stunVFX.transform.SetParent(Head);
            }
        }
        else
        {
            if (stunVFX != null)
            {
                Destroy(stunVFX);
            }
        }
    }

    public void CheckRemoveStun()
    {
        if (isStunned)
        {
            remainingStunTurns--;

            if (remainingStunTurns == 0)
            {
                Stun(false, 0);
            }
        }
    }

    #endregion
    
    public void SetAnimations(AnimatorOverrideController overrideController)
    {
        animator.runtimeAnimatorController = overrideController;
    }
    
    public void SetCharacterCard(GameObject card)
    {
        characterCard = card;
    }

    public GameObject GetCharacterCard()
    {
        return characterCard;
    }

    public void HiglightOutline(bool value)
    {
        outline.enabled = value;
    }

}