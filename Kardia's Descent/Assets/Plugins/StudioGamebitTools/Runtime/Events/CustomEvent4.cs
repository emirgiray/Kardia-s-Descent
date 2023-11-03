
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class CustomEvent4 : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent CustomEvents4;

    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void CustomEmitterFunc4()
    {
        if (gameObject.activeInHierarchy)
        {
            CustomEvents4.Invoke();
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
