using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Player player;
    
    [BoxGroup("Skils")] [SerializeField] private GameObject SkillsUI;
    [BoxGroup("Skils")] [SerializeField] private GameObject HorizontalLayoutGroup;
    [BoxGroup("Skils")] [SerializeField] private List<SkillButton> SkillButtons = new List<SkillButton>();
    
    [BoxGroup("Points")] [SerializeField] private GameObject PointsUI;
    [BoxGroup("Points")] [SerializeField] private TextMeshProUGUI actionText;
    [BoxGroup("Points")] [SerializeField] private Button skipTurnButton;
    
    [BoxGroup("Health")] [SerializeField] private GameObject HealthUI;
    [BoxGroup("Health")] [SerializeField] private Image Bar01;
    [BoxGroup("Health")] [SerializeField] private Image Bar02;
    [BoxGroup("Health")] [SerializeField] private TextMeshProUGUI nameText;

    public void SetPlayer(Player playerIn)
    {
        player = playerIn;
        nameText.text = player.name;
    }
    
    public void SubscribeToPlayerEvents()
    {

        player.OnActionPointsChange += UpdateActionPoints;//actions doesnt work for some reason
        

        player.OnActionPointsChangeEvent.AddListener(UpdateActionPoints);
        player.PlayerTurnStart.AddListener(() => SetSkipTurnButtonInteractable(true));
        
        skipTurnButton.onClick.AddListener(player.EndTurn);
    }

    private void OnDisable()
    {

        player.OnActionPointsChange -= UpdateActionPoints;
        //skipTurnButton.onClick.RemoveListener(player.EndTurn);
    }

    private void UpdateActionPoints(int value)
    {
        actionText.text = value.ToString();
    }


    
    public GameObject GetHorizontalLayoutGroup()
    {
        return HorizontalLayoutGroup;
    }


    public TextMeshProUGUI GetActionText()
    {
        return actionText;
    }
    
    public Image GetBar01()
    {
        return Bar01;
    }
    public Image GetBar02()
    {
        return Bar02;
    }

    public TextMeshProUGUI GetName()
    {
        return nameText;
    }

    public void SetSkillButtons(List<SkillButton> SkillButtonsIn)
    {
        SkillButtons = SkillButtonsIn;
    }

    public void SetSkipTurnButtonInteractable(bool value)
    {
        skipTurnButton.interactable = value;

        for (int i = 0; i < SkillButtons.Count; i++)
        {
            if (value)
            {
                if (SkillButtons[i].skill.skillReadyToUse)
                {
                    SkillButtons[i].EnableDisableButton(value);
                }
            }
            else
            {
                SkillButtons[i].EnableDisableButton(value);
            }
            
        }
    }
    
}
