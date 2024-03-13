using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    [SerializeField] private Tooltip tooltip;
    
    
    private void Awake()
    {
        if (current != null)
        {
            Destroy(this);
        }
        else
        {
            current = this;
        }
    }
    
    public static void Show(string content, string header, Vector3 pos)
    {
        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
        current.tooltip.SetPosition(pos);
    }
    
    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
