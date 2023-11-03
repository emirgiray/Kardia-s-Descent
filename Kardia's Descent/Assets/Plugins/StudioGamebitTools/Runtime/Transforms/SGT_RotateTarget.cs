using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Transforms
{

    public class SGT_RotateTarget : MonoBehaviour
    {
        public Transform RotateThisObject;
        public float _TurnSpeed;

        public Transform Target;
        public Camera _Camera;


        private void Start()
        {
            if (Target == null)
            {
                _Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }

        }



        public void Update()
        {

            if (Target != null)
            {
                Vector3 direction = Target.transform.position - transform.position;
                direction.y = 0f;
                RotateThisObject.rotation = Quaternion.Slerp(RotateThisObject.rotation, Quaternion.LookRotation(direction), _TurnSpeed);
                return;

            }
            else if (_Camera != null)
            {
                Vector3 direction = _Camera.transform.position - transform.position;
                direction.y = 0f;
                RotateThisObject.rotation = Quaternion.Slerp(RotateThisObject.rotation, Quaternion.LookRotation(direction), _TurnSpeed);


            }




        }




        /// <summary>
        /// You can assign Target In Runtime
        /// </summary>
        /// <param name="Value"></param>
        public void SetTarget(Transform Value)
        {
            Target = Value;

        }


        public void SetCamera(Camera Value)
        {
            _Camera = Value;
        }



        public void SetTargetNull()
        {
            Target = null;

        }


        public void SetCameraNull()
        {
            _Camera = null;

        }
    }
}
