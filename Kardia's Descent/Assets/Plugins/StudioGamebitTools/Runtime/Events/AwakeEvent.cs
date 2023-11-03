
using UnityEngine;
using UnityEngine.Events;

public class AwakeEvent : MonoBehaviour
{
    public UnityEvent AwakeEvents = new UnityEvent();

    
    void Awake()
    {

        AwakeEvents.Invoke();

    }


}