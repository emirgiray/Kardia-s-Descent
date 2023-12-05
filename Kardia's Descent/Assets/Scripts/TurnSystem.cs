using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }
    [SerializeField] public List<Player> players = new List<Player>();
    [SerializeField] public List<Enemy> enemies = new List<Enemy>();
    [SerializeField] public List<GameObject> allEntities = new List<GameObject>();

    public int turn = 0;
    public int round = 1;
    public int totalTurnsInScene = 1;
    public int totalTurnsInGame = 1;
    public int currentEnemyTurnOrder = 0;
    
    public enum TurnState
    {
        None, Friendly, Enemy
    }
    public TurnState turnState;
    
    [SerializeField] private GameObject RoundInfo;
    [SerializeField] private GameObject CharacterCardPrefab;
    
    public Action FriendlyTurn;
    public Action FriendlySelected;
    public Action EnemyTurn;
    public Action RoundChanged;
    public Action OnPlayerTurnEvent;
    public Action OnEnemyTurnEvent;
    public Action<int> TurnChange;
    public Action<int> RoundChange;
    [FoldoutGroup("Events")] public UnityEvent OnTurnChange;
    [FoldoutGroup("Events")] public UnityEvent<TextMeshProUGUI> OnTurnChangeUpdateText;
    [FoldoutGroup("Events")] public UnityEvent OnRoundChange;
    [FoldoutGroup("Events")] public UnityEvent<TextMeshProUGUI> OnRoundChangeUpdateText;
    [FoldoutGroup("Events")] public UnityEvent OnCombatStart;
    [FoldoutGroup("Events")] public UnityEvent OnPlayerTurn;
    [FoldoutGroup("Events")] public UnityEvent OnEnemyTurn;
    [FoldoutGroup("Events")] public UnityEvent<Player> OnPlayerDeath;
    [FoldoutGroup("Events")] public UnityEvent<Player> OnPlayerAdd;
    [FoldoutGroup("Events")] public UnityEvent<Enemy> OnEnemyDeath;
    [FoldoutGroup("Events")] public UnityEvent<Enemy> OnEnemyAdd;


    void Awake()
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
        
        players.AddRange(GameObject.FindObjectsOfType<Player>());
        enemies.AddRange(GameObject.FindObjectsOfType<Enemy>());

        for (int i = 0; i < players.Count; i++)
        {
            allEntities.Add(players[i].gameObject);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            allEntities.Add(enemies[i].gameObject);
        }

        for (int i = 0; i < allEntities.Count; i++)//set character cards, todo: also add these to character add/remove
        {
            GameObject newCard = Instantiate(CharacterCardPrefab, RoundInfo.transform);
            newCard.name = allEntities[i].name + " Card";
            newCard.GetComponent<CharacterRoundCard>().Init(allEntities[i].GetComponent<Character>(), RoundInfo.GetComponent<RoundInfo>());
            allEntities[i].GetComponent<Character>().SetCharacterCard(newCard);
            RoundInfo.GetComponent<RoundInfo>().AddObject(newCard);
            //newCard.GetComponent<CustomEvent3>().CustomEvents3.AddListener(RoundInfo.GetComponent<RoundInfo>().Rearrange);
        }
        
    }

    private void Start()
    {
        totalTurnsInGame = PlayerPrefs.GetInt("totalTurnsInGame", 1);
        PlayerPrefs.SetInt("totalTurnsInGame", totalTurnsInGame);
        DecideWhosTurn();
    }

    private void Update()//todo delete this, its for debug only
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NextRound();
        }
    }


    public void DecideWhosTurn()//todo bu isimlendirme yanlış, decideWhosRound olmalı
    {
        if (round % 2 == 1)
        {
            turnState = TurnState.Friendly;
            OnPlayerTurn.Invoke();
            OnEnemyTurnEvent?.Invoke();
            if(FriendlyTurn != null) FriendlyTurn();
        }
        if (round % 2 == 0)
        {
            turnState = TurnState.Enemy;
            OnEnemyTurn.Invoke();
            OnPlayerTurnEvent?.Invoke();
            if(EnemyTurn != null) EnemyTurn();
        }
        if (round == 0)//Combat has not started yet
        {
            turnState = TurnState.None;
            return;
            /*FriendlyTurn();*/
        }
        RoundChanged?.Invoke();
    }

    [Button,GUIColor(0,1,0)][FoldoutGroup("DEBUG")]
    public void CombatStarted()
    {
        turn = 1;
        OnCombatStart.Invoke();
    }
    
    [Button,GUIColor(0,1,0)][FoldoutGroup("DEBUG")]
    public void NextTurn()
    {
        turn++;
        totalTurnsInScene++;
        totalTurnsInGame++;
        PlayerPrefs.SetInt("totalTurnsInGame", totalTurnsInGame);
        TurnChange?.Invoke(turn);
        OnTurnChange.Invoke();
        if (turnState == TurnState.Friendly)
        {
            if (turn > players.Count)
            {
                NextRound();
            }
        }
        else if (turnState == TurnState.Enemy)
        {
            TurnSystem.Instance.currentEnemyTurnOrder++;
            if (turn > enemies.Count)
            {
                NextRound();
                //TurnSystem.Instance.currentEnemyTurnOrder = 0;
            }
        }
        /*else if (turnState == TurnState.None
        {
            
        }
        if (turn >= allEntities.Count)
        {
            NextRound();
        }*/
    }

    public void DecideEnemyTurnOrder()//this will work according to enemy initiative //todo this is reversed!!!!
    {
        List<Tuple<Enemy, int>> enemyList = new List<Tuple<Enemy, int>>();
        foreach (Enemy enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript!= null)
            {
                enemyList.Add(new Tuple<Enemy, int>(enemy, enemyScript.initiative));
            }
        }

        enemyList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].Item1.GetComponent<Enemy>().turnOrder = i;
        }

    }
    
    [Button,GUIColor(0,1,0)][FoldoutGroup("DEBUG")]
    public void NextRound()
    {
        turn = 1;
        TurnChange?.Invoke(turn);
        round++;
        RoundChange?.Invoke(round);
        OnRoundChange.Invoke();
        DecideWhosTurn();
        TurnSystem.Instance.currentEnemyTurnOrder = 0;
    }

    [Button,GUIColor(1,1,1)][FoldoutGroup("DEBUG")]
    public void PlayerDied(Player deadPlayer)
    {
        players.RemoveRange(players.IndexOf(deadPlayer), 1);
        allEntities.RemoveRange(allEntities.IndexOf(deadPlayer.gameObject), 1);
        OnPlayerDeath.Invoke(deadPlayer);
    }

    [Button,GUIColor(1,1,1)][FoldoutGroup("DEBUG")]
    public void EnemyDied(Enemy deadEnemy)
    {
        enemies.RemoveRange(enemies.IndexOf(deadEnemy), 1);
        allEntities.RemoveRange(allEntities.IndexOf(deadEnemy.gameObject), 1);
        OnEnemyDeath.Invoke(deadEnemy);
    }
    
    [Button,GUIColor(0,0,1)][FoldoutGroup("DEBUG")]
    public void PlayerAdded(Player newPlayer)
    {
       players.Add(newPlayer);//todo might need to change this to addrange
       allEntities.Add(newPlayer.gameObject);
       OnPlayerAdd.Invoke(newPlayer);
    }
    
    [Button,GUIColor(0,0,1)][FoldoutGroup("DEBUG")]
    public void EnemyAdded(Enemy newEnemy)
    {
        enemies.Add(newEnemy);
        allEntities.Add(newEnemy.gameObject);
        OnEnemyAdd.Invoke(newEnemy);
    }

    public void UpdateTurnText(TextMeshProUGUI turnText)
    {
        // turnText.text = "Turn: " + turn;
         turnText.text = turn.ToString();
    }
    public void UpdateRoundText(TextMeshProUGUI roundText)
    {
        // roundText.text = "Round: " + round;
        roundText.text = round.ToString();
    }

    public bool IsThisCharactersTurn(Character character)
    {
        if (turnState == TurnState.Friendly)
        {
            if (character is Player)
            {
                return true;
            }
        }
        else if (turnState == TurnState.Enemy)
        {
            if (character is Enemy)
            {
                DecideEnemyTurnOrder();
                return true;
            }
        }
        return false;
    }
}
