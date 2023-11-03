
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class OnDisableEvent : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent OnDisableEvents = new UnityEvent();

    private void OnDisable()
    {
        OnDisableEvents.Invoke();

    }
}


