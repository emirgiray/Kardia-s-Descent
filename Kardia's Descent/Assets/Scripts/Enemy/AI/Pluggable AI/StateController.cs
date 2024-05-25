using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class StateController : MonoBehaviour
{ 
    [SerializeField] public bool aiActive;
    [SerializeField] public bool canExitState = true;
    [SerializeField] public float currentStateSeconds = 0;
    
    public StateAI currentState;
    public StateAI defaultCombatStartState;
    public StateAI defaultNonCombatState;
    public StateAI remainState;
    public StateAI endTurnState;
   // public SkillsData selectedSkill;
    public EnemyStatsData enemyStats;
    public Pathfinder pathfinder;
    public MonoBehaviour stateControllerMono;
    [HideInInspector] public TurnSystem turnSystem;
    [HideInInspector] public Enemy enemy;
    [HideInInspector] public SkillContainer skillContainer;
    
    public Tile targetPlayerTile;
    public Tile decidedMoveTile;
    public Tile forcedTargetPlayerTile;
    public SkillContainer.Skills decidedAttackSkill;
    public SkillContainer.Skills lastUsedSkill;
    public SkillContainer.Skills forcedSkillToUse;
    
    public List<Player> players = new List<Player>();
    public List<Enemy> enemies = new List<Enemy>();
    
    public List<Tile> movableTiles = new List<Tile>();
    public List<Tile> attackableTiles = new List<Tile>();

    [MinMaxSlider(0,3)]
    public Vector2 RandomWaitTimer;
    [MinMaxSlider(0,3)]
    public Vector2 RandomWaitTimer2;
    
    [SerializeField] private GameObject waypointsParent;
    public List<Tile> waypoints = new List<Tile>();
    

    private void OnEnable()//todo might need to turn this into an action or delegate
    {
        turnSystem.OnPlayerDeath.AddListener(PlayerDied);
        turnSystem.OnEnemyDeath.AddListener(EnemyDied);
        turnSystem.OnPlayerAdd.AddListener(PlayerAdded);
        turnSystem.OnEnemyAdd.AddListener(EnemyAdded);
        
        
        enemy.OnTurnStart += ResetToDefaultCombatState;
        // enemy.OnTurnStart += TryActivateAI;
        turnSystem.TurnChanged += TryActivateAI;
    }
    private void OnDisable()
    {
        turnSystem.OnPlayerDeath.RemoveListener(PlayerDied);
        turnSystem.OnEnemyDeath.RemoveListener(EnemyDied);
        turnSystem.OnPlayerAdd.RemoveListener(PlayerAdded);
        turnSystem.OnEnemyAdd.RemoveListener(EnemyAdded);
        
        enemy.OnTurnStart -= ResetToDefaultCombatState;
        // enemy.OnTurnStart -= TryActivateAI;
        turnSystem.TurnChanged -= TryActivateAI;
    }

    private void Awake()
    {
        if (waypointsParent != null) FindWaypointsFromParent();
        enemy = GetComponent<Enemy>();
        
        skillContainer = GetComponent<SkillContainer>();
        turnSystem = enemy.everythingUseful.TurnSystem;
        RandomWaitAssigner();
        RandomWaitAssigner2();

        if (!enemy.inCombat)
        {
            currentState = defaultNonCombatState;
        }
        else
        {
            currentState = defaultCombatStartState;
        }
 
    }

    private void Start()
    {
        enemy.enabled = true; //this was getting disabled for some reason????
        /*players.AddRange(turnSystem.players);
        enemies.AddRange(turnSystem.enemies);*/
    }

    private bool runOnce;
    private bool runOnce2 = true;
    private void Update()
    {
        /*if (!enemy.inCombat && !enemy.isDead)
        {
            aiActive = true;
        }*/

        if (enemy.isDead && runOnce2 && turnSystem.turnState == TurnSystem.TurnState.Enemy && enemy.diedOnSelfTurn) // this will only happen if an enemy dies in its turn for example from a suicide skill
        {
            // currentState =endTurnState;
            EndTurn();
            runOnce2 = false;
        }
        
        
        if (!aiActive)
        {
            return;
        }
        else
        {
            
            if ((/*enemy.TurnSystem.IsThisCharactersTurn(enemy) &&*/ enemy.TurnSystem.turnState == TurnSystem.TurnState.Enemy && enemy.turnOrder == enemy.TurnSystem.currentEnemyTurnOrder) || !enemy.inCombat)
            {
                currentStateSeconds += Time.deltaTime;
                if (canExitState)
                {
                    currentStateSeconds = 0;
                    if (currentState != null)
                    {
                        currentState.UpdateState(this);

                        //Debug.Log($"STATE: {currentState}");
                        if (!runOnce && RandomWait())
                        {
                            currentState.DoActionsOnce(this);
                            runOnce = true;
                        }
                    }
                }
                else
                {
                    if (currentStateSeconds >= 20) //state overtime
                    {
                        canExitState = true;
                        Debug.Log($"State changed due to overtime"); //doesnt work??
                    }
                }
            }
            
            
        }
    }

    public void StartCombat()
    {
        ResetToDefaultCombatState();
        
        players.Clear();
        enemies.Clear();
        foreach (var var in turnSystem.playersInCombat)
        {
            players.Add(var);
            
        }
        
        foreach (var var in turnSystem.enemiesInCombat)
        {
            enemies.Add(var);
            
        }
        /*players = turnSystem.playersInCombat;
        enemies = turnSystem.enemiesInCombat;*/
    }

    private void TryActivateAI()
    {
        aiActive = (enemy.turnOrder == enemy.TurnSystem.currentEnemyTurnOrder && !enemy.isDead); 
    }

    public void SetAIActive(bool value)
    {
        aiActive = value;
    }
    public void TransitionToState(StateAI nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }
    
    private void OnExitState()
    {
        runOnce = false; //ActionOnce'ı sıfırlıyorum.
        
        RandomWaitAssigner2();
    }

    [Button, GUIColor(1f, 1f, 1f)]
    public void FindWaypointsFromParent()
    {
        waypoints.Clear();
        foreach (Transform child in waypointsParent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.GetComponent<Waypoints>().FindNewTile();
                waypoints.Add(child.GetComponent<Waypoints>().waypointTile);
            }
        }
    }
    
    public List<Tile> GetReachableTiles()
    {
        movableTiles = pathfinder.GetReachableTiles(enemy.characterTile, enemy.remainingActionPoints);
        return movableTiles;
    }
    

    public void ResetToDefaultCombatState()
    {
        currentState = defaultCombatStartState;
    }
    
    #region Add and Remove from lists

    public void PlayerDied(Player deadPlayer)
    {
        players.Remove(deadPlayer);
    }
    
    public void EnemyDied(Enemy deadEnemy)
    {
        enemies.Remove(deadEnemy);
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
    private float RandomTimer2;

    private float WaitTimer;
    private float WaitTimer2;
    public void RandomWaitAssigner()
    {
        WaitTimer = Random.Range(RandomWaitTimer.x, RandomWaitTimer.y);
        
    }
    public void RandomWaitAssigner2()
    {
        WaitTimer2 = Random.Range(RandomWaitTimer2.x, RandomWaitTimer2.y);
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

    public bool RandomWait2()
    {
        if (RandomTimer2 >= WaitTimer2)
        {
            RandomTimer2 = 0;
            return true;
        }
        else
        {
            RandomTimer2 += Time.deltaTime;
            return false;
        }
        
    }
    
    public void EndTurn()
    {
        enemy.EndTurn();
    }
}
