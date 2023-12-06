using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [BoxGroup("Player")][SerializeField] private Player player;
    [BoxGroup("Player")] [SerializeField] private TextMeshProUGUI nameText;
    [BoxGroup("Player")] [SerializeField] private RawImage playerPortrait;
    
    [BoxGroup("Skils")] [SerializeField] private GameObject HorizontalLayoutGroup;
    [BoxGroup("Skils")] [SerializeField] private List<SkillButton> SkillButtons = new List<SkillButton>();
    [BoxGroup("Skils")] [SerializeField] private List<GameObject> SkillKeymaps = new List<GameObject>();
    
    [BoxGroup("Points")] [SerializeField] private TextMeshProUGUI actionText;
    [BoxGroup("Points")] [SerializeField] private Image APBar01;
    [BoxGroup("Points")] [SerializeField] private List<GameObject> actionPointsPips = new List<GameObject>();
    [BoxGroup("Points")] [SerializeField] private Button skipTurnButton;
    
    [BoxGroup("Health")] [SerializeField] private TextMeshProUGUI healthText;
    // [BoxGroup("Health")] [SerializeField] private TextMeshProUGUI healthMaxText;
    [BoxGroup("Health")] [SerializeField] private Image Bar01;
    [BoxGroup("Health")] [SerializeField] private Image Bar02;

    public void SetPlayer(Player playerIn)
    {
        player = playerIn;
        nameText.text = player.name;
    }
    
    public void SubscribeToPlayerEvents()
    {

        //player.OnActionPointsChange += UpdateActionPoints;//actions doesnt work for some reason
        // player.OnCharacterRecieveDamageAction += UpdateHealth;
        
        healthText.text = $"{player.health.Max} / {player.health.Max}";
        //healthMaxText.text = player.health.Max.ToString();
        player.OnActionPointsChangeEvent.AddListener(UpdateActionPoints);
        //player.OnActionPointsChangeEvent2.AddListener();
        player.OnHealthChangeEvent.AddListener(UpdateHealth);
        player.PlayerTurnStart.AddListener(() => SetSkipTurnButtonInteractable(true));
        
        skipTurnButton.onClick.AddListener(player.EndTurn);
    }

    private void OnDisable()
    {
        // player.OnCharacterRecieveDamageAction -= UpdateHealth;
        //player.OnActionPointsChange -= UpdateActionPoints;
        //skipTurnButton.onClick.RemoveListener(player.EndTurn);
    }

    /// <summary>
    /// Only send the LAST value of AP, not the change amount
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    public void UpdateActionPoints(int value, string type)
    {
        // actionText.text = value.ToString();
        actionText.text = $"{value} / {player.maxActionPoints}";
        
        APBar01.DOFillAmount((float)value / player.maxActionPoints, 2);
        
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

    public void UpdateHealth()
    {
        // healthText.text = player.health._Health.ToString();
        healthText.text = $"{player.health._Health} / {player.health.Max}";
    }

    public void UpdateAP()
    {
        
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

        /*foreach (var var in SkillButtonsIn)
        {
            Debug.Log($"skill data: {var.skillData.name}, skill {var.skill}");
        }
        */
        
        for (int i = 0; i < SkillButtons.Count; i++)
        {
            if (i < 3)
            {
                SkillKeymaps[i].SetActive(true);
            }
            
        }
    }

    public void SetSkipTurnButtonInteractable(bool value)
    {
        skipTurnButton.interactable = value;

        for (int i = 0; i < SkillButtons.Count; i++)
        {
            if (value)
            {
                if (SkillButtons[i].skill.skillReadyToUse || SkillButtons[i].skill.skillData.passiveOrActive == SkillsData.PassiveOrActive.Passive)
                {
                    SkillButtons[i].EnableDisableButton(value);
                }
            }
            else
            {
                if (SkillButtons[i].skill.skillData.passiveOrActive == SkillsData.PassiveOrActive.Active)
                {
                    SkillButtons[i].EnableDisableButton(value);
                }
            }
            
        }
    }

    public void SetPlayerPortrait(RenderTexture renderTexture)
    {
        playerPortrait.texture = renderTexture;
    }

    public Button GetSkipButton()
    {
        return skipTurnButton;
    }
    
}
