using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Variables
{


[CreateAssetMenu(fileName = "SGTFloatVariables", menuName = "SGTVariables/SGTFloatVariables", order = 1)]
public class SGTFloatVariables : ScriptableObject
{

    public float Value = 0;

        public void IncreaseValue(float i)
        {

            Value = Value + i;
        }

        public void DecreaseValue(float i)
        {

            Value = Value - i;
        }

        public void ResetThisValue(float i)
        {
            Value = i;

        }
    }
}