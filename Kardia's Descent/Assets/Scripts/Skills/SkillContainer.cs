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
    [SerializeField] private List<SkillsData> skillsDataSOList = new List<SkillsData>();
    [SerializeField] public List<Skills> skillsList = new List<Skills>();
    //[SerializeField] public SkillsData selectedSOSkill;
    [SerializeField] public Skills selectedSkill;
    [SerializeField] private Skills lastSelectedSkill;
    [SerializeField] public bool skillSelected = false;
   // [SerializeField] private bool skillCanbeUsed = true;
    [SerializeField] public Character Character;
    [SerializeField] private Inventory Inventory;
    [SerializeField] public List<SkillButton> skillButtons = new List<SkillButton>();
    
    [HideInInspector] public int damageBeforeCoverDebuff;
    [HideInInspector] public int accuracyBeforeCoverDebuff;
    
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
            if (skill.remainingSkillCooldown == skill.skillCooldown)
            {
                skill.skillButton.EnableDisableButton(Character.remainingActionPoints >= skill.actionPointUse);
            }
            //Debug.Log($"skill: {skill.skillData.name} skill ap use: {skill.actionPointUse} ap: {Character.actionPoints}, can use skill: {Character.actionPoints >= skill.actionPointUse}");
        }

    }

    private void Start()
    {
        
        /*PopulateSkillsList();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int i1 = i;
            skillButtons[i].InitButton(skillsDataSOList[i], skillsList[i] , ()=> TrySelectSkill(skillsList[i1]), this);
            
            skillsList[i].skillButton = skillButtons[i];
        }*/

        StartCoroutine(Delay());
    }

    public IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        ApplyExtraStatValues();
    }

    public void PopulateSkillsList()
    {
        // Debug.Log($"Converted {skillsDataSOList.Count} skills to skills list");
        // Debug.Log($"skillButtons {skillButtons.Count} count");
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

        for (int i = 0; i < skillsDataSOList.Count; i++)
        {
          if(Character is Player)  skillsList[i].skillButton = Inventory.spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();
            // Debug.Log($"i: {i}, skillsList[i] {skillsList[i].skillButton.name}");
            
            
        }
        
        
        // Debug.Log($"skillButtons2 {skillButtons.Count} count");
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int i1 = i;

            // Debug.Log($"i: {i}, skillsList[i] {skillsList[i].skillData.name}");
            // Debug.Log($"i: {i}, skillButtons[i] {skillButtons[i].GetComponent<SkillButton>().skillData.name}");
            
            
            skillButtons[i].InitButton(skillsDataSOList[i], skillsList[i] , ()=> TrySelectSkill(skillsList[i1]), this);
            // Debug.Log($"i: {i}, skill, skill button: {skillButtons[i].GetComponent<SkillButton>().skillData.name}");
            skillsList[i].skillButton = skillButtons[i];
        }
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
                    skillsList[i].remainingSkillCooldown++;
                    if (Character is Player)
                    {
                        skillsList[i].skillButton.cooldownText.text = (skillsList[i].skillCooldown - skillsList[i].remainingSkillCooldown).ToString();
                        skillsList[i].skillButton.cooldownImage.GetComponent<Image>().DOFillAmount(1 - (float)skillsList[i].remainingSkillCooldown / skillsList[i].skillCooldown, 2f);
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

    /// <summary>
    /// Used in combat exit
    /// </summary>
    public void ForceResetSkillCooldowns()
    {
        if (Character is Player)
        {
            for (int i = 0; i < skillsList.Count; i++)
            {
                skillsList[i].skillButton.EnableDisableButton(true);
                skillsList[i].skillButton.cooldownImage.GetComponent<Image>().DOFillAmount(0, 2f);
                skillsList[i].skillButton.cooldownText.text = "";
            }
        }
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
                if (Character.canAttack && selectSkill.skillReadyToUse)
                {
                    skillSelected = true;
                    SelectSkill(selectSkill);

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
                    if (Character.canAttack && selectSkill.skillReadyToUse)
                    {
                        SelectSkill(selectSkill);
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
        // if (Character is Enemy)Debug.Log($"{this.name} Selected {selectSkill.skillData.name}");
        
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
            if (Interact.Instance.GetCurrentTile() != null)
            {
                if (Character.characterTile != Interact.Instance.GetCurrentTile())
                {
                    Character.Rotate(Interact.Instance.GetCurrentTile().transform.position);
                }
                else
                {
                    Character.Rotate(Interact.Instance.GetLastTile().transform.position);
                }
                
            }
            if (lastSelectedSkill.skillButton != null) lastSelectedSkill.skillButton.SwitchSelectedOutline(false);
            lastSelectedSkill = selectedSkill;
            selectedSkill.skillButton.SwitchSelectedOutline(true);
            Interact.Instance.SkillSelected?.Invoke();
            Interact.Instance.HighlightAttackableTiles(selectedSkill);
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackStart();
            
        }
        else if (Character is Enemy)
        {
            Character.AttackStart();
        }
        
        // CalculateExtraStatValues(selectSkill);
    }

    public void DeselectSkill(Skills deselectSkill, Enemy enemy = null)
    {
        // ResetExtraStatValues(deselectSkill);
        skillSelected = false;
        /*skillsList.Find(x => x.skillData == selectedSkill.skillData).remainingSkillCooldown = selectedSkill.remainingSkillCooldown;
        skillsList.Find(x => x.skillData == selectedSkill.skillData).skillReadyToUse = selectedSkill.skillReadyToUse;*/
        //skillsList.Find(x => x.skillData == selectedSkill.skillData).skillButton = selectedSkill.skillButton;
        
        if (Character is Player)
        {
            if (deselectSkill.skillButton != null)
            {
                deselectSkill.skillButton.SwitchSelectedOutline(false);
            }
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
        selectedSkill = null;
        
        
    }
    
bool impact = false;


/// <summary>
/// 
/// </summary>
/// <param name="selectedSkill"></param>
/// <param name="selectedTile"></param>
/// <param name="enemy">currently only used in enemy</param>
/// <param name="OnComplete">currently only used in enemy</param>
    public void UseSkill(Skills selectedSkill, Tile selectedTile, Enemy enemy = null, Action OnComplete = null)
    {
        SetImpact(false);
        /*if (Character is Enemy && !enemy.canAttack)
        {
            enemy.EndTurn();
            return;
        }*/

        // if (Character is Enemy)Debug.Log($"{this.name} Used {selectedSkill.skillData.name}");

        if (selectedSkill.skillCooldown != 0)
        {
            selectedSkill.remainingSkillCooldown = 0;
            selectedSkill.skillReadyToUse = false;
            // skillNotReadyAction?.Invoke();
            if (Character is Player)
            {
                selectedSkill.skillButton.cooldownImage.gameObject.SetActive(true);
                selectedSkill.skillButton.cooldownImage.GetComponent<Image>().DOFillAmount(1, 1f);
                selectedSkill.skillButton.cooldownText.text = (selectedSkill.skillCooldown).ToString();
                selectedSkill.skillButton.EnableDisableButton(false);
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
        //Debug.Log($"anim length: {attackAnimLength}");
        //Debug.Log($"skill lenght: {overrides[3].Key.length}");
    }

    public IEnumerator AttackCancelDelay(float attackAnimLength, Skills selectedSkill, Tile selectedTile, Enemy enemy = null, Action OnComplete = null)
    {
        // yield return new WaitForSecondsRealtime(attackAnimLength);
        
        selectedSkill.skillData.ActivateSkill(selectedSkill, Character, selectedTile,  () =>
        {
            DeselectSkill(selectedSkill, enemy);
            ResetCoverAccruacyDebuff();
            ResetCoverdamageDebuff();

            OnComplete?.Invoke();
            
            if (Character is Player)
            {
                Interact.Instance.selectedCharacter.GetComponent<Character>().AttackEnd(selectedSkill);
            }
            else if (Character is Enemy)
            {
                enemy.AttackEnd(selectedSkill);
            }
                
        });
        
        
        
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
        PopulateSkillsList();
    }

    
    //todo delete this and calcuale for half damage on cover
    public int CalculateCoverAccuracyDebuff(Tile attacker, Tile defender, Skills selectSkill)
    {
        if (Character.pathfinder.CheckCoverPoint(attacker, defender, true) && selectSkill.skillData.skillType == SkillsData.SkillType.Ranged)
        {
            // Debug.Log($"accuracy debuff: {selectSkill.coverAccuracyDebuff}");
            return selectSkill.coverAccuracyDebuff;
        }
        // Debug.Log($"accuracy debuff returned default: 0");
        return 0;
    }

    public int CalculateCoverDamageDebuff(Tile attacker, Tile defender, Skills selectSkill)
    {
        if (Character.pathfinder.CheckCoverPoint(attacker, defender, true) && selectSkill.skillData.skillType == SkillsData.SkillType.Ranged)
        {
            return selectSkill.damage / 2;
        }
        return 0;
    }
    

    public void ApplyCoverAccuracyDebuff()
    {
        accuracyBeforeCoverDebuff = selectedSkill.accuracy;
        selectedSkill.accuracy -= selectedSkill.coverAccuracyDebuff;
    }

    public void ApplyCoverDamageDebuff()
    {
        damageBeforeCoverDebuff = selectedSkill.damage;
        selectedSkill.damage /= 2;
        //Debug.Log($"damge before cover debuff: {damageBeforeCoverDebuff}, damage after cover debuff: {selectedSkill.damage}");
    }
    
    public void ResetCoverAccruacyDebuff()
    {
        if (selectedSkill == null || accuracyBeforeCoverDebuff == 0)
        {
            return;
        }
        
        selectedSkill.accuracy = accuracyBeforeCoverDebuff;
    }

    public void ResetCoverdamageDebuff()
    {
        if (selectedSkill == null || damageBeforeCoverDebuff == 0)
        {
            return;
        }
        
        selectedSkill.damage = damageBeforeCoverDebuff;
    }

    public void ApplyExtraStatValues()
    {
        foreach (var skill in skillsList)
        {
            if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
            {
                skill.accuracy += Character.extraRangedAccuracy;
            }
            else if (skill.skillData.skillType == SkillsData.SkillType.Melee)
            {
                skill.damage += Character.extraMeleeDamage;
            }
        }
        
        
    }
    
    /*public void CalculateExtraStatValues(Skills skill)
    {
        // apply accuracy and damage bonuses from stats here the reset after complete
        if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
        {
            skill.accuracy += Character.extraRangedAccuracy;
        }
        else if (skill.skillData.skillType == SkillsData.SkillType.Melee)
        {
            skill.damage += Character.extraMeleeDamage;
        }
    }

    public void ResetExtraStatValues(Skills skill)
    {
        if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
        {
            skill.accuracy -= Character.extraRangedAccuracy;
        }
        else if (skill.skillData.skillType == SkillsData.SkillType.Melee)
        {
            skill.damage -= Character.extraMeleeDamage;
        }
    }*/
    
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