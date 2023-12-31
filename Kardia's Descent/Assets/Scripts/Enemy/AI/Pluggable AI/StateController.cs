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
   // public SkillsData selectedSkill;
    public EnemyStatsData enemyStats;
    public Pathfinder pathfinder;
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
        enemy.OnTurnStart += TryActivateAI;
    }
    private void OnDisable()
    {
        turnSystem.OnPlayerDeath.RemoveListener(PlayerDied);
        turnSystem.OnEnemyDeath.RemoveListener(EnemyDied);
        turnSystem.OnPlayerAdd.RemoveListener(PlayerAdded);
        turnSystem.OnEnemyAdd.RemoveListener(EnemyAdded);
        
        enemy.OnTurnStart -= ResetToDefaultCombatState;
        enemy.OnTurnStart -= TryActivateAI;
    }

    private void Awake()
    {
        FindWaypointsFromParent();
        enemy = GetComponent<Enemy>();
        skillContainer = GetComponent<SkillContainer>();
        turnSystem = TurnSystem.Instance;
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
        
        /*players.AddRange(turnSystem.players);
        enemies.AddRange(turnSystem.enemies);*/
    }

    private bool runOnce;
    private void Update()
    {
        /*if (!enemy.inCombat && !enemy.isDead)
        {
            aiActive = true;
        }*/
        
        if (!aiActive)
        {
            return;
        }
        else
        {
            
            if ((TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder) || !enemy.inCombat)
            {
                currentStateSeconds += Time.deltaTime;
                if (canExitState)
                {
                    currentStateSeconds = 0;
                    currentState.UpdateState(this);
                    
                    //Debug.Log($"STATE: {currentState}");
                    if (!runOnce && RandomWait())
                    {
                        currentState.DoActionsOnce(this);
                        runOnce = true;
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
    }

    public void TryActivateAI()
    {
        aiActive = (enemy.turnOrder == TurnSystem.Instance.currentEnemyTurnOrder && !enemy.isDead); //todo do this with events in turn system
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
        // stateTimeElapsed = 0;
        // OneTimeAnim = false; //Animasyon onetime'ını resetliyor.
        runOnce = false; //ActionOnce'ı sıfırlıyorum.
        // AnimOver = false;
        // if (Anim!=null)
        // {
        //     if (previousAnim!="")
        //     {
        //         Anim.ResetTrigger(previousAnim);
        //     }
        //         
        // }
        
        // WaitTimer = Random.Range(RandomWaitTimer.x, RandomWaitTimer.y);
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
                child.GetComponent<Waypoints>().FindTile();
                waypoints.Add(child.GetComponent<Waypoints>().waypointTile);
            }
        }
    }
    
    public List<Tile> GetReachableTiles()
    {
        movableTiles = pathfinder.GetReachableTiles(enemy.characterTile, enemy.remainingActionPoints);
        return movableTiles;
    }
    
    public List<Tile> GetAttackableTiles()
    {
        attackableTiles = pathfinder.GetAttackableTiles(enemy.characterTile, skillContainer.selectedSkill);
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
