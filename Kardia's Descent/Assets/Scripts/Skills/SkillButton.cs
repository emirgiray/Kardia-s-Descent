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
    [SerializeField] private SkillsData skillData;
    [SerializeField] private Button button;

    private void Awake()
    {
        HighlightSkill();
    }

    public void InitButton(SkillsData skillDataIn , UnityAction useSkill)
    {
        skillData = skillDataIn;
        GetComponent<Image>().sprite = skillData.skillSprite;
        button.onClick.AddListener(useSkill);
        GetComponentInChildren<TextMeshProUGUI>().text = skillData.skillName;
    }

    public void DisableButton(bool value)
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
