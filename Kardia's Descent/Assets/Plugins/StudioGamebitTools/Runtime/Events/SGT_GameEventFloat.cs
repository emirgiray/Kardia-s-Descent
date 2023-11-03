
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


    [CreateAssetMenu(menuName = "SGT/Event/GameEventFloat_")]
public class SGT_GameEventFloat : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<SGT_GameEventFloatListener> eventListeners =
        new List<SGT_GameEventFloatListener>();
    [Button]
    public void Raise(float Value)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(Value);
    }

    public void RegisterListener(SGT_GameEventFloatListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SGT_GameEventFloatListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
