using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [SerializeField] public int actionPoints = 3;
    [SerializeField] public int remainingActionPoints;
    [SerializeField] public int maxActionPoints = 10;
    /*[SerializeField] public int moveRange = 3;
    [SerializeField] public int remainingMoveRange;*/
    [SerializeField] public int initiative = 1;
    [SerializeField] public bool canMove = true;
    [SerializeField] public bool canAttack = true;
    [SerializeField] public bool inCombat = true;
    [SerializeField] public bool isDead = false;
    [SerializeField] int totalSteps = 0;
    public bool Moving { get; private set; } = false;
    
    public CharacterMoveData movedata;
    public Tile characterTile;
    [SerializeField] LayerMask GroundLayerMask;
    
    
    public enum CharacterState
    {
         Idle, Moving, Attacking, Dead, WaitingTurn, WaitingNextRound
    }

    public CharacterState characterState;
    

    public Action<int> OnActionPointsChange;
    public Action OnTurnStart;
    public Action<Character> OnCharacterDeath;
    
    [FoldoutGroup("Events")] public UnityEvent<int> OnMovePointsChangeEvent;
    [FoldoutGroup("Events")] public UnityEvent<int> OnActionPointsChangeEvent;
    
    [FoldoutGroup("Events")] public UnityEvent PlayerTurnStart;
    [FoldoutGroup("Events")] public UnityEvent MoveStart;
    [FoldoutGroup("Events")] public UnityEvent MoveEnd;
    [FoldoutGroup("Events")] public UnityEvent MovePointsExhausted;
    [FoldoutGroup("Events")] public UnityEvent ActionPointsExhausted;
    
 
    private void Awake()
    {
        characterState = CharacterState.Idle;
        // ResetMovePoints();
        ResetActionPoints();
        FindTileAtStart();
    }

    

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

    IEnumerator MoveAlongPath(Path path, Action OnComplete = null)
    {
        const float MIN_DISTANCE = 0.05f;
        const float TERRAIN_PENALTY = 0.5f;

        int currentStep = 0;
        int pathLength = path.tiles.Length-1;
        Tile currentTile = path.tiles[0];
        float animationTime = 0f;

        while (currentStep <= pathLength && remainingActionPoints  > 0)
        {
            yield return null;
            //Move towards the next step in the path until we are closer than MIN_DIST
            Vector3 nextTilePosition = path.tiles[currentStep].transform.position;

            float movementTime = animationTime / (movedata.MoveSpeed + path.tiles[currentStep].terrainCost * TERRAIN_PENALTY);
            /*if(!path.tiles[currentStep].Occupied) */MoveAndRotate(currentTile.transform.position, nextTilePosition, movementTime);
            animationTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextTilePosition) > MIN_DISTANCE)
                continue;
            //decrease remainingMoveSteps if the value of currentTile is changed
            if (currentTile != path.tiles[currentStep])
            {
                if (inCombat)
                {
                    remainingActionPoints-- ;
                    OnActionPointsChange?.Invoke(remainingActionPoints);
                    OnActionPointsChangeEvent?.Invoke(remainingActionPoints);
                   // print(OnMovePointsChangeEvent);
                }
                
                CheckIfEndTurn();
            }
            //Min dist has been reached, look to next step in path
            /*if(!path.tiles[currentStep].Occupied)*/ currentTile = path.tiles[currentStep];
            currentStep++;
            animationTime = 0f;
        }

        // FinalizePosition(path.tiles[pathLength], false);
        FinalizePosition(currentTile, false);
        OnComplete?.Invoke();
    }

    public void StartMove(Path _path, Action OnComplete = null)
    {
        if (canMove)
        {
            MoveStart.Invoke();
            characterState = CharacterState.Moving;
            Moving = true;
            characterTile.Occupied = false;
            characterTile.ResetOcupying();
            if (this is Player)
            {
                characterTile.occupiedByPlayer = false;
            }

            if (this is Enemy)
            {
                characterTile.occupiedByEnemy = false;
                print("enemy move start");
            }
            StartCoroutine(MoveAlongPath(_path, OnComplete));
        }
    }

    void FinalizePosition(Tile tile, bool findTileAtStart)
    {
        transform.position = tile.transform.position;
        characterTile = tile;
        Moving = false;
        tile.Occupied = true;
        
        tile.occupyingCharacter = this;
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
            }
            
            if (gameObject.tag == "Enemy")//todo: this is a temporary fix for enemies not ending turn after moving, if we want different behaviour for enemies like move hit then flee, we need to change this
            {
               
                //remainingMoveRange = 0;
                //canMove = false;
                //EndofMovePoints();
                //TurnSystem.Instance.currentEnemyTurnOrder++;
            }
            
            MoveEnd.Invoke();
        }
        else
        {
            
        }
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        if (origin.DirectionTo(destination).Flat() != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
        }
    }

    public void Rotate(Vector3 destination)
    {
        // transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
        transform.DOLookAt(destination, 0.75f, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack);
 
        //StartCoroutine(RotateEnum(origin, destination));
    }

    /*public IEnumerator RotateEnum(Vector3 origin, Vector3 destination)
    {
        yield return null;
        // Rotate(origin, destination);
        transform.DOLookAt(destination, 0.75f, AxisConstraint.Y, Vector3.up).SetEase(Ease.OutBack);
    }*/
    

    public void ResetMovePoints()
    {
        remainingActionPoints = actionPoints;
        OnMovePointsChangeEvent?.Invoke(remainingActionPoints);
    }
        
    public void ResetActionPoints()
    {
        remainingActionPoints = actionPoints;
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints);
    }

    public void AddActionPoints()
    {
        remainingActionPoints += actionPoints;
        if (remainingActionPoints > maxActionPoints)
        {
            remainingActionPoints = maxActionPoints;
        }
        
    }
    
    bool doOnce = true;
    public void StartTurn()
    {
        if (inCombat)
        {
            if (TurnSystem.Instance.IsThisCharactersTurn(this))
            {
                
                canMove = true;
                canAttack = true;
                characterState = CharacterState.WaitingTurn;//todo enemy turn does not change to waiting for turn
                // ResetActionPoints();

                if (!doOnce)
                {
                    AddActionPoints();
                    OnActionPointsChangeEvent?.Invoke(remainingActionPoints);
                }
                doOnce = false;
                

                if (this is Enemy)
                {
                    OnTurnStart?.Invoke();
                }

                if (this is Player)
                {
                    if(PlayerTurnStart != null) PlayerTurnStart.Invoke();
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

    public void EndTurn()
    {
        canMove = false;
        canAttack = false;
        characterState = CharacterState.WaitingNextRound;
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

    public void AttackStart()
    {
        characterState = CharacterState.Attacking;
    }
    public void AttackCancel()
    {
        characterState = CharacterState.WaitingTurn;
    }

    public void Attack()
    {
        
    }

    public void AttackEnd(SkillContainer.Skills skill)
    {
        characterState = CharacterState.WaitingTurn;
        remainingActionPoints -= skill.actionPointUse;//maybe add a - or + variable to skill use

        OnActionPointsChange?.Invoke(remainingActionPoints);
        OnActionPointsChangeEvent?.Invoke(remainingActionPoints);
        CheckIfEndTurn();
    }

    public void OnCharacterDeathFunc()
    {
        isDead = true;
        characterState = CharacterState.Dead;
        gameObject.transform.DOMoveY(gameObject.transform.position.y - 0.5f, 0.5f);
        OnCharacterDeath?.Invoke(this);
        if(GetComponent<Player>() != null) TurnSystem.Instance.PlayerDied(GetComponent<Player>());
        if(GetComponent<Enemy>() != null) TurnSystem.Instance.EnemyDied(GetComponent<Enemy>());
        
    }
}