using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [BoxGroup("Turn And Round")] [SerializeField]
    private int turn = 1;
    [BoxGroup("Turn And Round")] [SerializeField]
    private int round = 1;
    [BoxGroup("Turn And Round")] [SerializeField]
    private GameObject turnAndRoundGO;
    [BoxGroup("Turn And Round")] [SerializeField]
    private TextMeshProUGUI turnText;
    [BoxGroup("Turn And Round")] [SerializeField]
    private TextMeshProUGUI roundText;

    [BoxGroup("Pause Menu")] [SerializeField]
    private GameObject PauseScreenGO;
    
    [BoxGroup("End Game")] [SerializeField]
    private GameObject WinLoseScreenGO;
    [BoxGroup("End Game")] [SerializeField]
    private GameObject WinTextGO;
    [BoxGroup("End Game")] [SerializeField]
    private GameObject LoseTextGO;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI scoreText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalDamageDealtText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalDamageTakenText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalKillsText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalHeartsCollectedText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalPlayTimeText;
    [BoxGroup("End Game")] [SerializeField]
    private Image bestHeartImage;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalTurnsText;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        TurnSystem.Instance.TurnChange += UpdateTurn;
        TurnSystem.Instance.RoundChange += UpdateRound;
        TurnSystem.Instance.OnCombatStartAction += ()=> turnAndRoundGO.SetActive(true);
        TurnSystem.Instance.OnCombatEndAction += ()=> turnAndRoundGO.SetActive(false);
    }

    private void OnDisable()
    {
        TurnSystem.Instance.TurnChange -= UpdateTurn;
        TurnSystem.Instance.RoundChange -= UpdateRound;
        TurnSystem.Instance.OnCombatStartAction -= ()=> turnAndRoundGO.SetActive(true);
        TurnSystem.Instance.OnCombatEndAction -= ()=> turnAndRoundGO.SetActive(false);
    }

    private void Start()
    {
        UpdateTurn(TurnSystem.Instance.turn);
        UpdateRound(TurnSystem.Instance.round);
    }

    private void UpdateTurn(int turn)
    {
        turnText.text = turn.ToString();
    }

    private void UpdateRound(int round)
    {
        roundText.text = round.ToString();
    }

    public void GameOver(bool win, int score, int damageDealt, int damageTaken, int kills, int heartsCollected, string playTime)
    {
        WinLoseScreenGO.SetActive(true);
        scoreText.text = score.ToString();
        totalDamageDealtText.text = damageDealt.ToString();
        totalDamageTakenText.text = damageTaken.ToString();
        totalKillsText.text = kills.ToString();
        totalHeartsCollectedText.text = heartsCollected.ToString();
        totalPlayTimeText.text = playTime;
        
        int prevRarity = 0;
        foreach (var player in GameManager.Instance.players)
        {
            if (player.isUnlocked && player.heartContainer.heartData != null)
            {
                if ((int)player.heartContainer.heartData.heartRarity >= prevRarity)
                {
                    prevRarity = (int) player.heartContainer.heartData.heartRarity;
                    bestHeartImage.sprite = player.heartContainer.heartData.HeartSprite;
                }
            }
            
        }
        
        //totalTurnsText.text = TurnSystem.Instance.totalTurnsInGame.ToString();
        if (win)
        {
            WinTextGO.SetActive(true);
        }
        else
        {
            LoseTextGO.SetActive(true);
        }
    }
    
    public void PauseGame(bool value)
    {
        PauseScreenGO.SetActive(value);
    }
    
}
