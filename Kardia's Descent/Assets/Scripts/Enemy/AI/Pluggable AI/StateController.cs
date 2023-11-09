using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class StateController : MonoBehaviour
{ 
    [SerializeField] public bool aiActive;
    
    public StateAI currentState;
    public StateAI defaultCombatStartState;
    public StateAI remainState;
   // public SkillsData selectedSkill;
    public EnemyStatsData enemyStats;
    [HideInInspector] public Pathfinder pathfinder;
    [HideInInspector] public TurnSystem turnSystem;
    [HideInInspector] public Enemy enemy;
    [HideInInspector] public SkillContainer skillContainer;
    
    public Player targetPlayer;
    public Tile decidedMoveTile;
    public SkillContainer.Skills decidedAttackSkill;
    
    public List<Player> players = new List<Player>();
    public List<Enemy> enemies = new List<Enemy>();
    
    public List<Tile> movableTiles = new List<Tile>();
    public List<Tile> attackableTiles = new List<Tile>();
    
    [MinMaxSlider(0,3)]
    public Vector2 RandomWaitTimer;
    

    private void OnEnable()//todo might need to turn this into an action or delegate
    {
        turnSystem.OnPlayerDeath.AddListener(PlayerDied);
        turnSystem.OnEnemyDeath.AddListener(EnemyDied);
        turnSystem.OnPlayerAdd.AddListener(PlayerAdded);
        turnSystem.OnEnemyAdd.AddListener(EnemyAdded);
        enemy.OnTurnStart += ResetToDefaultCombatState;
    }
    private void OnDisable()
    {
        turnSystem.OnPlayerDeath.RemoveListener(PlayerDied);
        turnSystem.OnEnemyDeath.RemoveListener(EnemyDied);
        turnSystem.OnPlayerAdd.RemoveListener(PlayerAdded);
        turnSystem.OnEnemyAdd.RemoveListener(EnemyAdded);
        enemy.OnTurnStart -= ResetToDefaultCombatState;
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        skillContainer = GetComponent<SkillContainer>();
        pathfinder = Pathfinder.Instance;
        turnSystem = TurnSystem.Instance;
        RandomWaitAssigner();
    }

    private void Start()
    {
        players.AddRange(turnSystem.players);
        enemies.AddRange(turnSystem.enemies);
    }

    private void Update()
    {
        aiActive = (enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder);//todo do this with events in turn system
        
        
        if (!aiActive)
        {
            return;
        }
        else
        {
            if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder)
            {
                currentState.UpdateState(this);
            }
        }
    }
    
    public void TransitionToState(StateAI nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
        }
    }

    public List<Tile> GetReachableTiles()
    {
        movableTiles = pathfinder.GetReachableTiles(enemy.characterTile, enemy.remainingActionPoints);
        return movableTiles;
    }
    
    public List<Tile> GetAttackableTiles()
    {
        attackableTiles = pathfinder.GetAttackableTiles(enemy.characterTile, skillContainer.selectedSkill.skillData.skillRange);
        return attackableTiles;
    }

    public void ResetToDefaultCombatState()
    {
        currentState = defaultCombatStartState;
    }
    
    #region Add and Remove from lists

    public void PlayerDied(Player deadPlayer)
    {
        players.RemoveRange(players.IndexOf(deadPlayer), 1);
    }
    
    public void EnemyDied(Enemy deadEnemy)
    {
        enemies.RemoveRange(enemies.IndexOf(deadEnemy), 1);
    }
    
    public void PlayerAdded(Player newPlayer)
    {
        players.Add(newPlayer);
    }
    
    public void EnemyAdded(Enemy newEnemy)
    {
        enemies.Add(newEnemy);
    }

    #endregion

    
    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
    
    private float RandomTimer;

    private float WaitTimer;
    public void RandomWaitAssigner()
    {
        WaitTimer = Random.Range(RandomWaitTimer.x, RandomWaitTimer.y);
    }
    
    public bool RandomWait()
    {
        if (RandomTimer >= WaitTimer)
        {
            RandomTimer = 0;
            return true;
        }
        else
        {
            RandomTimer += Time.deltaTime;
            return false;
        }
        
    }

    public void EndTurn()
    {
        enemy.EndTurn();
    }
}