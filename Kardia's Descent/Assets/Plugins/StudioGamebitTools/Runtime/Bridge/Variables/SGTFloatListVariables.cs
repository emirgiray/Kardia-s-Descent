using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SGTFloatListVariables", menuName = "SGTVariables/SGTFloatListVariables", order = 1)]
public class SGTFloatListVariables : ScriptableObject
{

    public List<float> Value = new List<float>();


    public void IncreaseValue(float newValue,int index)
    {

        Value[index] = Value[index] + newValue;
    }

    public void DecreaseValue(float newValue, int index)
    {

        Value[index] = Value[index] - newValue;
    }

    public void ResetThisValue(float newValue, int index)
    {
        Value[index] = newValue;

    }


    public void AddNewValue(float newValue)
    {
        Value.Add(newValue) ;

    }


    public void RemoveThisValue(float newValue)
    {
        Value.Remove(newValue);

    }


    public void ClearAllValues()
    {

        Value.Clear();
    }
}
