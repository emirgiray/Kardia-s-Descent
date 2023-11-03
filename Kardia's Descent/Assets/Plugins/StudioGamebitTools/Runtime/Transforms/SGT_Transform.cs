using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.Transforms
{

    public class SGT_Transform : MonoBehaviour
{

    public GameObject ThisObject;
        public bool Rotation = false;
        public Vector3 RotateThis;



        public void MoveHere(Vector3 Value)
        {
            ThisObject.transform.position = Value;
        }

        public void MoveHerebyTransform(Transform Value)
        {
            ThisObject.transform.position = Value.position;
            if (Rotation)
            {
                ThisObject.transform.rotation = Value.rotation;
            }
        }

        public void TransformMakeNull(Transform Value)
        {
            Value.parent = null;

        }

        public void TransformMakeParent(Transform Value)
        {
            ThisObject.transform.parent = Value;

        }

        public void SetScale(float Value)
        {
            ThisObject.transform.localScale = new Vector3(Value, Value, Value);
        }

        public void SetScaleY(float Value)
        {
            ThisObject.transform.localScale = new Vector3(ThisObject.transform.localScale.x, Value, ThisObject.transform.localScale.z);
        }

        public void SetScaleZ(float Value)
        {
            ThisObject.transform.localScale = new Vector3(ThisObject.transform.localScale.x, ThisObject.transform.localScale.y, Value);
        }

        public void SetScaleX(float Value)
        {
            ThisObject.transform.localScale = new Vector3(Value, ThisObject.transform.localScale.y, ThisObject.transform.localScale.z);
        }


        public void SetRotate()
        {
            transform.eulerAngles = RotateThis;
        }



    }
}
