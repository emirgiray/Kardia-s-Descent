
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

    [CreateAssetMenu(menuName = "SGT/Event/GameEvent_")]
    public class SGT_GameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<SGT_GameEventListener> eventListeners = 
            new List<SGT_GameEventListener>();

    [Button]
        public void Raise()
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
        }

        public void RegisterListener(SGT_GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(SGT_GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
