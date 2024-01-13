using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private RenderTexture playerPortrait;
    
    [FoldoutGroup("Stats For Quests")] [SerializeField]
    private int killsInTurn = 0;
    [ShowIf("characterClass", CharacterClass.Bruiser)]
    [SerializeField] private bool canBruiserDoubleAttack = true;
    public bool isUnlocked = false;
    
    public delegate void QuestDelegate();
    public static event QuestDelegate JackTheRipperQuestCompleted;
    private void OnEnable()
    {
        TurnSystem.Instance.FriendlyTurn += base.StartTurn;
        TurnSystem.Instance.OnPlayerCheckStunTurnEvent += CheckRemoveStun;
        TurnSystem.Instance.OnPlayerTurnEvent += ResetKillsInTurn;

        if (characterClass == CharacterClass.Bruiser)
        {
            OnAttackEndAction += BruiserDoubleAttack;
            TurnSystem.Instance.FriendlyTurn += ResetBruiserDoubleAttack;
        }
    }
    private void OnDisable()
    {
        TurnSystem.Instance.FriendlyTurn -= base.StartTurn;
        TurnSystem.Instance.OnPlayerCheckStunTurnEvent -= CheckRemoveStun;
        TurnSystem.Instance.OnPlayerTurnEvent -= ResetKillsInTurn;
        
        if (characterClass == CharacterClass.Bruiser)
        {
            OnAttackEndAction -= BruiserDoubleAttack;
            TurnSystem.Instance.FriendlyTurn -= ResetBruiserDoubleAttack;
        }
    }

    private void Start()
    {
        if (isUnlocked == false)
        {
            StartCoroutine(StartDelay());
        }

    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        animator.enabled = false;
    }
    public void BruiserDoubleAttack(SkillContainer.Skills skill)
    {
        if (canBruiserDoubleAttack && skill.skillData.name.Contains("Basic"))
        {
            remainingActionPoints += skill.actionPointUse;
            skill.remainingSkillCooldown = skill.skillCooldown;
            canBruiserDoubleAttack = false;
            Vector3 pos = Interact.Instance.lastAttackedTile.transform.position;
            Rotate(pos);
            SkillContainer.UseSkill(skill, Interact.Instance.lastAttackedTile);
            
        }
    }

    public void ResetBruiserDoubleAttack()
    {
        canBruiserDoubleAttack = true;
    }
    public void CheckeHeartQuest()
    {
        if(heartContainer.heartData == null) return;

        if (heartContainer.heartData.heartIndex == 0) //jack the ripper
        {
            if (killsInTurn >= 3)
            {
                heartContainer.isPowerUnlocked = true;
                JackTheRipperQuestCompleted?.Invoke();
            }
        }
    }
  
    public void IncreaseKills()
    {
        killsInTurn++;
    }

    private void ResetKillsInTurn()
    {
        killsInTurn = 0;
    }
    
    public void UnlockPlayer()
    {
        isUnlocked = true;
        animator.enabled = true;
    }

    public bool GetUnlocked()
    {
        return isUnlocked;
    }

    public RenderTexture GetPlayerPortrait()
    {
        return playerPortrait;
    }

    [Button]
    public List<Tile> gettilesinbetween(Tile dest)
    {
        return pathfinder.GetTilesInBetween(this, characterTile, dest, true);
    }

    
    /*private void Update()
    {
        if (characterState == CharacterState.Attacking)
        {
            //Rotate(transform.position, Interact.Instance.currentTile.transform.position);
            if (Interact.Instance.isMouseOverUI == false)
            {
                // StartCoroutine(RotateEnum(transform.position, Interact.Instance.currentTile.transform.position));
                Rotate(transform.position, Interact.Instance.currentTile.transform.position);
            }
        }
    }*/

    /*private void Start()
    {
        switch (characterClass)
        {
            case CharacterClass.None:
                break;
            case CharacterClass.Tank:
                break;
            case CharacterClass.Rogue:
                break;
            case CharacterClass.Sniper:
                break;
            case CharacterClass.Bombardier:
                break;
            case CharacterClass.TheRegular:
                break;
            case CharacterClass.Medic:
                break;
            case CharacterClass.Support:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }*/

   
}
