using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
     public List<Player> players = new List<Player>();
     public List<Enemy> enemies = new List<Enemy>();
     public List<Character> allEntities = new List<Character>();

     public UnityEvent<Transform> PlayerUnlockedEventTransform;

     [BoxGroup("Stats For End Game")]
     public int totalDamageDealt;
     [BoxGroup("Stats For End Game")]
     public int totalDamageTaken;
     [BoxGroup("Stats For End Game")]
     public int totalKills;
     [BoxGroup("Stats For End Game")]
     public int totalHeartsCollected;
     [BoxGroup("Stats For End Game")] [SerializeField] 
     private DateTime startTime;
     [BoxGroup("Stats For End Game")] [SerializeField]
     private TimeSpan playTime;
     [BoxGroup("Stats For End Game")] [SerializeField]
     private string formattedPlayTime;
     [BoxGroup("Stats For End Game")] [SerializeField]
     private int score;
    private void Awake()
    {
        startTime = DateTime.Now;
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        players.AddRange(GameObject.FindObjectsOfType<Player>(true));
        enemies.AddRange(GameObject.FindObjectsOfType<Enemy>(true));

        for (int i = 0; i < players.Count; i++)
        {
            allEntities.Add(players[i]);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            allEntities.Add(enemies[i]);
        }
        
        // AddPlayersToCombat();
    }

    #region Add/Remove Players/Enemies

    /// <summary>
    /// Can be called anytime, only adds players that are active in the scene and not already in combat
    /// </summary>
    public void AddPlayersToCombat()
    {
        foreach (var player in players)
        {
            if (player.gameObject.activeInHierarchy && TurnSystem.Instance.playersInCombat.Contains(player) == false )
            {
                TurnSystem.Instance.AddPlayer(player);
                
            }
        }
    }
    
    public void AddPlayerToCombat(Player player)
    {
        if (player.gameObject.activeInHierarchy && TurnSystem.Instance.playersInCombat.Contains(player) == false && player.GetUnlocked())
        {
            TurnSystem.Instance.AddPlayer(player);                
        }
    }

    public void AddEnemiesToCombat()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.inCombat && TurnSystem.Instance.enemiesInCombat.Contains(enemy) == false )
            {
                TurnSystem.Instance.AddEnemy(enemy);                
            }
        }
        
    }

    public void AddEnemyToCombat(Enemy enemy)
    {
        if (enemy.gameObject.activeInHierarchy && TurnSystem.Instance.enemiesInCombat.Contains(enemy) == false )
        {
            TurnSystem.Instance.AddEnemy(enemy);                
        }
    }

    public void RemoveEnemyFromGame(Enemy enemy)
    {
        enemies.Remove(enemy);
        allEntities.Remove(enemy);
    }
    
    public void RemovePlayerFromGame(Player player)
    {
        players.Remove(player);
        allEntities.Remove(player);

        if (players.Count == 0)
        {
            GameOver(false);
        }
    }

    public void PlayerUnlocked(Transform playerTransform)
    {
        PlayerUnlockedEventTransform?.Invoke(playerTransform);
    }

    #endregion


    public void GameOver(bool win)
    {
        CalcuatePlayTime();
        CalculateScore();
        UIManager.Instance.GameOver(win, score, totalDamageDealt, totalDamageTaken, totalKills, totalHeartsCollected, formattedPlayTime);
    }
    

    [Button, GUIColor(1f, 1f, 1f)]
    public void CalcuatePlayTime()
    {
        playTime = DateTime.Now - startTime;
        int hours = (int)playTime.TotalHours;
        int minutes = playTime.Minutes;
        int seconds = playTime.Seconds;
        formattedPlayTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        
    }

    public void CalculateScore()
    {
        score = (totalDamageDealt * 10) + (totalKills * 10) + (totalHeartsCollected * 20) - totalDamageTaken - (int)playTime.TotalSeconds;
    }
}
