using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SGT_Tools.Tools;
using TMPro;
using UnityEngine;

public class SpawnText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject buffIcon;
    public GameObject debuffIcon;

    [SerializeField] private SGT_DestroySelf destroySelf;

    [SerializeField] private DOTweenAnimation[] anims;
    [SerializeField] private DOTweenAnimation[] delayAnims;
    
    
    /*public void SetDestroyTime(float Time)
    {
        destroySelf.DestroyTime = Time;
        destroySelf.enabled = true;
    }*/

    public void SetAnimDelay(float time)
    {
        foreach (var anim in delayAnims)
        {
            anim.delay = time;
        }

        foreach (var anim in anims)
        {
            anim.DORestart();
        }
    }
}
