

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class CustomEventFloat : MonoBehaviour
{


    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void CustomEmitterFunc(float Value)
    {
        if (gameObject.activeInHierarchy)
        {
            CustomEvents.Invoke(Value);
        }


    }
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventFloat CustomEvents;


    public void OnPrint(string Value)
    {
        print(Value);

    }

    public void OnPrintFloat(float Value)
    {
        print("Float Değeri : " + Value);

    }
}
