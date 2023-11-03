
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Events
{
    

public class CustomEvent : MonoBehaviour
{
    
    
    [Button(ButtonSizes.Medium),GUIColor(0.4f,0.8f,1)][PropertyOrder(0)]
    public void CustomEmitterFunc()
    {
        if (gameObject.activeInHierarchy)
        {
            CustomEvents.Invoke();
        }
       

    }
    [DrawWithUnity] [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent CustomEvents;
    

    public void OnPrint(string Value)
    {
        print(Value);

    }

    public void OnPrintFloat(float Value)
    {
        print("Float Değeri : "+Value);

    }
}
}