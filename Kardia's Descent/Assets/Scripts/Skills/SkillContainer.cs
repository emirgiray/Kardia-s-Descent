using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] public List<SkillsData> skillsDataSOList = new List<SkillsData>();
    [SerializeField] public List<Skills> skillsList = new List<Skills>();
    //[SerializeField] public SkillsData selectedSOSkill;
    [SerializeField] public Skills selectedSkill;
    [SerializeField] public bool skillSelected = false;
    [SerializeField] public bool skillCanbeUsed = true;
    [SerializeField] public Character Character;
    [SerializeField] public Inventory Inventory;
    [SerializeField] public List<SkillButton> skillButtons = new List<SkillButton>();
    
    private static LTDescr delay;
    // public Action skillNotReadyAction;
    //public Action skillReadyAction;

    private void OnEnable()
    {
        Character = GetComponent<Character>();
        Inventory = GetComponent<Inventory>();
        if (Character is Player)
        {
            Character.OnActionPointsChange += CheckIfEnoughAPForSkill;
            
        }
        
        TurnSystem.Instance.RoundChanged += ResetSkillCooldowns;
        
    }

    private void OnDisable()
    {
        if (Character is Player)
        {
            Character.OnActionPointsChange -= CheckIfEnoughAPForSkill;
        }
        
        
        TurnSystem.Instance.RoundChanged -= ResetSkillCooldowns;
    }

    public void CheckIfEnoughAPForSkill(int ap)
    {
        
        foreach (var skill in skillsList)
        {
            skill.skillButton.EnableDisableButton(Character.remainingActionPoints >= skill.actionPointUse);
            //Debug.Log($"skill: {skill.skillData.name} skill ap use: {skill.actionPointUse} ap: {Character.actionPoints}, can use skill: {Character.actionPoints >= skill.actionPointUse}");
        }

    }

    private void Start()
    {
        
        PopulateSkillsList();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int i1 = i;
            skillButtons[i].InitButton(skillsDataSOList[i], skillsList[i] , ()=> TrySelectSkill(skillsList[i1]), this);
            
            skillsList[i].skillButton = skillButtons[i];
        }

        
    }


    public void PopulateSkillsList()
    {
        skillsList.AddRange(skillsDataSOList.ConvertAll(skill => new Skills
        {
            skillData = skill,
            damage = skill.skillDamage,
            range = skill.skillRange,
            skillCooldown = skill.skillCooldown,
            accuracy = skill.accuracy,
            coverAccuracyDebuff = skill.coverAccuracyDebuff,
            skillEffectDuration = skill.skillEffectDuration,
            skillBuffDebuffAmount = skill.skillBuffDebuffAmount,
            actionPointUse = skill.actionPointUse,
            remainingSkillCooldown = skill.skillCooldown,
            skillButton = this.skillButtons.Find(x => x.skillData == skill),//this doesnt work???
            animatorOverrideController = skill.animatorOverrideController
        }));
    }
    
    public void ResetSkillCooldowns()
    {
        if (TurnSystem.Instance.IsThisCharactersTurn(Character))
        {
            //Debug.Log($"Character {Character.name} {TurnSystem.Instance.turnState} turn started, resetting skill cooldowns");
            for (int i = 0; i < skillsList.Count; i++)
            {
                if (skillsList[i].remainingSkillCooldown < skillsList[i].skillCooldown)
                {
                    if (/*TurnSystem.Instance.turnState == TurnSystem.TurnState.Friendly &&*/ Character is Player)
                    {
                        skillsList[i].remainingSkillCooldown++;
                        skillsList[i].skillButton.cooldownText.text = (skillsList[i].skillCooldown - skillsList[i].remainingSkillCooldown).ToString(); 
                        skillsList[i].skillButton.cooldownImage.GetComponent<Image>().DOFillAmount(1 -(float)skillsList[i].remainingSkillCooldown / skillsList[i].skillCooldown , 2f).OnComplete((
                            () =>
                            {
                                // skillsList[i].skillReadyToUse = true;
                                if (Character is Player)
                                {
                                    skillsList[i].skillButton.EnableDisableButton(true);//todo maybe also check the skip button interactable
                                    skillsList[i].skillButton.cooldownImage.SetActive(false);
                                    skillsList[i].skillButton.cooldownText.text = "";//todo doesnt work??
                                }
                            }));
                        // skillsList[i].skillButton.cooldownImage.GetComponent<Image>().fillAmount = (float)skillsList[i].remainingSkillCooldown / skillsList[i].skillCooldown;
                    }
                    if (/*TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && */Character is Enemy)
                    {
                        skillsList[i].remainingSkillCooldown++;
                    }
                
                }

                if (skillsList[i].remainingSkillCooldown == skillsList[i].skillCooldown)
                {
                    skillsList[i].skillReadyToUse = true;
                    if (Character is Player)
                    {
                        skillsList[i].skillButton.EnableDisableButton(true);//todo maybe also check the skip button interactable
                        // skillsList[i].skillButton.cooldownImage.SetActive(false);
                        skillsList[i].skillButton.cooldownText.text = "";
                    }
                 
                }
                // skillsList[i].remainingSkillCooldown = skillsList[i].skillCooldown;
            
            }
        }
        //skillReadyAction?.Invoke();
    }
    
    public void SkillHighlighted(SkillsData highlightSkill)
    {
        Interact.Instance.SkillHighlighted?.Invoke();
    }

    public void TrySelectSkill(Skills selectSkill)
    {
        if (Character.characterState == Character.CharacterState.WaitingTurn || Character.characterState == Character.CharacterState.Idle || Character.characterState == Character.CharacterState.Attacking)
        {
            if (Character.actionPoints < selectSkill.actionPointUse)
            {
                Debug.Log("Not enough AP");
                return;
            }
            
            if (skillSelected == false) //if no skill is selected select that sakill
            {
                if (Character.canAttack )
                {
                    // if (selectedSkill.skillReadyToUse)
                    {
                        skillSelected = true;
                        SelectSkill(selectSkill);
                    }
                    // else
                    // {
                    //     Debug.Log("Skill not ready to use");
                    // }
                }
            }
            else
            {
                if (selectSkill == selectedSkill) //if the skill is already selected deselect it
                {
                    DeselectSkill(selectSkill);
                }
                else //if a skill is already selected and a new skill is selected, deselect the old skill and select the new one
                {
                    if (Character.canAttack)
                    {
                        // if (selectedSkill.skillReadyToUse)
                        {
                            SelectSkill(selectSkill);
                        }
                        // else
                        // {
                        //     Debug.Log("Skill not ready to use");
                        // }
                    }
                }
            }
        }
    }

    public void SetAnimAtorOverrides(AnimatorOverrideController animatorOverrideController)
    {
        Character.SetAnimations(animatorOverrideController);
    }


    
    public void SelectSkill(Skills selectSkill, Enemy enemy = null)
    {
        /*if (Character is Enemy && !enemy.canAttack)
        {
            enemy.EndTurn();
            return;
        }*/
        if (Character is Enemy)Debug.Log($"{this.name} Selected {selectSkill.skillData.name}");
        
        /*selectedSkill = skillsList.Find(x => x.skillData == selectSkill);
        selectedSkill.skillData = selectSkill;
        selectedSkill.remainingSkillCooldown = skillsList.Find(x => x.skillData == selectedSkill.skillData).remainingSkillCooldown;
        selectedSkill.skillCooldown = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillCooldown;
        selectedSkill.skillReadyToUse = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillReadyToUse;
        selectedSkill.skillButton = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillButton;*/
        
        selectedSkill = selectSkill;
        if (selectedSkill.animatorOverrideController != null)
        {
            SetAnimAtorOverrides(selectedSkill.animatorOverrideController);
        }
        
        if (Character is Player)
        {
            Interact.Instance.SkillSelected?.Invoke();
            Interact.Instance.HighlightAttackableTiles();
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackStart();
            
        }
        else if (Character is Enemy)
        {
            Character.AttackStart();
        }
    }

    public void DeselectSkill(Skills deselectSkill, Enemy enemy = null)
    {
        if (Character is Enemy)Debug.Log($"{this.name} Deselected {deselectSkill.skillData.name}");

        skillSelected = false;
        /*skillsList.Find(x => x.skillData == selectedSkill.skillData).remainingSkillCooldown = selectedSkill.remainingSkillCooldown;
        skillsList.Find(x => x.skillData == selectedSkill.skillData).skillReadyToUse = selectedSkill.skillReadyToUse;*/
        //skillsList.Find(x => x.skillData == selectedSkill.skillData).skillButton = selectedSkill.skillButton;
        selectedSkill = null;
        

        if (Character is Player)
        {
            Interact.Instance.ClearHighlightAttackableTiles();
            Interact.Instance.SkillDeselected?.Invoke();
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackCancel();
            StopCoroutine(Interact.Instance.HitChanceUIToMousePos());  
            Interact.Instance.HitChanceUIGameObject.SetActive(false);
        }
        else if (Character is Enemy)
        {
            enemy.AttackCancel();
        }
    }
    
