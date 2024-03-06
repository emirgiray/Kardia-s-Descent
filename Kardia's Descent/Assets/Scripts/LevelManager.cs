using System;
using System.Collections;
using System.Collections.Generic;
using FischlWorks_FogWar;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
     public List<Player> players = new List<Player>();
     public List<Enemy> enemies = new List<Enemy>();
     [HideInInspector] 
     public List<Character> allEntities = new List<Character>();
     
     [HideInInspector] 
     public List<GameObject> playersGO = new();
     
     [FoldoutGroup("Events")]
     public UnityEvent GamePausedEvent; //these are not used
     [FoldoutGroup("Events")]
     public UnityEvent GameUnpausedEvent;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
    }

    public void InitializeCharacters()
    {
        players.Clear();
        enemies.Clear();
        allEntities.Clear();
        playersGO.Clear();
        
        
        players.AddRange(FindObjectsOfType<Player>(true));

        foreach (var player in players)
        {
            playersGO.Add(player.gameObject);
        }
        
        enemies.AddRange(FindObjectsOfType<Enemy>(true));

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

    public void AddPlayerToGame(Player player)
    {
        
        players.Add(player);
        allEntities.Add(player);
        playersGO.Add(player.gameObject);
    }
    
    public void RemovePlayerFromGame(Player player)
    {
        players.Remove(player);
        allEntities.Remove(player);

        List<Player> unlockedPlayers = new List<Player>();
        
        foreach (var player2 in players)
        {
            if (player2.isUnlocked)
            {
                unlockedPlayers.Add(player2);
            }
        }
        
        playersGO.Remove(player.gameObject);

        if (players.Count == 0 || unlockedPlayers.Count == 0)
        {
            GameManager.Instance.GameOver(false);
        }
    }

    public void PlayerUnlocked(Transform playerTransform)
    {
        csFogWar fogWar = FindObjectOfType<csFogWar>();
        fogWar.AddFogRevealerRevelear(playerTransform);
        MainPrefabScript.Instance.SelectedPlayers.Add(playerTransform.gameObject);
    }

    #endregion


   
}
