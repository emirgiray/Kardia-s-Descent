using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace SGT_Tools.AI
{

public class SGT_DamageGetter : MonoBehaviour
{


        public SGT_Damageble _Damageble;
        public float DelayForSecondDamage = 0.3f;
        public string Tag = "Player";
        public UnityEvent<GameObject> GetDamage = new UnityEvent<GameObject>();
        private bool DamagePause = false;
        private void Start()
        {
            if (GetComponentInParent<SGT_Damageble>()!=null)
            {
                _Damageble = GetComponentInParent<SGT_Damageble>();
            }
            

        }



        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.CompareTag(Tag))
            {
                if (!DamagePause)
                {

                     _Damageble.Damage(collision.gameObject,collision.contacts[0].point, collision.contacts[0].normal);
                    GetDamage.Invoke(collision.gameObject);
                    StartCoroutine(Delay());
                }
               
            }


        }

        IEnumerator Delay()
        {
            DamagePause = true;
            yield return new WaitForSecondsRealtime(DelayForSecondDamage);
            DamagePause = false;
        }
    }


    

}
