using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
     public List<Player> players = new List<Player>();
     public List<Enemy> enemies = new List<Enemy>();
     public List<Character> allEntities = new List<Character>();
    
    private void Awake()
    {
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
        
        AddPlayersToCombat();
    }
    
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

    public void AddEnemyToCombat()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.inCombat && TurnSystem.Instance.enemiesInCombat.Contains(enemy) == false )
            {
                TurnSystem.Instance.AddEnemy(enemy);                
            }
        }
        
    }

    public void RemoveEnemyFromGame(Enemy enemy)
    {
        enemies.Remove(enemy);
        allEntities.Remove(enemy);
    }

    
}
