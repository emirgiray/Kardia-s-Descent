using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class WaitAndRaiseEvent : MonoBehaviour
{

    public float WaitTime;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent OnRaiseEvents = new UnityEvent();



    public void WaitAndRaise()
    {
        StartCoroutine(RaiseThis());

    }



    IEnumerator RaiseThis()
    {
        yield return new WaitForSecondsRealtime(WaitTime);
        OnRaiseEvents.Invoke();

    }

    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    
    public void TestNow()
    {
        OnRaiseEvents.Invoke();
    }
    

}
