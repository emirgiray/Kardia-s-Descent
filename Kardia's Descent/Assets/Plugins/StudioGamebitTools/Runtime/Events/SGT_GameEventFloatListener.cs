

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

    public class SGT_GameEventFloatListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public SGT_GameEventFloat Event;

    [DrawWithUnity]
    [Tooltip("Response to invoke when Event is raised.")]
    public SGTEventFloat Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(float Value)
    {
        if (gameObject.activeInHierarchy)
        {
            Response.Invoke(Value);
        }

    }
}
