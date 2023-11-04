using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] public List<SkillsData> skillsDataList = new List<SkillsData>();
    [SerializeField] public bool skillSelected = false;
    [SerializeField] public SkillsData selectedSkill;
    [SerializeField] public Character Character;

    private void Start()
    {
        Character = GetComponent<Character>();
    }

    public void SkillHighlighted(SkillsData highlightSkill)
    {
        Interact.Instance.SkillHighlighted?.Invoke();
    }

    public void TrySelectSkill(SkillsData selectSkill)
    {
        if (skillSelected == false) //if no skill is selected select that sakill
        {
            skillSelected = true;
            if (Character.canAttack)
            {
                SelectSkill(selectSkill);
            }
        }
        else
        {
            if (selectSkill == selectedSkill) //if the skill is already selected deselect it
            {
                DeslectSkill(selectSkill);
            }
            else //if a skill is already selected and a new skill is selected, deselect the old skill and select the new one
            {
                if (Character.canAttack)
                {
                    SelectSkill(selectSkill);
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
        Debug.Log($"{this.name} Selected {selectSkill.name}");

        selectedSkill = selectSkill;
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
        Debug.Log($"{this.name} Deselected {deselectSkill.name}");

        skillSelected = false;
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

        Debug.Log($"{this.name} Used {selectedSkill.name}");

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
        skillsDataList.AddRange(skillsDataListIn);
    }
}