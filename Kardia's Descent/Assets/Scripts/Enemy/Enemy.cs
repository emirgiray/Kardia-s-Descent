using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Character
{
    [SerializeField] private StateController stateController;

    [SerializeField] private GameObject hpBar;
    
    public int turnOrder = 0; //this is based on initiative and changes every round according to initiative changes
    public enum DecideState
    {
        Patrolling, Moving, Attacking, Defending, Idle
    }
    public DecideState decideState;
    private void OnEnable()
    {
        everythingUseful.TurnSystem.EnemyTurn += base.StartTurn;
        //base.canMove = false; //todo denemek için yaptım belki değiştirilebilir
        stateController = GetComponent<StateController>();
        everythingUseful.TurnSystem.OnEnemyCheckStunTurnEvent += CheckRemoveStun;
    }

    private void OnDisable()
    {
        everythingUseful.TurnSystem.EnemyTurn -= base.StartTurn;
        everythingUseful.TurnSystem.OnEnemyCheckStunTurnEvent -= CheckRemoveStun;
    }

    public void HPBarDelay()
    {
        hpBar.SetActive(true);
    }
    
    /*private void Update()
    {
        if (characterState == CharacterState.Attacking)
        { 
            Rotate(transform.position, stateController.targetPlayer.transform.position);
        }
    }*/
}