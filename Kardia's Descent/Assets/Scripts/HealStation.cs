using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
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
    [SerializeField] private Image healMultipliarEffect;
    
    [SerializeField] private TextMeshProUGUI healAmountText;
    [SerializeField] private TextMeshProUGUI healMultiplierText;
    [SerializeField] private TextMeshProUGUI healTotalText;
    [SerializeField] private TextMeshProUGUI HealStationInfoText;
    
    [SerializeField] private Button healButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private List<GameObject> HeartButtons = new();
    [SerializeField] private Sprite emptyHeartSprite;
    
    [SerializeField] private Player player;
    [SerializeField] private HeartData selectedHeartData;

    private Transform cameraStartTransform;
    [SerializeField] private GameObject newCam;
    [SerializeField] private GameObject healCam;

    [SerializeField] private GameObject potionPrefab;
    [SerializeField] private Transform potionSpawnTransform;
    [SerializeField] private List<GameObject> steamVFX = new();

    [SerializeField] private  List<GameObject> machinesToRumble = new();
    
    private Dictionary<int, Color> textColors = new Dictionary<int, Color>()
    {
        {0, new Color(1,1,1)},
        {1, new Color(0.1f,0.3f,1f) },
        {2, new Color(1f, 1,0) }, 
       
    };


    Vector3 leftPanelStartPos = new Vector3(-1500, 0, 0);
    Vector3 rightPanelStartPos = new Vector3(1500, 0, 0); 
    Vector3 rightPanelEndPos = new Vector3(960, 0, 0); 
    Vector3 leftPanelEndPos = new Vector3(-960, 0, 0); 
    
    private Vector3 healMultiplierTextStartScale;
    private Vector3 selectedHeartImageStartScale;
    private Vector3 healMultipliarEffectStartScale;

    public void StartSelection(Player playerIn)
    {
        healMultiplierTextStartScale = healMultiplierText.transform.localScale;
        selectedHeartImageStartScale = selectedHeartImage.transform.localScale;
        healMultipliarEffectStartScale = healMultipliarEffect.transform.localScale;
        
        everythingUseful.Interact.StopAllLogic();
        cameraStartTransform = everythingUseful.Interact.cameraTransform;
        everythingUseful.CineMachineCutRest(newCam, true);
        /*everythingUseful.Interact.MoveCameraAction?.Invoke(cameraTargetTransform, 1f);
        everythingUseful.Interact.ZoomCameraAction?.Invoke( -10f, 1);*/
        
        this.player = playerIn;
        healUI.SetActive(true);
        
        LeftPanel.transform.localPosition = leftPanelStartPos;
        RightPanel.transform.localPosition = rightPanelStartPos;
        LeftPanel.transform.DOLocalMove(leftPanelEndPos, 0.5f).From(leftPanelStartPos).SetEase(Ease.InOutQuad);
        RightPanel.transform.DOLocalMove(rightPanelEndPos, 0.5f).From(rightPanelStartPos).SetEase(Ease.InOutQuad);
        healAmountText.gameObject.SetActive(false);
        healMultiplierText.gameObject.SetActive(false);
        healTotalText.gameObject.SetActive(false);
        healMultipliarEffect.gameObject.SetActive(false);
        
        HealStationInfoText.gameObject.SetActive(true);
        
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
    float textPunchScale = 0.1f;
    
    Tween textPunchScaleTween;
    Tween heartPunchScaleTween;
    
    Tween multipliarEffectPunchScaleTween;
    Tween multipliarEffectColorTween;
    Tween multipliarEffectRotateTween;
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
                textPunchScale = 0.1f;
                break;
            case HeartData.HeartRarity.Rare:
                healMultipliar = RareMultiplier;
                textColor = textColors[1];
                textSize = 120f;
                textPunchScale = 0.3f;
                break;
            case HeartData.HeartRarity.Legendary:
                healMultipliar = Legendary;
                textColor = textColors[2];
                textSize = 150f;
                textPunchScale = 0.5f;
                break;
        }
        if (textPunchScaleTween != null) textPunchScaleTween.Kill();
        healMultiplierText.transform.localScale = healMultiplierTextStartScale;
        textPunchScaleTween = healMultiplierText.transform.DOPunchScale(Vector3.one * textPunchScale, 0.5f, 1, 1).SetLoops(-1, LoopType.Restart).SetRelative();
        
        if (multipliarEffectPunchScaleTween != null) multipliarEffectPunchScaleTween.Kill();
        if (multipliarEffectColorTween != null) multipliarEffectColorTween.Kill();
        if (multipliarEffectRotateTween != null) multipliarEffectRotateTween.Kill();
        healMultipliarEffect.transform.localScale = healMultipliarEffectStartScale;
        //multipliarEffectColorTween = healMultipliarEffect.DOColor(textColor, 1f).SetLoops(-1, LoopType.Yoyo);
        healMultipliarEffect.color = textColor;
        multipliarEffectPunchScaleTween = healMultipliarEffect.transform.DOPunchScale(Vector3.one * textPunchScale, 0.5f, 1, 1).SetLoops(-1, LoopType.Restart).SetRelative();
        multipliarEffectRotateTween = healMultipliarEffect.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart);
        
        if (heartPunchScaleTween != null) heartPunchScaleTween.Kill();
        selectedHeartImage.transform.localScale = selectedHeartImageStartScale;
        selectedHeartImage.sprite = heartData.HeartSprite;
        heartPunchScaleTween = selectedHeartImage.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 1).SetRelative();
        
        healAmountText.text = $"{healAmount} x";
        healMultiplierText.text = healMultipliar.ToString();
        healMultiplierText.color = textColor;
        healMultiplierText.fontSize = textSize;
        healTotalText.text = $"= {healAmount * healMultipliar}";
        
        healAmountText.gameObject.SetActive(true);
        healMultiplierText.gameObject.SetActive(true);
        healTotalText.gameObject.SetActive(true);
        healMultipliarEffect.gameObject.SetActive(true);
        HealStationInfoText.gameObject.SetActive(false);
        
        healButton.onClick.RemoveAllListeners();
        healButton.onClick.AddListener(() => HealPlayer());
    }

   
    
    
    private void HealPlayer()
    {
        everythingUseful.CineMachineCutRest(healCam, true);
        everythingUseful.CineMachineCutRest(newCam, false);
        
        StartCoroutine(HealPlayerDelay());

        foreach (var vfx in steamVFX)
        {
            vfx.SetActive(true);
        }

        foreach (var machineToRumble in machinesToRumble)
        {
            machineToRumble.transform.DOShakeScale(shakeScaleDuration, shakeScaleStrength, shakeScaleVibrato, shakehScaleRandomness).SetEase(shakeScaleEase);
            machineToRumble.transform.DOShakeRotation(shakeRotationDuration, shakeRotationStrength, shakeRotationVibrato, shakeRotationRandomness).SetEase(shakeRotationEase);
        }
        

    }

    [SerializeField] [FoldoutGroup("TEST")] private float shakeScaleDuration = 3;
    [SerializeField] [FoldoutGroup("TEST")] private float shakeScaleStrength = 3;
    [SerializeField] [FoldoutGroup("TEST")] private int shakeScaleVibrato = 10;
    [SerializeField] [FoldoutGroup("TEST")] private float shakehScaleRandomness = 90;
    [SerializeField] [FoldoutGroup("TEST")] private Ease shakeScaleEase = Ease.InOutQuad;

    [SerializeField] [FoldoutGroup("TEST")] private float shakeRotationDuration = 3;
    [SerializeField] [FoldoutGroup("TEST")] private float shakeRotationStrength = 3;
    [SerializeField] [FoldoutGroup("TEST")] private int shakeRotationVibrato = 10;
    [SerializeField] [FoldoutGroup("TEST")] private float shakeRotationRandomness = 90;
    [SerializeField] [FoldoutGroup("TEST")] private Ease shakeRotationEase = Ease.InOutQuad;
    
    
    [Button, GUIColor(1f, 1f, 1f), PropertyOrder(-1)] [FoldoutGroup("TEST")]
    public void TestAnims()
    {
        StartCoroutine(HealPlayerDelay());

        foreach (var vfx in steamVFX)
        {
            vfx.SetActive(true);
        }
        
        foreach (var machineToRumble in machinesToRumble)
        {
            machineToRumble.transform.DOShakeScale(shakeScaleDuration, shakeScaleStrength, shakeScaleVibrato, shakehScaleRandomness).SetEase(shakeScaleEase);
            machineToRumble.transform.DOShakeRotation(shakeRotationDuration, shakeRotationStrength, shakeRotationVibrato, shakeRotationRandomness).SetEase(shakeRotationEase);
        }
    }

    private IEnumerator HealPlayerDelay()
    {
        yield return new WaitForSeconds(1);
        
        var spawnedPot = Instantiate(potionPrefab, potionSpawnTransform.position, Quaternion.identity);
        spawnedPot.TryGetComponent(out ProjectileMove projectileMove);
        // activate the parabola and on its end activate the skill
        projectileMove.parabolaController.OnParabolaEnd.AddListener(() =>
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
        });
        //projectileMove.parabolaController.Animation = true;
        var newTarget = player.transform.position + Vector3.up * 2;
        projectileMove.SetAndStartParabolaYOffset(player.Head);
        
        
        yield return new WaitForSeconds(1);
        foreach (var vfx in steamVFX)
        {
            vfx.SetActive(false);
        }
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
        
        everythingUseful.CineMachineCutRest(healCam, false); 
        everythingUseful.CineMachineCutRest(newCam, false);
    }

    public void ResetToDefault()
    {
        healMultiplierText.transform.localScale = healMultiplierTextStartScale;
        selectedHeartImage.sprite = emptyHeartSprite;
        healButton.interactable = false;
        healAmountText.gameObject.SetActive(false);
        healMultiplierText.gameObject.SetActive(false);
        healTotalText.gameObject.SetActive(false);
        
        HealStationInfoText.gameObject.SetActive(true);
        foreach (var button in HeartButtons)
        {
            button.GetComponent<Image>().sprite = emptyHeartSprite;
            //button.GetComponent<Button>().interactable = false;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        
    }
   
}
