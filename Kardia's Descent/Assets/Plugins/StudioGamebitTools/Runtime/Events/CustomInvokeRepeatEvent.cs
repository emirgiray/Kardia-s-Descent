
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class CustomInvokeRepeatEvent : MonoBehaviour
{

    [Tooltip("Select Start Time")]
    public float StartTime = 0;
    [Tooltip("Select Repeat Time")]
    public float RepeatTime = 0;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StartInvokeEvent = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent LaunchEvent = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StopInvokeEvent = new UnityEvent();

    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    //Invoke'u Çalıştırmak için kullanılır.
    public void StartInvoke()
    {

        InvokeRepeating("InvokeLaunch", StartTime, RepeatTime);

    }
    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    //Aralıklarla çalışacak fonksiyon.
    public void InvokeLaunch()
    {

        LaunchEvent.Invoke();

    }


    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    //Invoke repeat'i durdurur.
    public void StopInvoke()
    {

        CancelInvoke();
        StopInvokeEvent.Invoke();

    }


}
