using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Bridge;

namespace SGT_Tools.Transforms
{

    public class SGT_Parabole : MonoBehaviour
    {
        [Tooltip("Select empty If you want to move this gameobject")]
        public Transform MoveThisObject = null;
        public Transform StartPos;
        public Transform EndPos;

        public float height = 2;
        public float Speed = 1;
        private float Animation;
        private Vector3 StartPosition;

        [FoldoutGroup("Events")]
        public UnityEvent LerpStartEvent = new UnityEvent();
        [FoldoutGroup("Events")]
        public UnityEvent LerpFinishEvent = new UnityEvent();

        private bool _LerpActive = false;

        void Update()
        {
            if (_LerpActive)
            {
                Animation += Time.deltaTime;


                if (MoveThisObject != null)
                {
                    MoveThisObject.position = SGT_Math.Parabola(StartPosition, EndPos.position, height, Animation / Speed);

                }
                else
                {
                    transform.position = SGT_Math.Parabola(StartPosition, EndPos.position, height, Animation / Speed);
                }


                if (Animation >= Speed)
                {
                    LerpFinishEvent.Invoke();
                    _LerpActive = false;
                }
            }


        }




        [Button]
        public void LerpStarter()
        {
            if (StartPos!=null)
            {
                StartPosition = StartPos.position;
            }
            else
            {
                StartPosition = transform.position;
            }
            _LerpActive = true;
            Animation = 0;
            LerpStartEvent.Invoke();
        }




        public void SetStartPos(Transform Value)
        {
            StartPos = Value;
        }

        public void SetEndPos(Transform Value)
        {
            EndPos = Value;
        }



        public void LerpStarterController(bool Value)
        {

            _LerpActive = Value;

        }


        public void SetSpeed(float Value)
        {
            Speed = Value;
        }

        public void SetHeight(float Value)
        {

            height = Value;
        }

    }

}
