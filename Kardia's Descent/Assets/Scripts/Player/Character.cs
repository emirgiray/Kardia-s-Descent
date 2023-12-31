using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [BoxGroup("Character Info")]
    public Sprite characterSprite;
    [BoxGroup("Character Info")][SerializeField] 
    private CharacterStats characterStats; 
    [BoxGroup("Character Info")]
    public SkillContainer SkillContainer;
    [BoxGroup("Character Info")]
    public Pathfinder pathfinder;
    [BoxGroup("Character Info")]
    public CharacterMoveData movedata;
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
    [BoxGroup("Stats")]
    public int initiative = 1;//todo: this works in reverse!!!
    [BoxGroup("Stats")]
    public SGT_Health health;
    
    [BoxGroup("Functions")] [SerializeField] 
    public bool canMove = true;
    [BoxGroup("Functions")] [SerializeField] 
    public bool canAttack = true;
    [BoxGroup("Functions")] [SerializeField] 
    public bool inCombat = false;
    [BoxGroup("Functions")] [SerializeField] 
    public bool isStunned = false;
    [BoxGroup("Functions")] [SerializeField] 
    private int remainingStunTurns = 0;
    [BoxGroup("Functions")] [SerializeField] 
    public bool isDead = false;
    
    [BoxGroup("Transforms")] [SerializeField] 
    private Transform Head;
    [BoxGroup("Transforms")]
    public Transform Hand;
    public enum CharacterClass
    {
        None, Tank, Rogue, Sniper, Bombardier, TheRegular, Medic, Support, Bruiser, Melee, Ranged
    }

    [BoxGroup("Character Info")]
    public CharacterClass characterClass;
    
    public bool Moving { get; private set; } = false;
    
    
    public LayerMask GroundLayerMask;
    [SerializeField] LayerMask detectionLayerMask;
    
    
    
    public enum CharacterState
    {
        Idle, Moving, Attacking, Dead, WaitingTurn, WaitingNextRound
    }
    [BoxGroup("Character Info")]
    public CharacterState characterState;
    
    public static Action<int> OnActionPointsChange;
    public Action OnTurnStart;
    public Action CombatStartedAction;
    public Action CombatEndedAction;
    public Action<Character> OnCharacterDeath;
    public Action OnCharacterRecieveDamageAction;
    public Action<SkillContainer.Skills> OnAttackEndAction;
    
    [FoldoutGroup("Events")] public UnityEvent<int> OnMovePointsChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int, string> OnActionPointsChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int> OnActionPointsChangeEvent2;
    [FoldoutGroup("Events")] public UnityEvent OnHealthChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent PlayerTurnStart;
    [FoldoutGroup("Events")] public UnityEvent MoveStart;
    [FoldoutGroup("Events")] public UnityEvent MoveEnd;
    [FoldoutGroup("Events")] public UnityEvent MovePointsExhausted;
    [FoldoutGroup("Events")] public UnityEvent ActionPointsExhausted;

    [HideInInspector] public int extraMeleeDamage = 0;
    [HideInInspector] public int extraRangedAccuracy = 0;

    private void Awake()
    {
        //characterState = CharacterState.Idle;
        // ResetMovePoints();
        AssignSkillValues();
        ResetActionPoints();
        FindTileAtStart();
    }

    public void AssignSkillValues()
    {
        if (characterStats != null)
        {
            extraMeleeDamage = characterStats.Strength * 5;
            actionPoints = characterStats.Dexterity;
            GetComponent<SGT_Health>().Max = 100 + characterStats.Constitution * 10;
            extraRangedAccuracy = characterStats.Dexterity * 5;
        }
    }

    #region Movement

    /// <summary>
    /// If no starting tile has been manually assigned, we find one beneath us
    /// </summary>
    void FindTileAtStart()
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

        Debug.Log("Unable to find a start position");
    }

    public void StartMove(Path _path, bool rotate = true, Action OnComplete = null, bool spendActionPoints = true)
    {
        if (canMove)
        {
            MoveStart.Invoke();
            characterState = CharacterState.Moving;
            Moving = true;
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
                //  print("enemy move start");
            }
            //lastTransform.position = _path.tiles[_path.tiles.Count -1 ].transform.position - _path.tiles[0].transform.position ;
            StartCoroutine(MoveAlongPath(_path, rotate, OnComplete, spendActionPoints));
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
        float animationTime = 0f;

        while (currentStep <= pathLength )
        {
            if (remainingActionPoints  > 0 || !spendActionPoints || TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn)
            {
                yield return null;
                //Move towards the next step in the path until we are closer than MIN_DIST
                Vector3 nextTilePosition = path.tiles[currentStep].transform.position;

                float movementTime = animationTime / (movedata.MoveTime + path.tiles[currentStep].terrainCost * TERRAIN_PENALTY);
                /*if(!path.tiles[currentStep].Occupied) */
                
                
                MoveAndRotate(currentTile, path.tiles[currentStep] /*nextTilePosition*/, movementTime , currentStep, pathLength, rotate);
                
                animationTime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextTilePosition) > MIN_DISTANCE)
                    continue;
                //decrease remainingMoveSteps if the value of currentTile is changed
                if (currentTile != path.tiles[currentStep])
                {
                    if (inCombat && spendActionPoints && TurnSystem.Instance.turnState != TurnSystem.TurnState.FreeRoamTurn)
                    {
                        remainingActionPoints-- ;
                        // OnActionPointsChange?.Invoke(remainingActionPoints);
                        OnActionPointsChange(remainingActionPoints);
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
    
    void FinalizePosition(Tile tile, bool findTileAtStart)
    {
        transform.position = tile.transform.position;
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
                    Interact.Instance.HighlightReachableTiles();
                }
            }

            animator.SetBool("Walk", false);
            MoveEnd.Invoke();
            
            CheckToStartCombat();
        }
    }

    float movementThreshold = 0.1f;

    void MoveAndRotate(Tile origin, Tile destination, float duration, float currentStep, float pathLength, bool rotate)
    {
        
        transform.position = Vector3.Lerp(origin.transform.position, destination.transform.position, duration);
        if (rotate)
        {
            if (Vector3.Distance(transform.position, destination.transform.position) > movementThreshold)
            {
                transform.DOLookAt(destination.transform.position, 0.75f, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack);
            }
        }
    }

    public void Rotate(Vector3 destination, float duration = 0.75f, Action OnComplete = null)
    {
        
        // transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
        transform.DOLookAt(destination, duration, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack).OnComplete(()=> OnComplete?.Invoke());
 
        //StartCoroutine(RotateEnum(origin, destination));
    }
    

    #endregion
    
    #region Turn System

    bool doOnce = true;
    public void StartTurn()
    {
        if (TurnSystem.Instance.turnState != TurnSystem.TurnState.FreeRoamTurn && inCombat)
        {
            if (TurnSystem.Instance.IsThisCharactersTurn(this))
            {

                if (!isStunned)
                {
                    canMove = true;
                    canAttack = true;
                    characterState = CharacterState.WaitingTurn;//todo enemy turn does not change to waiting for turn
                    
                }
                
                // ResetActionPoints();

                if (!doOnce)
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
                    if(this == Interact.Instance.selectedCharacter ) Interact.Instance.HighlightReachableTiles();
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
        // TurnSystem.Instance.NextTurn();
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
        if(this is Player && TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn)
        {
            return;
        }

        if (this is Player)
        {
            SkillContainer.DeselectSkill(SkillContainer.selectedSkill);
            pathfinder.ClearIllustratedPath();
        }
        
        canMove = false;
        canAttack = false;
        characterState = CharacterState.WaitingNextRound;
        
        characterCard.GetComponent<CustomEvent2>().CustomEmitterFunc2();
        
        /*delay = LeanTween.delayedCall(1, () =>
        {
            characterCard.GetComponent<CustomEvent3>().CustomEmitterFunc3();
        });*/
        
        if (this is Enemy)
        {
            GetComponent<StateController>().aiActive = false;
        }

        if (this is Player)
        {
            Interact.Instance.ClearHighlightAttackableTiles();
            Interact.Instance.ClearHighlightReachableTiles();
        }
        
        TurnSystem.Instance.NextTurn();
    }

    [Button]
    public void CheckToStartCombat()
    {
        if (this is Player)
        {
            for (int i = 0; i < GameManager.Instance.enemies.Count; i++)
            {
                if (pathfinder.GetTilesInBetween(this, characterTile, GameManager.Instance.enemies[i].characterTile, true).Count <= 4)
                {
                    if (!Physics.Linecast(this.transform.position, GameManager.Instance.enemies[i].transform.position, detectionLayerMask))
                    {
                        StartCombat();
                        GameManager.Instance.enemies[i].StartCombat();
                        
                        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn)
                        {
                            TurnSystem.Instance.CombatStarted();
                            
                        }
                        //Debug.Log($"Combat started by {TurnSystem.Instance.enemies[i].name}");
                    }
                }
                
            }
        }

        if (this is Enemy)
        {
            for (int i = 0; i < TurnSystem.Instance.playersInCombat.Count; i++)
            {
                if (pathfinder.GetTilesInBetween(this, characterTile, TurnSystem.Instance.playersInCombat[i].characterTile, true).Count <= 4)
                {
                    if (!Physics.Linecast(this.transform.position, TurnSystem.Instance.playersInCombat[i].transform.position, detectionLayerMask))
                    {
                        StartCombat();
                        TurnSystem.Instance.playersInCombat[i].StartCombat();
                        
                        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.FreeRoamTurn)
                        {
                            TurnSystem.Instance.CombatStarted();
                            
                        }
                        //Debug.Log($"Combat started by {TurnSystem.Instance.enemies[i].name}");
                    }
                }
                
            }
        }
    }
    #endregion

    #region Combat

    public void StartCombat()
    {
        inCombat = true;
        CombatStartedAction?.Invoke();
        if (this is Player)
        {
            GameManager.Instance.AddPlayersToCombat();
        }
        if (this is Enemy)
        {
            GameManager.Instance.AddEnemyToCombat();
            GetComponent<StateController>().StartCombat();
        }
    }

    public void ExitCombat()
    {
        inCombat = false;
        CombatEndedAction?.Invoke();
        ResetActionPoints();
        SkillContainer.ForceResetSkillCooldowns();
    }
    
        
    public void ResetActionPoints()
    {
        remainingActionPoints = actionPoints;
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "+");
    }

    public void AddActionPoints()
    {
        remainingActionPoints += actionPoints;
        if (remainingActionPoints > maxActionPoints)
        {
            remainingActionPoints = maxActionPoints;
        }
        
    }


    public void AttackStart()
    {
        
        characterState = CharacterState.Attacking;
        
        animator.SetTrigger("AttackStart");
    }

    
    public void AttackCancel()
    {
        characterState = CharacterState.WaitingTurn;

        if (this is Player)
        {
            Interact.Instance.EnableDisableHitChanceUI(false);
        }
        
        //animator.ResetTrigger("Attack");
        // animator.SetTrigger("AttackCancel");
    }

    public void Attack()
    {
        if (this is Player)
        {
            Interact.Instance.EnableDisableHitChanceUI(false);
        }
        
        animator.ResetTrigger("AttackCancel");
        animator.SetTrigger("Attack");
    }

    public void AttackEnd(SkillContainer.Skills skill)
    {
        OnAttackEndAction?.Invoke(skill);
        characterState = CharacterState.WaitingTurn;
        remainingActionPoints -= skill.actionPointUse;//maybe add a - or + variable to skill use
        OnActionPointsChange?.Invoke(remainingActionPoints);
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints, "-");
        CheckIfEndTurn();
    }

    public void OnCharacterDeathFunc()
    {
        animator.SetBool("Dead", true);
        isDead = true;
        characterState = CharacterState.Dead;
        characterCard.SetActive(false);
        //gameObject.transform.DOMoveY(gameObject.transform.position.y - 0.5f, 0.5f);
        Stun(false, 0);
        OnCharacterDeath?.Invoke(this);
        if (this is Player)
        {
            characterTile.occupiedByPlayer = false;
            characterTile.occupyingPlayer = null;
            TurnSystem.Instance.PlayerDied(GetComponent<Player>());
        }
        if (this is Enemy)
        {
            characterTile.occupiedByEnemy = false;
            characterTile.occupyingEnemy = null;
            TurnSystem.Instance.EnemyDied(GetComponent<Enemy>());
        }
        characterTile.occupyingGO = null;
        characterTile.Occupied = false;
        //characterTile = null;
        
    }

    public void OnCharacterRecieveDamageFunc()
    {
        animator.SetTrigger("Hit");
        OnCharacterRecieveDamageAction?.Invoke();
        OnHealthChangeEvent?.Invoke();
    }

    GameObject stunVFX = null;
    public void Stun(bool value, int turns)
    {
        canMove = !value;
        canAttack = !value;
        
        isStunned = value;
        remainingStunTurns = turns;
        
        if (value)
        {
            if (stunVFX == null)
            {
                stunVFX = VFXManager.Instance.stunVFX.SpawnVFX(Head);
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

}