

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Variables
{


    [CreateAssetMenu(fileName = "SGTBoolVariables", menuName = "SGTVariables/SGTBoolVariables", order = 1)]
    public class SGTBoolVariables : ScriptableObject
    {

        public bool Value = false;



        public void ChangeValue(bool NewValue)
        {

            Value = NewValue;
        }

    }
}