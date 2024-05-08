using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//todo add anims
//todo download hot reload :)
public class HealStation : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    [SerializeField] private float healAmount = 100;
    private float healMultipliar = 1;

    [SerializeField] private float CommonMultiplier = 1; 
    [SerializeField] private float RareMultiplier = 1.5f; 
    [SerializeField] private float Legendary = 2f;
    
    [SerializeField] private VFXSpawner healVFX;
    
    [SerializeField] private GameObject healUI;
    [SerializeField] private GameObject LeftPanel;
    [SerializeField] private GameObject RightPanel;
    [SerializeField] private GameObject playerHeartsLayoutGroup;
    [SerializeField] private Image selectedHeartImage;
    
    [SerializeField] private TextMeshProUGUI healAmountText;
    [SerializeField] private TextMeshProUGUI healMultiplierText;
    [SerializeField] private TextMeshProUGUI healTotalText;
    
    [SerializeField] private Button healButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private List<GameObject> HeartButtons = new();
    [SerializeField] private Sprite emptyHeartSprite;
    
    [SerializeField] private Player player;
    [SerializeField] private HeartData selectedHeartData;

    private Transform cameraStartTransform;
    [SerializeField] private Transform cameraTargetTransform;
    
    private Dictionary<int, Color> textColors = new Dictionary<int, Color>()
    {
        {0, new Color(1,1,1)},
        {1, new Color(0.1f,0.3f,1f) },
        {2, new Color(1f, 1,0) }, 
       
    };

    private void OnEnable()
    {
        
    }
    
    Vector3 leftPanelStartPos = new Vector3(-1500, 0, 0);
    Vector3 rightPanelStartPos = new Vector3(1500, 0, 0); 
    Vector3 rightPanelEndPos = new Vector3(960, 0, 0); 
    Vector3 leftPanelEndPos = new Vector3(-960, 0, 0); 

    public void StartSelection(Player playerIn)
    {
        everythingUseful.Interact.StopAllLogic();
        cameraStartTransform = everythingUseful.Interact.cameraTransform;
        everythingUseful.Interact.MoveCameraAction?.Invoke(cameraTargetTransform, 1f);
        everythingUseful.Interact.ZoomCameraAction?.Invoke( -10f, 1);
        
        this.player = playerIn;
        healUI.SetActive(true);
        
        LeftPanel.transform.localPosition = leftPanelStartPos;
        RightPanel.transform.localPosition = rightPanelStartPos;
        LeftPanel.transform.DOLocalMove(leftPanelEndPos, 0.5f).From(leftPanelStartPos).SetEase(Ease.InOutQuad);
        RightPanel.transform.DOLocalMove(rightPanelEndPos, 0.5f).From(rightPanelStartPos).SetEase(Ease.InOutQuad);
        
        healButton.interactable = false;
        healAmountText.gameObject.SetActive(false);
        healMultiplierText.gameObject.SetActive(false);
        healTotalText.gameObject.SetActive(false);
        
        for (int i = 0; i < player.inventory.heartsInInventory.Count; i++)
        {
            HeartButtons[i].GetComponent<Image>().sprite = player.inventory.heartsInInventory[i].HeartSprite;
            //HeartButtons[i].GetComponent<Button>().interactable = true;
            var i1 = i;
            HeartButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectHeart(player.inventory.heartsInInventory[i1]));
        }
        
    }
    
    
    Color textColor = Color.white;
    float textSize = 1;
    private void SelectHeart(HeartData heartData)
    {
        selectedHeartData = heartData;
        healButton.interactable = true;
        
        
        switch (heartData.heartRarity)
        {
            case HeartData.HeartRarity.Common:
                healMultipliar = CommonMultiplier;
                textColor = textColors[0];
                textSize = 100;
                break;
            case HeartData.HeartRarity.Rare:
                healMultipliar = RareMultiplier;
                textColor = textColors[1];
                textSize = 120f;
                break;
            case HeartData.HeartRarity.Legendary:
                healMultipliar = Legendary;
                textColor = textColors[2];
                textSize = 150f;
                break;
        }
        selectedHeartImage.sprite = heartData.HeartSprite;
        selectedHeartImage.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 1);
        
        healAmountText.text = $"{healAmount} x";
        healMultiplierText.text = healMultipliar.ToString();
        healMultiplierText.color = textColor;
        healMultiplierText.fontSize = textSize;
        healTotalText.text = $"= {healAmount * healMultipliar}";
        
        healAmountText.gameObject.SetActive(true);
        healMultiplierText.gameObject.SetActive(true);
        healTotalText.gameObject.SetActive(true);
        
        healButton.onClick.RemoveAllListeners();
        healButton.onClick.AddListener(() => HealPlayer());
    }

    private void HealPlayer()
    {
        player.inventory.heartsInInventory.Remove(selectedHeartData);
        if (player.heartContainer.heartData == selectedHeartData)
        {
            player.heartContainer.UnequipHeart(selectedHeartData);
        }
        
        int healthToGive = (int)(healAmount * healMultipliar);
        player.health.HealthDecrease(-healthToGive);
        player.OnHealthChangeEvent?.Invoke();
        healVFX.SpawnVFX(player.transform);
        EndSelection();
    }

    public void EndSelection()
    {
        everythingUseful.Interact.ContinueAllLogic();
        everythingUseful.Interact.MoveCameraAction?.Invoke(cameraStartTransform, 1f);
        
        LeftPanel.transform.DOLocalMove(leftPanelStartPos, 0.35f).From(leftPanelEndPos).SetEase(Ease.InOutQuad);
        RightPanel.transform.DOLocalMove(rightPanelStartPos, 0.35f).From(rightPanelEndPos).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            ResetToDefault();
            
            healUI.SetActive(false);
        });
    }

    public void ResetToDefault()
    {
        selectedHeartImage.sprite = emptyHeartSprite;
        healButton.interactable = false;
        healAmountText.gameObject.SetActive(false);
        healMultiplierText.gameObject.SetActive(false);
        healTotalText.gameObject.SetActive(false);
        foreach (var button in HeartButtons)
        {
            button.GetComponent<Image>().sprite = emptyHeartSprite;
            //button.GetComponent<Button>().interactable = false;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        
    }
   
}
