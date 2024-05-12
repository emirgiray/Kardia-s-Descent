using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float delayTime = 0.5f;

    [SerializeField] private string header;
    [SerializeField] [Multiline] private string content;

    public UnityEvent OnPointerEnterEvent;
    public UnityEvent OnPointerExitEvent;

    private Coroutine showTooltipCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
        showTooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(true));
    }

    private IEnumerator ShowTooltipAfterDelay(bool isUI)
    {
        yield return new WaitForSeconds(delayTime);
        TooltipSystem.Show(content, header, transform.position, isUI);
    }

    private void OnMouseEnter()
    {
        OnPointerEnterEvent?.Invoke();
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
        showTooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
        TooltipSystem.Hide();
    }

    private void OnMouseExit()
    {
        OnPointerExitEvent?.Invoke();
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
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
