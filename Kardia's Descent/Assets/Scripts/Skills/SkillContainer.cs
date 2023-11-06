using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] public List<SkillsData> skillsDataSOList = new List<SkillsData>();
    [SerializeField] public List<Skills> skillsList = new List<Skills>();
    //[SerializeField] public SkillsData selectedSOSkill;
    [SerializeField] public Skills selectedSkill;
    [SerializeField] public bool skillSelected = false;
    [SerializeField] public bool skillCanbeUsed = true;
    [SerializeField] public Character Character;
    [SerializeField] public List<SkillButton> skillButtons = new List<SkillButton>();

    public Action skillNotReadyAction;
    //public Action skillReadyAction;

    private void OnEnable()
    {
        TurnSystem.Instance.RoundChanged += ResetSkillCooldowns;
    }

    private void OnDisable()
    {
        TurnSystem.Instance.RoundChanged -= ResetSkillCooldowns;
    }

    public void ResetSkillCooldowns()
    {
        for (int i = 0; i < skillsList.Count; i++)
        {
            if (skillsList[i].remainingSkillCooldown < skillsList[i].skillCooldown)
            {
                if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Friendly && Character is Player)
                {
                    skillsList[i].remainingSkillCooldown++;
                    skillsList[i].skillButton.cooldownText.text = (skillsList[i].remainingSkillCooldown).ToString(); 
                }
                if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy && Character is Enemy)
                {
                    skillsList[i].remainingSkillCooldown++;
                }
                
            }

            if (skillsList[i].remainingSkillCooldown == skillsList[i].skillCooldown)
            {
                skillsList[i].skillReadyToUse = true;
                if(Character is Player) skillsList[i].skillButton.EnableDisableButton(true);
                if(Character is Player) skillsList[i].skillButton.cooldownImage.SetActive(false);
                if(Character is Player) skillsList[i].skillButton.cooldownText.text = "";
            }
            // skillsList[i].remainingSkillCooldown = skillsList[i].skillCooldown;
            
        }
        //skillReadyAction?.Invoke();
    }

    private void Start()
    {
        Character = GetComponent<Character>();
        PopulateSkillsList();
    }

    public void PopulateSkillsList()
    {
        skillsList.AddRange(skillsDataSOList.ConvertAll(skill => new Skills
        {
            skillData = skill,
            /*skillDamage = skill.skillDamage,
            skillRange = skill.skillRange,*/
            skillCooldown = skill.skillCooldown,
            remainingSkillCooldown = skill.skillCooldown,
            skillButton = this.skillButtons.Find(x => x.skillData == skill)
        }));
    }

    public void SetSelectedSkillCooldonw(Skills skill)
    {
        //check if "skill" is already in the "skillsList" and return its "remainingSkillCooldown"
        selectedSkill.remainingSkillCooldown = skillsList.Find(x => x.skillData == skill.skillData).remainingSkillCooldown;
    }

    public void FindSkill(SkillsData skillsData)
    {
        
    }

    
    public void SkillHighlighted(SkillsData highlightSkill)
    {
        Interact.Instance.SkillHighlighted?.Invoke();
    }

    public void TrySelectSkill(SkillsData selectSkill)
    {
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
            if (selectSkill == selectedSkill.skillData) //if the skill is already selected deselect it
            {
                DeslectSkill(selectSkill);
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

    public void SelectSkill(SkillsData selectSkill, Enemy enemy = null)
    {
        if (Character is Enemy && !enemy.canAttack)
        {
            enemy.EndTurn();
            return;
        }
        if (Character is Enemy)Debug.Log($"{this.name} Selected {selectSkill.name}");
        
        selectedSkill = skillsList.Find(x => x.skillData == selectSkill);
        selectedSkill.skillData = selectSkill;
        selectedSkill.remainingSkillCooldown = skillsList.Find(x => x.skillData == selectedSkill.skillData).remainingSkillCooldown;
        selectedSkill.skillCooldown = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillCooldown;
        selectedSkill.skillReadyToUse = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillReadyToUse;
        selectedSkill.skillButton = skillsList.Find(x => x.skillData == selectedSkill.skillData).skillButton;
        
        
        if (Character is Player)
        {
            Interact.Instance.SkillSelected?.Invoke();
            Interact.Instance.HighlightAttackableTiles();
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackStart();
        }
        else if (Character is Enemy)
        {
            // enemy.AttackStart();
        }
    }

    public void DeslectSkill(SkillsData deselectSkill, Enemy enemy = null)
    {
        if (Character is Enemy)Debug.Log($"{this.name} Deselected {deselectSkill.name}");

        skillSelected = false;
        skillsList.Find(x => x.skillData == selectedSkill.skillData).remainingSkillCooldown = selectedSkill.remainingSkillCooldown;
        skillsList.Find(x => x.skillData == selectedSkill.skillData).skillReadyToUse = selectedSkill.skillReadyToUse;
        //skillsList.Find(x => x.skillData == selectedSkill.skillData).skillButton = selectedSkill.skillButton;
        selectedSkill = null;
        

        if (Character is Player)
        {
            Interact.Instance.ClearHighlightAttackableTiles();
            Interact.Instance.SkillDeselected?.Invoke();
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackCancel();
        }
        else if (Character is Enemy)
        {
            enemy.AttackCancel();
        }
    }
    

    public void UseSkill(SkillsData selectedSkill, Tile selectedTile, Enemy enemy = null)
    {
        if (Character is Enemy && !enemy.canAttack)
        {
            enemy.EndTurn();
            return;
        }

        if (Character is Enemy)Debug.Log($"{this.name} Used {selectedSkill.name}");

        this.selectedSkill.remainingSkillCooldown = 0;
            // this.selectedSkill.skillButton.GetComponent<SkillButton>().SetSkillButtonCooldownText(this.selectedSkill.remainingSkillCooldown);
        //if (this.selectedSkill.remainingSkillCooldown <= 0)
        {
            this.selectedSkill.skillReadyToUse = false;
            skillNotReadyAction?.Invoke();
            this.selectedSkill.skillButton.EnableDisableButton(false);
            this.selectedSkill.skillButton.cooldownImage.gameObject.SetActive(true);
            this.selectedSkill.skillButton.cooldownText.text = (this.selectedSkill.skillCooldown).ToString(); 
        }
        selectedSkill.ActivateSkill(gameObject, selectedTile, () => DeslectSkill(selectedSkill, enemy));
        
 

        if (Character is Player)
        {
            Interact.Instance.selectedCharacter.GetComponent<Character>().AttackEnd();
        }
        else if (Character is Enemy)
        {
            enemy.AttackEnd();
        }
    }


    public void AddSkills(List<SkillsData> skillsDataListIn)
    {
        skillsDataSOList.AddRange(skillsDataListIn);
    }
    
    [System.Serializable]
    public class Skills
    {
        public SkillsData skillData;
        /*public int skillDamage;
        public int skillRange;*/
        public int skillCooldown;
        public int remainingSkillCooldown;
        public bool skillReadyToUse = true;
        public SkillButton skillButton;

    }

    
}