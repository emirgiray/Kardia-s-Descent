using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;


namespace SGT_Tools.AI
{
    /// <summary>
    /// If you add SGT_Health script bar2 for animation, This script will add automaticly to SGT_Health gameObject.
    /// </summary>
    public class SGT_HealthAnim : MonoBehaviour
    {
        public Image Bar01;
        public Image Bar02;

        public float BarSpeed;
        public float BarDelay;

        private bool DelayLerp = false;



        private void Update()
        {
            if (!DelayLerp)
            {
                if (Bar02 != null)
                    Bar02.fillAmount = Mathf.Lerp(Bar02.fillAmount, Bar01.fillAmount, Time.deltaTime * BarSpeed);
            }

        }



        public void DelayBar()
        {
            StartCoroutine(DelayWait());
        }



        IEnumerator DelayWait()
        {
            DelayLerp = true;
            yield return new WaitForSecondsRealtime(BarDelay);
            DelayLerp = false;

        }



    }
}


