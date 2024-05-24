using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SerializedDictionary", menuName = "ScriptableObjects/SerializedDictionary", order = 0)]
public class SerializedDictionary : SerializedScriptableObject
{
    //[ColorUsage( true,  true)]
    public Dictionary<int, Color> IntColorMap;
}
