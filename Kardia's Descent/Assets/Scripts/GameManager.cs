using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public List<GameObject> SelectedPlayers = new();
    public int selectMax = 4;
    private int currentSelected = 0;
    
    [SerializeField] private bool isPaused = false;
    [BoxGroup("Stats For End Game")]
    public int totalDamageDealt;
    [BoxGroup("Stats For End Game")]
    public int totalDamageTaken;
    [BoxGroup("Stats For End Game")]
    public int totalKills;
    [BoxGroup("Stats For End Game")]
    public int totalHeartsCollected;
    [BoxGroup("Stats For End Game")] 
    public DateTime startTime;
    [BoxGroup("Stats For End Game")] [SerializeField]
    private TimeSpan playTime;
    [BoxGroup("Stats For End Game")] [SerializeField]
    private string formattedPlayTime;
    [BoxGroup("Stats For End Game")] [SerializeField]
    private int score;
     
    [BoxGroup("Options")] [SerializeField]
    private bool fpsOn = false;
    [BoxGroup("Options")] [SerializeField]
    private GameObject fpsGO;
    [BoxGroup("Options")] [SerializeField]
    private TextMeshProUGUI fpsText;
    
    [FoldoutGroup("Events")]
    public UnityEvent GamePausedEvent;
    [FoldoutGroup("Events")]
    public UnityEvent GameUnpausedEvent;
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
        startTime = DateTime.Now;
        SaveLoadSystem.Instance.loadOnAwake = true;
        // save selected players to save system

        foreach (var player in SelectedPlayers)
        {
            LevelManager.Instance.AddPlayerToGame(player.GetComponent<Player>());
        }
        
        SaveLoadSystem.Instance.SaveGame();
        MainPrefabScript.Instance.MainCamera.enabled = true;
        SaveLoadSystem.Instance.saveData.lastScene = SceneChanger.Instance.firstLevel;
        SceneChanger.Instance.LoadFirstLevel();
    }

    /// <summary>
    /// Returns true if the player is added to the list in the main menu, false if the list is full
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool AddToSelectedPlayers(GameObject player)
    {
        if (SelectedPlayers.Count >= selectMax) return false;
        SelectedPlayers.Add(player);
        currentSelected++;
        return true;
    }
    
    public bool RemoveFromSelectedPlayers(GameObject player)
    {
        if (SelectedPlayers.Count <= 0) return false;
        SelectedPlayers.Remove(player);
        currentSelected--;
        return true;
        
    }
    
    #region Game States
    
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

    private void CalculateScore()
    {
        score = (totalDamageDealt * 10) + (totalKills * 10) + (totalHeartsCollected * 20) - totalDamageTaken * (int)playTime.TotalSeconds / 10;
    }
    
    public void PauseGame(bool value)
    {
        isPaused = !isPaused;
        UIManager.Instance.PauseGame(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        (isPaused ? GamePausedEvent : GameUnpausedEvent)?.Invoke();
    }

    #endregion

    #region Options

    public void ToggleFPS()
    {
        fpsOn = !fpsOn;
        fpsGO.SetActive(fpsOn);
        if (fpsOn)
        {
            StartCoroutine(FPS());
        }
        else
        {
            StopCoroutine(FPS());
        }
    }

    private IEnumerator FPS()
    {
        while (fpsOn)
        {
            yield return new WaitForSecondsRealtime(1);
            fpsText.text = (1f / Time.unscaledDeltaTime).ToString("0");
        }
    }

    #endregion
}
