using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SGT_Tools.Tools
{
    //Bu script Patlayan particle aktif oldukdan sonra kendini disable etmesi için yazıldı.
    public class SGT_DestroySelf : MonoBehaviour
    {

        public float DestroyTime = 0;
        [SerializeField] private bool DestroyTotaly = false;
        public UnityEvent BeforeDestroyed = new UnityEvent();
        public UnityEvent Destroyed = new UnityEvent();
        private bool DestroyCancel = false;
        IEnumerator co;




        void OnDisable()
        {
            StopCoroutine(co); // stop it.

        }

        void OnEnable()
        {
            co = Destroyit(); // create an IEnumerator object
            StartCoroutine(co); // start the coroutine

        }



        /// <summary>
        /// Dışardan custom olarak bu işlemi yapabilmek için kullanabilirsin.
        /// </summary>
        public void DestroyNow()
        {
            if (DestroyTotaly)
            {
                BeforeDestroyed.Invoke();
                Destroy(gameObject);
                Destroyed.Invoke();
            }
            else
            {
                BeforeDestroyed.Invoke();
                gameObject.SetActive(false);
                Destroyed.Invoke();
            }

        }

        IEnumerator Destroyit()
        {
            yield return new WaitForSeconds(DestroyTime);
            if (!DestroyCancel)
            {
                if (DestroyTotaly)
                {
                    BeforeDestroyed.Invoke();
                    Destroy(gameObject);
                    Destroyed.Invoke();
                }
                else
                {
                    BeforeDestroyed.Invoke();
                    gameObject.SetActive(false);
                    Destroyed.Invoke();
                }
            }

        }


        //Eğer elimize alırsak bunu çalıştırmalıyız.
        public void CancelDestroy()
        {
            DestroyCancel = true;

        }



        //Elimizeden bırakınca da tekrar bunu çalıştırmalıyız ki destroy olsun.
        public void DestroyStart()
        {

            DestroyCancel = false;
            if (this.enabled)
            {
                StartCoroutine(Destroyit()); // start the coroutine
            }

        }



    }
}