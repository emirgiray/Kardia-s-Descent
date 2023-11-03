using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class StateController : MonoBehaviour
{
    [SerializeField] public StateAI currentState;
    
    [SerializeField] public Enemy enemy;
    [SerializeField] public SkillsData selectedSkill;
    [SerializeField] public EnemyStatsData enemyStats;
    [SerializeField] public Pathfinder pathfinder;
    [SerializeField] public List<Player> players = new List<Player>();
    [SerializeField] public List<Enemy> enemies = new List<Enemy>();
    [SerializeField] public Player targetPlayer;
    [MinMaxSlider(0,3)]
    public Vector2 RandomWaitTimer;
    
    [SerializeField] bool aiActive;
    

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        pathfinder = Pathfinder.Instance;
        WaitRandom();
    }

    private void Update()
    {
        if (!aiActive)
        {
            return;
        }
        else
        {
            currentState.UpdateState(this);
        }
    }

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
    public void WaitRandom()
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
}
