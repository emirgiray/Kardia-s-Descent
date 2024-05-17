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
    [SerializeField] public Skills lastSelectedSkill;
    [SerializeField] public bool skillSelected = false;
   // [SerializeField] private bool skillCanbeUsed = true;
    [SerializeField] public Character Character;
    [SerializeField] private Inventory Inventory;
    [SerializeField] public List<SkillButton> skillButtons = new List<SkillButton>();
    
    [HideInInspector] public int damageBeforeCoverDebuff;
    [HideInInspector] public int accuracyBeforeCoverDebuff;
    
    // these are for the area skill effected tiles (bombadier)
    [HideInInspector] public List<Tile> effectedTiles;
    [HideInInspector] public List<Tile> innerEffectedTiles;
    [HideInInspector] public List<Tile> outerEffectedTiles;
    private static LTDescr delay;
    public float coverDamageMultiplier = 1;
    public float otherDamageMultiplier = 1;
    public float rangedCloseDamageMultiplier = 1;
    public bool characterTooClose = false;
    // public Action skillNotReadyAction;
    //public Action skillReadyAction;

    private void OnEnable()
    {
        if (Character is Player)
        {
            Character.OnActionPointsChange += CheckIfEnoughAPForSkill;
        }
        
        Character.everythingUseful.TurnSystem.RoundChanged += ResetSkillCooldowns;
    }

    private void OnDisable()
    {
        if (Character is Player)
        {
            Character.OnActionPointsChange -= CheckIfEnoughAPForSkill;
        }
        
        Character.everythingUseful.TurnSystem.RoundChanged -= ResetSkillCooldowns;
    }

    public void CheckIfEnoughAPForSkill(int ap)
    {
        
        foreach (var skill in skillsList)
        {
            if (skill.remainingSkillCooldown == skill.skillCooldown)
            {
                skill.skillButton.EnableDisableButton(Character.remainingActionPoints >= skill.actionPointUse);
            }
            //Debug.Log($"skill: {skill.skillData.name} skill ap use: {skill.actionPointUse} ap: {Character.remainingActionPoints}, can use skill: {Character.remainingActionPoints >= skill.actionPointUse}");
        }

    }

    private void Start()
    {
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
        if (Character.TurnSystem.IsThisCharactersTurn(Character))
        {
            //Debug.Log($"Character {Character.name} {Character.TurnSystem.turnState} turn started, resetting skill cooldowns");
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
                skillsList[i].skillReadyToUse = true;
                skillsList[i].skillButton.EnableDisableButton(true);
                skillsList[i].skillButton.cooldownImage.GetComponent<Image>().DOFillAmount(0, 2f);
                skillsList[i].skillButton.cooldownText.text = "";
            }
        }
    }
    
    public void SkillHighlighted(SkillsData highlightSkill)
    {
        Character.Interact.SkillHighlighted?.Invoke();
    }

    public void TrySelectSkill(Skills selectSkill)
    {
        if (Character.characterState == Character.CharacterState.WaitingTurn || Character.characterState == Character.CharacterState.Idle || Character.characterState == Character.CharacterState.Attacking)
        {
            if (Character.remainingActionPoints < selectSkill.actionPointUse)
            {
                Debug.Log("Not enough AP");
                return;
            }

            if (selectSkill.skillData.passiveOrActive == SkillsData.PassiveOrActive.Passive)
            {
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
        selectedSkill = selectSkill;
        
        if (selectedSkill.animatorOverrideController != null)
        {
            SetAnimAtorOverrides(selectedSkill.animatorOverrideController);
        }
        
        if (Character is Player)
        {
            if (Character.Interact.GetCurrentTile() != null)
            {
                if (Character.characterTile != Character.Interact.GetCurrentTile())
                {
                    Character.Rotate(Character.Interact.GetCurrentTile().transform.position);
                }
                else
                {
                    if (Character.Interact.GetLastTile() != null)
                    {
                        Character.Rotate(Character.Interact.GetLastTile().transform.position);
                    }
                }
                
            }
            if (lastSelectedSkill.skillButton != null) lastSelectedSkill.skillButton.SwitchSelectedOutline(false);
            lastSelectedSkill = selectedSkill;
            selectedSkill.skillButton.SwitchSelectedOutline(true);
            Character.Interact.SkillSelected?.Invoke();
            Character.AttackStart();
            Character.Interact.HighlightAttackableTiles(selectedSkill);
            
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
            Character.Interact.ClearHighlightAttackableTiles();
            Character.Interact.SkillDeselected?.Invoke();
            Character.AttackCancel();
            StopCoroutine(Character.Interact.HitChanceUIToMousePos());

            /*foreach (var skill in skillsList)
            {
                if (Character.remainingActionPoints < skill.actionPointUse)
                {
                    skill.skillButton.EnableDisableButton(false);
                }
            }*/

            Character.Interact.HitChanceUIGameObject.SetActive(false);
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
        
        Character.Attack();
        StartCoroutine(AttackCancelDelay(attackAnimLength, selectedSkill, selectedTile, enemy, OnComplete));
        //Debug.Log($"anim length: {attackAnimLength}");
        //Debug.Log($"skill lenght: {overrides[3].Key.length}");
    }

    private IEnumerator AttackCancelDelay(float attackAnimLength, Skills selectedSkill, Tile selectedTile, Enemy enemy = null, Action OnComplete = null)
    {
        if (Character is Player)
        {
            if (selectedSkill.skillData.skillType == SkillsData.SkillType.Ranged && characterTooClose)
            {
                rangedCloseDamageMultiplier = 0.5f;
            }
        }
        else
        {
            if (Character.characterClass == Character.CharacterClass.Ranged && characterTooClose)
            {
                rangedCloseDamageMultiplier = 0.5f;
            }
        }
        float multipliars = coverDamageMultiplier * otherDamageMultiplier * rangedCloseDamageMultiplier * selectedSkill.SkillDamageMultipliar;
        //Debug.Log($"multipliars: {multipliars}, coverDamageMultiplier: {coverDamageMultiplier}, otherDamageMultiplier: {otherDamageMultiplier}, rangedCloseDamageMultiplier: {rangedCloseDamageMultiplier}, selectedSkill.SkillDamageMultipliar: {selectedSkill.SkillDamageMultipliar}");
        selectedSkill.skillData.ActivateSkill(selectedSkill, Character, selectedTile,multipliars,  () =>
        {
            DeselectSkill(selectedSkill, enemy);
            ResetCoverAccruacyDebuff();
            //ResetCoverdamageDebuff();

            OnComplete?.Invoke();
            
            if (Character is Player)
            {
                Character.AttackEnd(selectedSkill);
            }
            else if (Character is Enemy)
            {
                enemy.AttackEnd(selectedSkill);
            }
                
            coverDamageMultiplier = 1;
            otherDamageMultiplier = 1;
            //rangedCloseDamageMultiplier = 1;
            selectedSkill.SkillDamageMultipliar = 1;
            //characterTooClose = false;
        });
        
        
        
        yield return null;
    }

    public void CharacterTooCloseForRanged(bool value)
    {
         //otherDamageMultiplier = value ? 0.5f : 1;

         //if (value)
         /*{
             foreach (var skill in skillsList)
             {
                 if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
                 {
                     skill.SkillDamageMultipliar = value ? 0.5f : 1;

                     foreach (var button in skillButtons)
                     {
                         if (button.skillData.name == skill.skillData.name)
                         {
                             button.SkillDebuffImage
                         }
                     }
                 }
             }
         }*/

         if (characterTooClose == value)
         {
             return;
         }
         
         foreach (var button in skillButtons)
         {
             if (button.skillData.skillType == SkillsData.SkillType.Ranged && button.skillData.skillDamage > 0)
             {
                 button.rangedDebuffImage.SetActive(value);

                 if (value)
                 {
                     button.AddToContent("Too close for ranged attack, damage halved");
                 }
                 else
                 {
                     button.RemoveLastLine();
                 }
             }
         }

         characterTooClose = value;
         
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
            coverDamageMultiplier = 0.5f;
            return selectSkill.damage / 2;
        }
        coverDamageMultiplier = 1;
        return 0;
    }
    

    public void ApplyCoverAccuracyDebuff()
    {
        accuracyBeforeCoverDebuff = selectedSkill.accuracy;
        selectedSkill.accuracy -= selectedSkill.coverAccuracyDebuff;
    }

    public int ApplyCoverDamageDebuff()
    {
        damageBeforeCoverDebuff = selectedSkill.damage;
        selectedSkill.damage /= 2;
        
        return selectedSkill.damage / 2;
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
        public float SkillDamageMultipliar = 1;
    }

    
}