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
    
    [BoxGroup("Skils")] [SerializeField] private GameObject HorizontalLayoutGroup;
    [BoxGroup("Skils")] [SerializeField] private List<SkillButton> SkillButtons = new List<SkillButton>();
    
    [BoxGroup("Points")] [SerializeField] private TextMeshProUGUI actionText;
    [BoxGroup("Points")] [SerializeField] private List<GameObject> actionPointsPips = new List<GameObject>();
    [BoxGroup("Points")] [SerializeField] private Button skipTurnButton;
    
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

        //player.OnActionPointsChange += UpdateActionPoints;//actions doesnt work for some reason
        

        player.OnActionPointsChangeEvent.AddListener(UpdateActionPoints);
        player.PlayerTurnStart.AddListener(() => SetSkipTurnButtonInteractable(true));
        
        skipTurnButton.onClick.AddListener(player.EndTurn);
    }

    private void OnDisable()
    {

        //player.OnActionPointsChange -= UpdateActionPoints;
        //skipTurnButton.onClick.RemoveListener(player.EndTurn);
    }

    public void UpdateActionPoints(int value, string type)
    {
        actionText.text = value.ToString();

        for (int i = 0; i < actionPointsPips.Count; i++)
        {
            if (type == "+")
            {
                if (i < value)
                {
                    actionPointsPips[i].SetActive(true);
                }
                
            }
            else if (type == "-")
            {
                if (i >= value)
                {
                    actionPointsPips[i].SetActive(false);
                }
                
            }
        }
        
        /*if (type == "+")
        {
            actionPointsPips[value - 1].SetActive(true);
        }
        else if (type == "-")
        {
            actionPointsPips[value].SetActive(false);
        }*/
        //actionPointsPips.ForEach(x => x.SetActive(false));
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
