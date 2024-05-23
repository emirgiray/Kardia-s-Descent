using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    [BoxGroup("Enabled Click")] [SerializeField] 
    private float punchScale = .05f;
    [BoxGroup("Enabled Click")] [SerializeField] 
    private float duration = .5f;
    [BoxGroup("Enabled Click")] [SerializeField] 
    private int vibrato = 10;
    [BoxGroup("Enabled Click")] [SerializeField] 
    private float elasticity = 1;
    
    [BoxGroup("Disabled Click")] [SerializeField] 
    private float punchScaleD = .1f;
    [BoxGroup("Disabled Click")] [SerializeField] 
    private float durationD = .5f;
    [BoxGroup("Disabled Click")] [SerializeField] 
    private int vibratoD = 20;
    [BoxGroup("Disabled Click")] [SerializeField] 
    private float elasticityD = 1;
    
    [BoxGroup("Hover")] [SerializeField] 
    private Vector3 hoverScale = new Vector3(1.5f, 1.5f, 1.5f);
    [BoxGroup("Hover")] [SerializeField] 
    private float hoverDuration = .2f;
    
    Tween punchTween;
    Tween colorTween;
    Tween hoverTween;
    Vector3 originalScale;
    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = button.transform.localScale;
        //eventTrigger.OnPointerClick(OnPointerClickFunc);
        
       // button.onClick.AddListener(OnClick);
    }

    
    private void OnClick()
    {
        //Debug.Log($"button.interactable: {button.interactable}");
        punchTween?.Rewind();
        punchTween?.Kill();
        colorTween?.Rewind();
        colorTween?.Kill();
        hoverTween?.Rewind();
        hoverTween?.Kill();
        if (button.interactable && button.enabled)
        {
            punchTween = button.transform.DOPunchScale(new Vector3(punchScale, punchScale, punchScale), duration, vibrato, elasticity);
        }
        else
        {
            punchTween =  button.transform.DOPunchScale(new Vector3(punchScaleD, 0, 0), durationD, vibratoD, elasticityD);
            colorTween = button.image.DOColor(Color.red, durationD / 2).OnComplete(() => button.image.DOColor(Color.white, durationD / 2));
           
        }
    }

    
    private void OnHover()
    {
        hoverTween?.Pause();
        hoverTween?.Kill();
        //if (button.interactable && button.enabled)
        {
            hoverTween = button.transform.DOScale(hoverScale, hoverDuration / 2).SetUpdate(true);
        }
    }
    
    private void OnUnHover()
    {
        //Debug.Log($"unhover");
        punchTween?.Rewind();
        punchTween?.Kill();
        colorTween?.Rewind();
        colorTween?.Kill();
        hoverTween?.Pause();
        hoverTween?.Kill();
        hoverTween = button.transform.DOScale(originalScale, hoverDuration / 3).SetUpdate(true);
       // hoverTween?.Kill();
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnHover();
    }

    public void SetHoverScale(Vector3 originalScale, Vector3 newScale)
    {
        this.originalScale = originalScale;
        hoverScale = newScale;
        
    }
}
