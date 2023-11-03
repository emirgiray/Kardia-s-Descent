using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Transforms
{
    public class SGT_Translate : MonoBehaviour
    {



        [SerializeField] private Transform ThisObject;
        [SerializeField] private Vector3 Direction;
        [SerializeField] private float Speed = 1;



        public void TranslateDirection()
        {

            if (ThisObject != null)
            {
                ThisObject.transform.Translate(Direction * Speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Direction * Speed * Time.deltaTime);
            }
        }


    }
}
