

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Variables
{


    [CreateAssetMenu(fileName = "SGTIntVariables", menuName = "SGTVariables/SGTIntVariables", order = 1)]
    public class SGTIntVariables : ScriptableObject
    {

        public int Value = 0;


        public void IncreaseValue(int i)
        {

            Value = Value+i;
        }

        public void DecreaseValue(int i)
        {

            Value = Value - i;
        }
        public void ResetThisValue(int i)
        {
            Value = i;

        }
    }
}