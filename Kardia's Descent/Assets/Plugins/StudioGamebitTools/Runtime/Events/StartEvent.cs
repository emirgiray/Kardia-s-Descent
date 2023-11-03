
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class StartEvent : MonoBehaviour
{
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StartEvents = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {

        StartEvents.Invoke();
    }

  
}
