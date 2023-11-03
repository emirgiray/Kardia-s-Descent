


using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(menuName = "SGT/Event/GameEventGameObject_")]
public class SGT_GameEventGameObject : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<SGT_GameEventGameObjectListener> eventListeners =
        new List<SGT_GameEventGameObjectListener>();

    public void Raise(GameObject Value)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(Value);
    }

    public void RegisterListener(SGT_GameEventGameObjectListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SGT_GameEventGameObjectListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
