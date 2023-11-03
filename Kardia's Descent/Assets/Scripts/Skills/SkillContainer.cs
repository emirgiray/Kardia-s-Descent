using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] private List<SkillsData> skillsDataList = new List<SkillsData>();
    [SerializeField] public bool skillSelected = false;
    [SerializeField] public SkillsData selectedSkill;

    public void SkillHighlighted(SkillsData highlightSkill)
    {
        Interact.Instance.SkillHighlighted?.Invoke();
    } 
    
    public void TrySelectSkill(SkillsData selectSkill)
    {
        if (skillSelected == false) //if no skill is selected select that sakill
        {
            skillSelected = true;
            SelectSkill(selectSkill);
        }
        else
        {
            if (selectSkill == selectedSkill) //if the skill is already selected deselect it
            {
                DeslectSkill(selectSkill);
            }
            else //if a skill is already selected and a new skill is selected, deselect the old skill and select the new one
            {
                SelectSkill(selectSkill);
            }
        }
        
    }

    public void SelectSkill(SkillsData selectSkill)
    {
        Debug.Log($"{selectSkill.name} Selected");
        
        selectedSkill = selectSkill;
        Interact.Instance.SkillSelected?.Invoke();
        Interact.Instance.selectedCharacter.GetComponent<Character>().AttackStart();
        Interact.Instance.HighlightAttackableTiles();
    }
    public void DeslectSkill(SkillsData deselectSkill)
    {
        Debug.Log($"{deselectSkill.name} Deselected");
        
        skillSelected = false;
        selectedSkill = null;
        Interact.Instance.selectedCharacter.GetComponent<Character>().AttackCancel();
        Interact.Instance.ClearHighlightAttackableTiles();
        Interact.Instance.SkillDeselected?.Invoke();
    }
    public void UseSkill(SkillsData selectedSkill, Tile selectedTile)
    {
        Debug.Log($"{selectedSkill.name} Used");
        selectedSkill.ActivateSkill(gameObject, selectedTile, ()=> DeslectSkill(selectedSkill));
        Interact.Instance.selectedCharacter.GetComponent<Character>().AttackEnd();
    }
    
    
    public void AddSkills(List<SkillsData> skillsDataListIn)
    {
        skillsDataList.AddRange(skillsDataListIn);
    }
}
