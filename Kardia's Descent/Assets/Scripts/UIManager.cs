using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private int turn = 1;
    [SerializeField] private int round = 1;
    
    [SerializeField] private GameObject turnAndRoundGO;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI roundText;

    [SerializeField] private GameObject WinScreenGO;
    [SerializeField] private GameObject LoseScreenGO;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalDamageDealtText;
    [SerializeField] private TextMeshProUGUI totalDamageTakenText;
    [SerializeField] private TextMeshProUGUI totalKillsText;
    [SerializeField] private TextMeshProUGUI totalHeartsCollectedText;
    [SerializeField] private TextMeshProUGUI totalPlayTimeText;
    [SerializeField] private TextMeshProUGUI totalturnsText;
    

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
        scoreText.text = score.ToString();
        totalDamageDealtText.text = damageDealt.ToString();
        totalDamageTakenText.text = damageTaken.ToString();
        totalKillsText.text = kills.ToString();
        totalHeartsCollectedText.text = heartsCollected.ToString();
        totalPlayTimeText.text = playTime;
        totalturnsText.text = TurnSystem.Instance.totalTurnsInGame.ToString();
        if (win)
        {
            WinScreenGO.SetActive(true);
        }
        else
        {
            LoseScreenGO.SetActive(true);
        }
    }
    

    
}
