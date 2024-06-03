using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Character
{
    [SerializeField] private StateController stateController;

    [SerializeField] private GameObject hpBar;
    
    public int turnOrder = 0; //this is based on initiative and changes every round according to initiative changes

    public List<Character> killOnDeath = new();
    
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
        
        base.OnCharacterDeath += KillOnDeath;
    }

    private void OnDisable()
    {
        everythingUseful.TurnSystem.EnemyTurn -= base.StartTurn;
        everythingUseful.TurnSystem.OnEnemyCheckStunTurnEvent -= CheckRemoveStun;
        
        base.OnCharacterDeath -= KillOnDeath;
    }

    public void HPBarDelay()
    {
        hpBar.SetActive(true);
    }

    private void KillOnDeath(Character character)
    {
        foreach (var go in killOnDeath)
        {
            go.health.Kill();
        }
    }

    
    /*private void Update()
    {
        if (characterState == CharacterState.Attacking)
        { 
            Rotate(transform.position, stateController.targetPlayer.transform.position);
        }
    }*/
}