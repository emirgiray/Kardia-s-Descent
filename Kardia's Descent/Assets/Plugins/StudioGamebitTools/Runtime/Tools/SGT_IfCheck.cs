using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Tools;
using SGT_Tools.Variables;

namespace SGT_Tools.Tools
{


    public class SGT_IfCheck : MonoBehaviour
    {

        public Vector2 DynamicRange = new Vector2(-50,100);
        [MinMaxSlider("DynamicRange", true)]
        public Vector2 MinMaxValue = new Vector2(-10,10);
        public SGTIntVariables IntValue;
        public SGTFloatVariables FloatValue;

        public UnityEvent Success = new UnityEvent();
        public UnityEvent NotSuccess = new UnityEvent();



        public void CheckIfTrueOrNot(float Value)
        {

            if (gameObject.activeInHierarchy)
            {

                 if (Value>=MinMaxValue.x&& Value <= MinMaxValue.y)
                {
                    Success.Invoke();
                }
                else
                {
                    NotSuccess.Invoke();
                }
            }
           

        }



        public void CheckValueIfTrueOrNot()
        {

            if (gameObject.activeInHierarchy)
            {
                if (IntValue != null)
                {
                    if (IntValue.Value >= MinMaxValue.x && IntValue.Value <= MinMaxValue.y)
                    {
                        Success.Invoke();
                    }
                    else
                    {
                        NotSuccess.Invoke();
                    }
                }


                if (FloatValue != null)
                {
                    if (FloatValue.Value >= MinMaxValue.x && FloatValue.Value <= MinMaxValue.y)
                    {
                        Success.Invoke();
                    }
                    else
                    {
                        NotSuccess.Invoke();
                    }
                }
            }




          


        }



    }

}

