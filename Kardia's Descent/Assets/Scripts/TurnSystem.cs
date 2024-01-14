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
    [SerializeField] public List<Player> playersInCombat = new List<Player>();
    [SerializeField] public List<Enemy> enemiesInCombat = new List<Enemy>();
    [SerializeField] public List<Character> allEntitiesInCombat = new List<Character>();
    public GameObject combatStartedImage;

    public int turn = 0;
    public int round = 1;
    public int totalTurnsInScene = 1;
    public int totalTurnsInGame = 1;
    public int currentEnemyTurnOrder = 0;
    
    public enum TurnState
    {
        FreeRoamTurn, Friendly, Enemy
    }
    public TurnState turnState;
    
    [SerializeField] private GameObject RoundInfo;
    [SerializeField] private GameObject CharacterCardPrefab;
    
    public Action FriendlyTurn;
    public Action FriendlySelected;
    public Action OnCombatStartAction;
    public Action OnCombatEndAction;
    public Action EnemyTurn;
    public Action RoundChanged;
    public Action TurnChanged;
    public Action OnPlayerTurnEvent;
    public Action OnPlayerCheckStunTurnEvent;
    public Action OnEnemyTurnEvent;
    public Action OnEnemyCheckStunTurnEvent;
    public Action<int> TurnChange;
    public Action<int> RoundChange;
    [FoldoutGroup("Events")] public UnityEvent OnTurnChange;
    [FoldoutGroup("Events")] public UnityEvent<TextMeshProUGUI> OnTurnChangeUpdateText;
    [FoldoutGroup("Events")] public UnityEvent OnRoundChange;
    [FoldoutGroup("Events")] public UnityEvent<TextMeshProUGUI> OnRoundChangeUpdateText;
    [FoldoutGroup("Events")] public UnityEvent OnPlayerTurn;
    [FoldoutGroup("Events")] public UnityEvent OnEnemyTurn;
    [FoldoutGroup("Events")] public UnityEvent<Player> OnPlayerDeath;
    [FoldoutGroup("Events")] public UnityEvent<Player> OnPlayerAdd;
    [FoldoutGroup("Events")] public UnityEvent<Transform> OnPlayerAddTransform;
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
        
        /*players.AddRange(GameObject.FindObjectsOfType<Player>());
        enemies.AddRange(GameObject.FindObjectsOfType<Enemy>());

        for (int i = 0; i < players.Count; i++)
        {
            allEntities.Add(players[i].gameObject);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            allEntities.Add(enemies[i].gameObject);
        }*/

        /*for (int i = 0; i < allEntities.Count; i++)//set character cards,
        {
            GameObject newCard = Instantiate(CharacterCardPrefab, RoundInfo.transform);
            newCard.name = allEntities[i].name + " Card";
            newCard.GetComponent<CharacterRoundCard>().Init(allEntities[i].GetComponent<Character>(), RoundInfo.GetComponent<RoundInfo>());
            allEntities[i].GetComponent<Character>().SetCharacterCard(newCard);
            RoundInfo.GetComponent<RoundInfo>().AddObject(newCard);
            //newCard.GetComponent<CustomEvent3>().CustomEvents3.AddListener(RoundInfo.GetComponent<RoundInfo>().Rearrange);
        }*/
        
    }

    private void Start()
    {
        totalTurnsInGame = PlayerPrefs.GetInt("totalTurnsInGame", 1);
        PlayerPrefs.SetInt("totalTurnsInGame", totalTurnsInGame);
        DecideWhosTurn();
    }

    /*private void Update()//todo delete this, its for debug only
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NextRound();
        }
#endif
    }*/


    public void DecideWhosTurn()//todo bu isimlendirme yanlış, decideWhosRound olmalı
    {
        if (round % 2 == 1)
        {
            turnState = TurnState.Friendly;
            OnPlayerTurn.Invoke();
            OnPlayerTurnEvent?.Invoke();
             OnEnemyCheckStunTurnEvent?.Invoke(); //stun events are reversed because it wasnt working properly before idk why
            if(FriendlyTurn != null) FriendlyTurn();
        }
        if (round % 2 == 0)
        {
            turnState = TurnState.Enemy;
            OnEnemyTurn.Invoke();
             OnPlayerCheckStunTurnEvent?.Invoke();
            OnEnemyTurnEvent?.Invoke();
            if(EnemyTurn != null) EnemyTurn();
        }
        if (round == 0)//Combat has not started yet
        {
            turnState = TurnState.FreeRoamTurn;
            //return;
            /*FriendlyTurn();*/
        }
        RoundChanged?.Invoke();
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
            if (turn > playersInCombat.Count)
            {
                NextRound();
            }
        }
        else if (turnState == TurnState.Enemy)
        {
            TurnSystem.Instance.currentEnemyTurnOrder++;
            if (turn > enemiesInCombat.Count)
            {
                NextRound();
                //TurnSystem.Instance.currentEnemyTurnOrder = 0;
            }
        }
        TurnChanged?.Invoke();
        /*else if (turnState == TurnState.None
        {
            
        }
        if (turn >= allEntities.Count)
        {
            NextRound();
        }*/
    }

    [Button,GUIColor(0,1,0)][FoldoutGroup("DEBUG")]
    public void CombatStarted()
    {
        combatStartedImage.SetActive(true);
        turn = 1;
        round = 1;
        TurnChange?.Invoke(turn);
        OnTurnChange.Invoke();
        OnCombatStartAction?.Invoke();
        RoundChange?.Invoke(round);
        OnRoundChange.Invoke();
        DecideWhosTurn();
    }

    [Button, GUIColor(1f, 1f, 1f)]
    public void CombatEnded()
    {
        turn = 0;
        round = 0;
        OnCombatEndAction?.Invoke();
        DecideWhosTurn();
    }
    
    public void DecideEnemyTurnOrder()//this will work according to enemy initiative //todo this is reversed!!!!
    {
        List<Tuple<Enemy, int>> enemyList = new List<Tuple<Enemy, int>>();
        foreach (Enemy enemy in enemiesInCombat)
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
        playersInCombat.RemoveRange(playersInCombat.IndexOf(deadPlayer), 1);
        allEntitiesInCombat.RemoveRange(allEntitiesInCombat.IndexOf(deadPlayer), 1);
        OnPlayerDeath.Invoke(deadPlayer);
        RemoveCard(deadPlayer);
        GameManager.Instance.RemovePlayerFromGame(deadPlayer);
    }
    
    [Button,GUIColor(1,1,1)][FoldoutGroup("DEBUG")]
    public void PlayerExitedCombat(Player deadPlayer)
    {
        playersInCombat.RemoveRange(playersInCombat.IndexOf(deadPlayer), 1);
        allEntitiesInCombat.RemoveRange(allEntitiesInCombat.IndexOf(deadPlayer), 1);
        RemoveCard(deadPlayer);
    }

    [Button,GUIColor(1,1,1)][FoldoutGroup("DEBUG")]
    public void EnemyDied(Enemy deadEnemy)
    {
        if (enemiesInCombat.Contains(deadEnemy))
        {
            enemiesInCombat.RemoveRange(enemiesInCombat.IndexOf(deadEnemy), 1);
            allEntitiesInCombat.RemoveRange(allEntitiesInCombat.IndexOf(deadEnemy), 1);
            OnEnemyDeath.Invoke(deadEnemy);
            RemoveCard(deadEnemy);

            GameManager.Instance.RemoveEnemyFromGame(deadEnemy);
        
            if (enemiesInCombat.Count <= 0)
            {
                CombatEnded();
                List<Player> playersToRemove = new List<Player>();
                foreach (var player in playersInCombat)
                {
                    playersToRemove.Add(player);
                }

                foreach (var player in playersToRemove)
                {
                    player.ExitCombat();
                }
            }
        }
    }
    
    [Button,GUIColor(0,0,1)][FoldoutGroup("DEBUG")]
    public void AddPlayer(Player newPlayer)
    {
       playersInCombat.Add(newPlayer);//todo might need to change this to addrange
       allEntitiesInCombat.Add(newPlayer);
       AddCard(newPlayer);
       OnPlayerAdd.Invoke(newPlayer);
       OnPlayerAddTransform.Invoke(newPlayer.transform); //this adds the new player to fog of war
    }
    
    [Button,GUIColor(0,0,1)][FoldoutGroup("DEBUG")]
    public void AddEnemy(Enemy newEnemy)
    {
        enemiesInCombat.Add(newEnemy);
        allEntitiesInCombat.Add(newEnemy);

        AddCard(newEnemy);
        OnEnemyAdd.Invoke(newEnemy);
        
    }

    public void AddCard(Character character)
    {
        //for (int i = 0; i < allEntities.Count; i++)//set character cards,
        {
            GameObject newCard = Instantiate(CharacterCardPrefab, RoundInfo.transform);
            newCard.name = character.name + " Card";
            newCard.GetComponent<CharacterRoundCard>().Init(character, RoundInfo.GetComponent<RoundInfo>());
            character.SetCharacterCard(newCard);
            // RoundInfo.GetComponent<RoundInfo>().AddObject(newCard, character);
            StartCoroutine(AddCardDelay(newCard, character));
            /*if (turnState == TurnState.Friendly)
            {
                if (character is Player)
                {
                    newCard.transform.SetAsFirstSibling();
                }
                if (character is Enemy)
                {
                    newCard.transform.SetAsLastSibling();
                }
                
            }
            else if (turnState == TurnState.Enemy)
            {
                if (character is Player)
                {
                    newCard.transform.SetAsLastSibling();
                }
                if (character is Enemy)
                {
                    newCard.transform.SetAsFirstSibling();
                }
            }*/
            
            //newCard.GetComponent<CustomEvent3>().CustomEvents3.AddListener(RoundInfo.GetComponent<RoundInfo>().Rearrange);
        }
    }

    public IEnumerator AddCardDelay(GameObject obj, Character character)
    {
        yield return new WaitForEndOfFrameUnit();
        RoundInfo.GetComponent<RoundInfo>().AddObject(obj, character);
        
    }

    public void RemoveCard(Character character)
    {
        RoundInfo.GetComponent<RoundInfo>().RemoveObject(character.GetCharacterCard());
        Destroy(character.GetCharacterCard());
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
        else// freeroam
        {
            if (character is Player)
            {
                return true;
            }
        }
        return false;
    }
}
