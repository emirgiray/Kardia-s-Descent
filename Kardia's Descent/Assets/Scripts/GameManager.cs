using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public List<GameObject> SelectedPlayers = new();
    public int selectMax = 4;
    private int currentSelected = 0;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    public void StartRun()
    {
        MainPrefabScript.Instance.SelectedPlayers = SelectedPlayers;
        
        SceneChanger.Instance.LoadFirstLevel();
    }

    public void AddToSelectedPlayers(GameObject player)
    {
        if (SelectedPlayers.Count >= selectMax) return;
        SelectedPlayers.Add(player);
        currentSelected++;
    }
    
    public void RemoveFromSelectedPlayers(GameObject player)
    {
        if (SelectedPlayers.Count <= 0) return;
        SelectedPlayers.Remove(player);
        currentSelected--;
    }

}
