using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Variables;

namespace SGT_Tools.AI
{
    [DefaultExecutionOrder(100)]
    public class SGT_LevelSystemSetter : MonoBehaviour
    {


        /// <summary>
        /// This script work with SGT_LevelSystem together. You can change level value from this script.
        /// </summary>

        public LevelSystemSetterClass[] _LevelSystemSetter;



        private void Start()
        {
            UpdateValueFromlist();

        }



        [Button]
        public void UpdateValueFromlist()
        {

            for (int i = 0; i < _LevelSystemSetter.Length; i++)
            {
                if ( _LevelSystemSetter[i].Level.Value<_LevelSystemSetter[i].LevelValueFromIndex.Value.Count)
                {
                    _LevelSystemSetter[i].UpdateValueEvent.Invoke(_LevelSystemSetter[i].LevelValueFromIndex.Value[_LevelSystemSetter[i].Level.Value]);    
                }
                
            }
        }


    }

    [System.Serializable]
    public class LevelSystemSetterClass
    {

        public SGTIntVariables Level;
        [Tooltip("Set this value from list. Level index is like level")]
        public SGTFloatListVariables LevelValueFromIndex;

        [Tooltip("You can use this value from another script and also convert as int")]
        public UnityEvent<float> UpdateValueEvent = new UnityEvent<float>();

    }


}

