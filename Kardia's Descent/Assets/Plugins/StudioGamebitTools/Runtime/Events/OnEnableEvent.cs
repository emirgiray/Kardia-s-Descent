

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class OnEnableEvent : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent OnEnableEvents = new UnityEvent();

    private void OnEnable()
    {
        OnEnableEvents.Invoke();

    }
}
