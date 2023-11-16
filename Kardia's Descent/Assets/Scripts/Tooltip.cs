using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private int characterWrapLimit = 80;

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private Ease fadeInEase;
    [SerializeField] private Ease fadeOutEase;
    

    private void OnEnable()
    {
        gameObject.GetComponent<Image>().DOFade(1, fadeTime).SetEase(fadeInEase);
        headerText.DOFade(1, fadeTime).SetEase(fadeInEase);
        contentText.DOFade(1, fadeTime).SetEase(fadeInEase);
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Image>().DOFade(0, fadeTime).SetEase(fadeOutEase);
        headerText.DOFade(0, fadeTime).SetEase(fadeOutEase);
        contentText.DOFade(0, fadeTime).SetEase(fadeOutEase);
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerText.gameObject.SetActive(false);
        }
        else
        {
            headerText.gameObject.SetActive(true);
            headerText.text = header;
        }

        contentText.text = content;
        
        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;
        
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }
    
    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerText.text.Length;
            int contentLength = contentText.text.Length;
        
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }
        
        Vector2 position = Input.mousePosition;
        
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
