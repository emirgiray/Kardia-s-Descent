


using UnityEngine;
using UnityEngine.Events;


    public class SGT_GameEventIntListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public SGT_GameEventInt Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<int> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(int Value)
    {
        if (gameObject.activeInHierarchy)
        {
            Response.Invoke(Value);
        }

    }
}