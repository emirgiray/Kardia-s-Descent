using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Physic
{


public class SGT_RigidbodyControl : MonoBehaviour
{

    public Rigidbody CurrentRigidbody = null;



    public void SetCurrentRigidbody(GameObject Value)
    {
        CurrentRigidbody = Value.GetComponentInChildren<Rigidbody>();

    }


    public void SetKinematic(bool Value)
    {

        CurrentRigidbody.isKinematic = Value;

    }

    public void SetGravity(bool Value)
    {

        CurrentRigidbody.useGravity = Value;

    }


        public void SetDrag(float Value)
        {
            CurrentRigidbody.drag = Value;

        }


        public void SetAngularDrag(float Value)
        {
            CurrentRigidbody.angularDrag = Value;
            
        }


        public void AddForce(Vector3 Value)
        {
            CurrentRigidbody.AddForce(Value);

        }


        public void FreezeAllOrNone(bool Value)
        {

            if (Value)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.None;
            }
            

        }


        public void FreezeRotate()
        {

         
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
      

        }

        public void FreezePosition()
        {

       
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezePosition;
            

        }

        public void FreezePositionCoordinate(int i)
        {
            if (i==0)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezePositionX;
            }
            else if (i==1)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            }
            else if (i==2)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
            }


        }

        public void FreezeRotationCoordinate(int i)
        {
            if (i == 0)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
            }
            else if (i == 1)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
            }
            else if (i == 2)
            {
                CurrentRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
            }


        }


        public void VelocityZero()
        {

            CurrentRigidbody.velocity = Vector3.zero;

        }


        public void VelocitySet(Vector3 Value)
        {

            CurrentRigidbody.velocity = Value;

        }


        public void AngularSpeedSet(Vector3 Value)
        {
            CurrentRigidbody.angularVelocity = Value;

        }
        public void AngularSpeedReset()
        {
            CurrentRigidbody.angularVelocity = Vector3.zero;

        }




    }
}