using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace SGT_Tools.Audio
{
    public class SGT_AudioSourceControl : MonoBehaviour
    {


        [HideInInspector]public SGT_AudioSFXManager SoundManager;


        private void OnDisable()
        {
            
            SetSource();
            if (DisableSouce!=null)
            {
                StopCoroutine(DisableSouce);
            }
            
        }



        Coroutine DisableSouce;
     
        public void DisableTimerStart(float Timer)
        {

            if (gameObject.activeInHierarchy)
            {
                DisableSouce = StartCoroutine(TimerDisable(Timer));
            }
                
            
        }

        IEnumerator TimerDisable(float Timer)
        {

            yield return new WaitForSecondsRealtime(Timer);

            SetSource();

        }

   



        public void SetSource()
        {

            if (SoundManager!=null)
            {
                SoundManager.GetBackSourceAsParent(gameObject, gameObject.GetComponent<AudioSource>());
            }
               
               gameObject.SetActive(false);
        }


    }
}
