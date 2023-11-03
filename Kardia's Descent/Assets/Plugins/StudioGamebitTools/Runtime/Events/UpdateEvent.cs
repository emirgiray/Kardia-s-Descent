
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class UpdateEvent : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent UpdateEvents = new UnityEvent();

    
    void Update()
    {

        UpdateEvents.Invoke();
    }


}