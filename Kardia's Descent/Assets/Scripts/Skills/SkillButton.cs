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

    private void Awake()
    {
        HighlightSkill();
    }

    public void InitButton(SkillsData skillDataIn, SkillContainer.Skills skillIn , UnityAction useSkill, SkillContainer skillContainerIn)
    {
        skillData = skillDataIn;
        skill = skillIn;
        skillContainer = skillContainerIn;
        
        SkillImage.sprite = skillData.skillSprite;
        //GetComponentInChildren<TextMeshProUGUI>().text = skillData.skillName;
        tooltipTrigger.SetHeader(skillDataIn.skillName);
        tooltipTrigger.SetContent(skillDataIn.skillDescription);
        tooltipTrigger.AddToContent("");
        
        if (skillDataIn.skillClass == SkillsData.SkillClass.Buff)
        {
            SkillSelectedOutline.GetComponent<UIOutline>().color = Interact.Instance.skillColors[1];
        }
        else
        {
            SkillSelectedOutline.GetComponent<UIOutline>().color = Interact.Instance.skillColors[0];
        }

        
        if (skillDataIn.passiveOrActive == SkillsData.PassiveOrActive.Active)
        {
            if (skillDataIn.skillClass != SkillsData.SkillClass.Buff)
            {
                tooltipTrigger.AddToContent($"Damage: {skillDataIn.skillDamage + skillContainerIn.Character.extraMeleeDamage}");
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
        //pointer enter
       GetComponent<EventTrigger>().triggers[0].callback.AddListener((data)=> Interact.Instance.EnableMovement(false));
       GetComponent<EventTrigger>().triggers[0].callback.AddListener((data)=> Interact.Instance.EnableAttackableTiles(false));
       
       //pointer exit
       GetComponent<EventTrigger>().triggers[1].callback.AddListener((data)=> Interact.Instance.EnableMovement(true));
       GetComponent<EventTrigger>().triggers[1].callback.AddListener((data)=> Interact.Instance.EnableAttackableTiles(true));

    }
    
}
