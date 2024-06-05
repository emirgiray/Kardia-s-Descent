using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
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

    [SerializeField] private Material PauseRendererEffectReset;
    
    
    [FoldoutGroup("Events")]
    public UnityEvent GamePausedEvent;
    [FoldoutGroup("Events")]
    public UnityEvent GameUnpausedEvent;
    [FoldoutGroup("Events")]
    public UnityEvent RunStartedEvent;
    
    public void StartRun() // this fires once the player presses start
    {
        RunStartedEvent?.Invoke();
        MainPrefabScript.Instance.SelectedPlayers = SelectedPlayers;
        startTime = DateTime.Now;
        everythingUseful.SaveLoadSystem.loadOnAwake = true;
        // save selected players to save system

        foreach (var player in SelectedPlayers)
        {
            everythingUseful.LevelManager.AddPlayerToGame(player.GetComponent<Player>());
            everythingUseful.MainPrefabScript.spawnedPlayers.Add(player);
        }
        

        everythingUseful.SaveLoadSystem.SaveGame();
        everythingUseful.MainPrefabScript.MainCamera.enabled = true;
        everythingUseful.SaveLoadSystem.inGameSaveData.lastScene = everythingUseful.SceneChanger.firstLevel;
        everythingUseful.SceneChanger.LoadFirstLevel();
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

    public void ResetToDefault()
    {
        everythingUseful.Interact.ResetToDefault();
    }
    
    #region Game States
    
    public void GameOver(bool win)
    {
        CalcuatePlayTime();
        CalculateScore();
        everythingUseful.UIManager.GameOver(win, score, totalDamageDealt, totalDamageTaken, totalKills, totalHeartsCollected, formattedPlayTime);
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
        score = (totalDamageDealt * 10) + (totalKills * 100) + (totalHeartsCollected * 20) - totalDamageTaken * (int)playTime.TotalSeconds / 100;
        if (score < 0) score = 0;
       
    }
    
    public void PauseGame(bool value)
    {
        isPaused = !isPaused;
        everythingUseful.UIManager.PauseGame(isPaused);
        everythingUseful.Interact.isPaused = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        (isPaused ? GamePausedEvent : GameUnpausedEvent)?.Invoke();
    }

    private void OnApplicationQuit()
    {
        PauseRendererEffectReset.SetFloat("_Dissolve", 1);
        //DOTween.KillAll();
    }

    public void SetPauseRendererEffect(float value)
    {
        PauseRendererEffectReset.SetFloat("_Dissolve", value);
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
