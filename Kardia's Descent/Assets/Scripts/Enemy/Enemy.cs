using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Character
{
    [SerializeField] private StateController stateController;
    
    public int turnOrder = 0; //this is based on initiative and changes every round according to initiative changes
    public enum DecideState
    {
        Patrolling, Moving, Attacking, Defending, Idle
    }
    public DecideState decideState;
    private void OnEnable()
    {
        TurnSystem.Instance.EnemyTurn += base.StartTurn;
        //base.canMove = false; //todo denemek için yaptım belki değiştirilebilir
        stateController = GetComponent<StateController>();
        TurnSystem.Instance.OnEnemyCheckStunTurnEvent += CheckRemoveStun;
    }

    private void OnDisable()
    {
        TurnSystem.Instance.EnemyTurn -= base.StartTurn;
        TurnSystem.Instance.OnEnemyCheckStunTurnEvent -= CheckRemoveStun;
    }

    /*private void Update()
    {
        if (characterState == CharacterState.Attacking)
        { 
            Rotate(transform.position, stateController.targetPlayer.transform.position);
        }
    }*/
}