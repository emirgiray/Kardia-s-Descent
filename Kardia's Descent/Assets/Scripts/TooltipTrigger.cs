using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;
    [SerializeField] private string header;
    [SerializeField] [Multiline] private string content;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () => { TooltipSystem.Show(content, header); });
        // TooltipSystem.Show(content, header);
    }

    private void OnMouseEnter()
    {
        delay = LeanTween.delayedCall(0.5f, () => { TooltipSystem.Show(content, header); });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    private void OnMouseExit()
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    public void SetContent(string contentIn)
    {
        content = contentIn;
    }
    
    public void SetHeader(string headerIn)
    {
        header = headerIn;
    }

}
