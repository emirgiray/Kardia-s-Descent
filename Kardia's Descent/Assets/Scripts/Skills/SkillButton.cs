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
    [SerializeField] private Button button;
    [SerializeField] public TextMeshProUGUI cooldownText;
    [SerializeField] public GameObject cooldownImage;
    [SerializeField] private SkillContainer skillContainer;

    private void Awake()
    {
        HighlightSkill();
    }

    public void InitButton(SkillsData skillDataIn , UnityAction useSkill, SkillContainer skillContainerIn)
    {
        skillData = skillDataIn;
        GetComponent<Image>().sprite = skillData.skillSprite;
        button.onClick.AddListener(useSkill);
        GetComponentInChildren<TextMeshProUGUI>().text = skillData.skillName;
        skillContainer = skillContainerIn;
        //OnInit();
    }

    public void EnableDisableButton(bool value)
    {
        button.interactable = value;
    }

    /*private void Update()
    {
        /*if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            print("selected");
        }#1#
        //HighlightSkill();
    }*/

    /*public void OnInit()//this fires when this is initiated from inventory 
    {
        skillContainer.skillNotReadyAction += () => EnableDisableButton(false);
        skillContainer.skillReadyAction += () => EnableDisableButton(true);
    }

    private void OnDisable()
    {
        skillContainer.skillNotReadyAction -= () => EnableDisableButton(false);
        skillContainer.skillReadyAction -= () => EnableDisableButton(true);
    }*/

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
