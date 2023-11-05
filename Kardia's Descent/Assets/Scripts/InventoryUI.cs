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
    
    [BoxGroup("Points")] [SerializeField] private GameObject PointsUI;
    [BoxGroup("Points")] [SerializeField] private TextMeshProUGUI moveText;
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
        player.OnMovePointsChange += UpdateMovePoints;//actions dont work for some reason
        player.OnActionPointsChange += UpdateActionPoints;
        
        player.OnMovePointsChangeEvent.AddListener(UpdateMovePoints);
        player.OnActionPointsChangeEvent.AddListener(UpdateActionPoints);
        
        skipTurnButton.onClick.AddListener(player.EndTurn);
    }

    private void OnDisable()
    {
        player.OnMovePointsChange -= UpdateMovePoints;
        player.OnActionPointsChange -= UpdateActionPoints;
        //skipTurnButton.onClick.RemoveListener(player.EndTurn);
    }

    private void UpdateActionPoints(int value)
    {
        actionText.text = value.ToString();
    }

    private void UpdateMovePoints(int value)
    {
        moveText.text = value.ToString();
    }
    
    public GameObject GetHorizontalLayoutGroup()
    {
        return HorizontalLayoutGroup;
    }

    public TextMeshProUGUI GetMoveText()
    {
        return moveText;
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


    
}
