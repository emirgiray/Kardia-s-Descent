using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private int turn = 1;
    [SerializeField] private int round = 1;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI roundText;

    private void OnEnable()
    {
        TurnSystem.Instance.TurnChange += UpdateTurn;
        TurnSystem.Instance.RoundChange += UpdateRound;
    }

    private void OnDisable()
    {
        TurnSystem.Instance.TurnChange -= UpdateTurn;
        TurnSystem.Instance.RoundChange -= UpdateRound;
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
  
}
