using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SkillButton : MonoBehaviour
{
    [SerializeField] public SkillsData skillData;
    [SerializeField] public SkillContainer.Skills skill;
    [SerializeField] private Image SkillImage;
    [SerializeField] private GameObject SkillSelectedOutline;
    [SerializeField] private Button button;
    [SerializeField] public TextMeshProUGUI cooldownText;
    [SerializeField] public TextMeshProUGUI apCostText;
    [SerializeField] public GameObject cooldownImage;
    [SerializeField] private SkillContainer skillContainer;
    [SerializeField] private TooltipTrigger tooltipTrigger;
    

    public void InitButton(SkillsData skillDataIn, SkillContainer.Skills skillIn , UnityAction useSkill, SkillContainer skillContainerIn)
    {
        skillData = skillDataIn;
        skill = skillIn;
        skillContainer = skillContainerIn;
        HighlightSkill();
        SkillImage.sprite = skillData.skillSprite;
        //GetComponentInChildren<TextMeshProUGUI>().text = skillData.skillName;
        tooltipTrigger.SetHeader(skillDataIn.skillName);
        tooltipTrigger.SetContent(skillDataIn.skillDescription);
        tooltipTrigger.AddToContent("");
        
        if (skillDataIn.skillClass == SkillsData.SkillClass.Buff)
        {
            SkillSelectedOutline.GetComponent<UIOutline>().color = skillData.everythingUseful.Interact.skillColors[1];
        }
        else
        {
            SkillSelectedOutline.GetComponent<UIOutline>().color = skillData.everythingUseful.Interact.skillColors[0];
        }

        
        if (skillDataIn.passiveOrActive == SkillsData.PassiveOrActive.Active)
        {
            if (skillDataIn.skillClass != SkillsData.SkillClass.Buff)
            {
                if (skillDataIn.skillType == SkillsData.SkillType.Melee)
                {
                    tooltipTrigger.AddToContent($"Damage: {skillDataIn.skillDamage + skillContainerIn.Character.extraMeleeDamage}");
                }
                else if (skillDataIn.skillType == SkillsData.SkillType.Ranged)
                {
                    tooltipTrigger.AddToContent($"Damage: {skillDataIn.skillDamage}");
                }
                tooltipTrigger.AddToContent($"Accuracy: {skillDataIn.accuracy + skillContainerIn.Character.extraRangedAccuracy}");
            }
            
            tooltipTrigger.AddToContent($"Range: {skillDataIn.skillRange}");
            tooltipTrigger.AddToContent($"AP Use: {skillDataIn.actionPointUse}");
            tooltipTrigger.AddToContent($"CD: {skillDataIn.skillCooldown}");
        }
        
        if (skillDataIn.passiveOrActive == SkillsData.PassiveOrActive.Active)
        {
            button.onClick.AddListener(useSkill);
            apCostText.text = skillDataIn.actionPointUse.ToString();
            //OnInit();
        }
        else
        {
            button.enabled = false;
        }
    }

    public void InitForMenuButton(SkillsData skillDataIn)
    {
        SkillImage.sprite = skillDataIn.skillSprite;
        
        tooltipTrigger.SetHeader(skillDataIn.skillName);
        tooltipTrigger.SetContent(skillDataIn.skillDescription);
        tooltipTrigger.AddToContent("");
        if (skillDataIn.passiveOrActive == SkillsData.PassiveOrActive.Active)
        {
            if (skillDataIn.skillClass != SkillsData.SkillClass.Buff)
            {
                if (skillDataIn.skillType == SkillsData.SkillType.Melee)
                {
                    tooltipTrigger.AddToContent($"Damage: {skillDataIn.skillDamage}");
                }
                else if (skillDataIn.skillType == SkillsData.SkillType.Ranged)
                {
                    tooltipTrigger.AddToContent($"Damage: {skillDataIn.skillDamage}");
                }
                tooltipTrigger.AddToContent($"Accuracy: {skillDataIn.accuracy}");
            }
            
            tooltipTrigger.AddToContent($"Range: {skillDataIn.skillRange}");
            tooltipTrigger.AddToContent($"AP Use: {skillDataIn.actionPointUse}");
            tooltipTrigger.AddToContent($"CD: {skillDataIn.skillCooldown}");
        }
        
        if (skillDataIn.passiveOrActive == SkillsData.PassiveOrActive.Active)
        {
            apCostText.text = skillDataIn.actionPointUse.ToString();
        }
    }

    public void SetHeader(string text)
    {
        tooltipTrigger.SetHeader(text);
    }

    public void SetContent(string text)
    {
        tooltipTrigger.SetContent(text);
    }

    public void AddToContent(string text)
    {
        tooltipTrigger.AddToContent(text);
    }
    
    public void SwitchSelectedOutline(bool value)
    {
        SkillSelectedOutline.SetActive(value);
    }

    public void EnableDisableButton(bool value)
    {
        button.interactable = value;
    }

    public void SetSkillContainer(SkillContainer skillContainerIn)
    {
        skillContainer = skillContainerIn;
    }
    
    public void HighlightSkill()
    {
        if (!skillData.everythingUseful.SceneChanger.isOnMainMenu)
        {
            //pointer enter
            GetComponent<EventTrigger>().triggers[0].callback.AddListener((data)=> skillData.everythingUseful.Interact.EnableMovement(false));
            GetComponent<EventTrigger>().triggers[0].callback.AddListener((data)=> skillData.everythingUseful.Interact.EnableAttackableTiles(false));
       
            //pointer exit
            GetComponent<EventTrigger>().triggers[1].callback.AddListener((data)=> skillData.everythingUseful.Interact.EnableMovement(true));
            GetComponent<EventTrigger>().triggers[1].callback.AddListener((data)=> skillData.everythingUseful.Interact.EnableAttackableTiles(true));
        }

    }
    
}