bool impact = false;
    public void UseSkill(Skills selectedSkill, Tile selectedTile, Enemy enemy = null, Action OnComplete = null)
    {
        SetImpact(false);
        /*if (Character is Enemy && !enemy.canAttack)
        {
            enemy.EndTurn();
            return;
        }*/

        if (Character is Enemy)Debug.Log($"{this.name} Used {selectedSkill.skillData.name}");

        if (this.selectedSkill.skillCooldown != 0)
        {
            this.selectedSkill.remainingSkillCooldown = 0;
            this.selectedSkill.skillReadyToUse = false;
            // skillNotReadyAction?.Invoke();
            if (Character is Player)
            {
                this.selectedSkill.skillButton.EnableDisableButton(false);
                this.selectedSkill.skillButton.cooldownImage.gameObject.SetActive(true);
                this.selectedSkill.skillButton.cooldownText.text = (this.selectedSkill.skillCooldown).ToString();
            }
        }
        
        // write the sequence of anim clips 
        /*for (int i = 0; i < Character.animator.runtimeAnimatorController.animationClips.Length;  i++)
        {
            Debug.Log($"index: {i}, name: {Character.animator.runtimeAnimatorController.animationClips[i].name} Length: {Character.animator.runtimeAnimatorController.animationClips[i].length} ");
        }*/
        float attackAnimLength = 0;
        
        for (int i = 0; i < Character.animator.runtimeAnimatorController.animationClips.Length;  i++)
        {
            if (Character.animator.runtimeAnimatorController.animationClips[i].name.Contains("attack"))
            {
                attackAnimLength = Character.animator.runtimeAnimatorController.animationClips[i].length + 0.8666667f; //+ exit blend length
            }
           
        }
        
        StartCoroutine(AttackCancelDelay(attackAnimLength, selectedSkill, selectedTile, enemy, OnComplete));
        Character.Attack();
        Debug.Log($"anim length: {attackAnimLength}");
        //Debug.Log($"skill lenght: {overrides[3].Key.length}");
        
        
        
        /*selectedSkill.skillData.ActivateSkill(selectedSkill, Character, selectedTile, gameObject,  () =>
        {
            //animationClips[3] is the useSkill animation
            delay = LeanTween.delayedCall(attackAnimLength/*Character.animator.runtimeAnimatorController.animationClips[3].length#1#, () =>
            {
                DeselectSkill(selectedSkill, enemy);
                ResetCoverAccruacyDebuff();
                OnComplete?.Invoke();
            });
            
           
            
        });
        
        if (Character is Player)
        {
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackEnd(selectedSkill);
        }
        else if (Character is Enemy)
        {
            enemy.AttackEnd(selectedSkill);
        }*/
    }

    public IEnumerator AttackCancelDelay(float attackAnimLength, Skills selectedSkill, Tile selectedTile, Enemy enemy = null, Action OnComplete = null)
    {
        // yield return new WaitForSecondsRealtime(attackAnimLength);
        selectedSkill.skillData.ActivateSkill(selectedSkill, Character, selectedTile, gameObject,  () =>
        {

                DeselectSkill(selectedSkill, enemy);
                ResetCoverAccruacyDebuff();
                OnComplete?.Invoke(); 
        });
        
        if (Character is Player)
        {
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackEnd(selectedSkill);
        }
        else if (Character is Enemy)
        {
            enemy.AttackEnd(selectedSkill);
        }
        
        yield return null;
    }

    public void SetImpact(bool value)
    {
        impact = value;
    }

    public bool GetImpact()
    {
        return impact;
    }
    
    public void AddSkills(List<SkillsData> skillsDataListIn)
    {
        skillsDataSOList.AddRange(skillsDataListIn);
    }

    public int CalculateCoverAccuracyDebuff(Tile attacker, Tile defender, Skills selectSkill)
    {
        if (Pathfinder.Instance.CheckCoverPoint(attacker, defender))
        {
            Debug.Log($"accuracy debuff: {selectSkill.coverAccuracyDebuff}");
            return selectSkill.coverAccuracyDebuff;
        }
        Debug.Log($"accuracy debuff returned default: 0");
        return 0;
    }
    
    public void ApplyCoverAccuracyDebuff()
    {
        selectedSkill.accuracy -= selectedSkill.coverAccuracyDebuff;
    }

    public void ResetCoverAccruacyDebuff()//todo !!!!!!this doesnt calculate buffs/debuffs before cover check
    {
        if (selectedSkill == null)
        {
            return;
        }
        
        selectedSkill.accuracy = selectedSkill.skillData.accuracy;
    }
    
    [System.Serializable]
    public class Skills
    {
        public SkillsData skillData;
        public int damage;
        public int range;
        public int skillCooldown;
        public int remainingSkillCooldown;
        public int actionPointUse;
        public int skillEffectDuration;
        public int skillBuffDebuffAmount;
        public int accuracy;
        public int coverAccuracyDebuff;
        public bool skillReadyToUse = true;
        public SkillButton skillButton;
        public AnimatorOverrideController animatorOverrideController;

    }

    
}