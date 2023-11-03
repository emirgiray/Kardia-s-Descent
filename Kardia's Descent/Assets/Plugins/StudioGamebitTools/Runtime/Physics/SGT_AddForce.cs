using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Physic
{


public class SGT_AddForce : MonoBehaviour
{
    //Objeleri addforce yapmak için kullanılır.

    
        public float Speed;
        public GameObject ForcedObject = null;
        public Rigidbody rb = null;
        public Vector3 _Direction = new Vector3();

        public void AddForceThis()
        {
            if (ForcedObject != null)
            {
            
                rb.AddForce(_Direction * Speed);
            }

        }


        public void AddForceWithDirection(Vector3 Value)
        {
            if (ForcedObject != null)
            {

                rb.AddForce(Value * Speed);
            }

        }


        //Transform'u dışardan değiştirebilirsin.
        public void SetDirection(Vector3 Value)
        {
            _Direction = Value;
        }


        //Speed'i dışardan değiştir.
        public void SetSpeed(float value)
        {
            Speed = value;
        }

        //Dışardan obje atamak için kullanılır.
        public void SetObject(GameObject Value)
        {
            ForcedObject = Value;

        }

        public void SetRigidbody(Rigidbody Value)
        {
            rb = Value;
        }

  }
}