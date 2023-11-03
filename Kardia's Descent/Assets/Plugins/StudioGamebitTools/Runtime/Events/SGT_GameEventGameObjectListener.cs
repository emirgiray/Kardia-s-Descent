

using UnityEngine;
using UnityEngine.Events;


    public class SGT_GameEventGameObjectListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public SGT_GameEventGameObject Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<GameObject> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(GameObject Value)
    {
        if (gameObject.activeInHierarchy)
        {
            Response.Invoke(Value);
        }

    }
}
