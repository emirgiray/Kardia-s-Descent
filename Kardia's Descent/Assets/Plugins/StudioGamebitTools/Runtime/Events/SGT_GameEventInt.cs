

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


    [CreateAssetMenu(menuName = "SGT/Event/GameEventInt_")]
public class SGT_GameEventInt : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<SGT_GameEventIntListener> eventListeners =
        new List<SGT_GameEventIntListener>();

    [Button]
    public void Raise(int Value)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(Value);
    }

    public void RegisterListener(SGT_GameEventIntListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SGT_GameEventIntListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
