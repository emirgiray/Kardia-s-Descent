
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class CustomEvent2 : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent CustomEvents2;

    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void CustomEmitterFunc2()
    {
        if (gameObject.activeInHierarchy)
        {
            CustomEvents2.Invoke();
        }
    }


    public void OnPrint(string Value)
    {
        print(Value);

    }

    public void OnPrintFloat(float Value)
    {
        print("Float Değeri : " + Value);

    }

}
