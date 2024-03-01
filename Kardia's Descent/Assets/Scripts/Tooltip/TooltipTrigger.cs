using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;
    [SerializeField] private float delayTime = 0.5f;
    
    [SerializeField] private string header;
    [SerializeField] [Multiline] private string content;
    
    public UnityEvent OnPointerEnterEvent;
    public UnityEvent OnPointerExitEvent;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
        delay = LeanTween.delayedCall(delayTime, () => { TooltipSystem.Show(content, header, transform.position); });
        // Debug.Log($"çalıştı pointer"); //sometimes tooltip gives out errors but seems to fix it
        // TooltipSystem.Show(content, header);
    }

    private void OnMouseEnter()
    {
        OnPointerEnterEvent?.Invoke();
        delay = LeanTween.delayedCall(delayTime, () => { TooltipSystem.Show(content, header, transform.position); });
        // Debug.Log($"çalıştı mouse"); //sometimes tooltip gives out errors but seems to fix it
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    private void OnMouseExit()
    {
        OnPointerExitEvent?.Invoke();
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    public void SetContent(string contentIn)
    {
        content = contentIn;
    }

    public void AddToContent(string contentIn)
    {
        content += "\n";
        content += contentIn;
    }

    
    public void SetHeader(string headerIn)
    {
        header = headerIn;
    }

}
