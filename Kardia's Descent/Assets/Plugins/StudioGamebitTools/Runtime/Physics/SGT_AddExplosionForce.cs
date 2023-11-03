using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.Physic
{

    public class SGT_AddExplosionForce : MonoBehaviour
    {



        public float radius = 1.0F;
        public float power = 10.0F;

        public Collider[] colliders;
        public string TagSelection = "";





        public void AddExplosionForce(Transform Center)
        {

            Collider[] colliders = Physics.OverlapSphere(Center.position, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, Center.position, radius, 3.0F);
            }


        }


        public void AddExplosionForceOnlyTheseColliders(Transform Center)
        {

           
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, Center.position, radius, 3.0F);
            }


        }


        public void AddExplosionForceOnlyTag(Transform Center)
        {

            Collider[] colliders = Physics.OverlapSphere(Center.position, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null&&rb.gameObject.CompareTag(TagSelection))
                    rb.AddExplosionForce(power, Center.position, radius, 3.0F);
            }


        }



        public void SetRadius(float Value)
        {

            radius = Value;

        }


        public void SetPower(float Value)
        {

            power = Value;

        }



    }








}

