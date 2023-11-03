

using UnityEngine;
using UnityEngine.Events;


    public class SGT_GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public SGT_GameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
        if (gameObject.activeInHierarchy)
        {
            Response.Invoke();
        }
            
        }
    }
