using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EventManager", menuName = "EventManager", order = 1)]
public class SGT_EventManager : ScriptableObject
{
    public EventsType[] Events;

    public void TriggerEventNone(string Value)
    {


        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == Value)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventNone(Value);
                }
                break;
            }
        }

    }


    public void TriggerEventFloat(string EventName,float Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventFloat(Value, EventName);
                }
                break;
            }
        }
    }

    public void TriggerEventInt(string EventName, int Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventInt(Value, EventName);
                }
                break;
            }
        }
    }


    public void TriggerEventGameObject(string EventName, GameObject Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventGameObject(Value, EventName);
                }
                break;
            }
        }
    }

    public void TriggerEventString(string EventName, string Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventString(Value, EventName);
                }
                break;
            }
        }
    }


    public void TriggerEventBool(string EventName, bool Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventBool(Value, EventName);
                }
                break;
            }
        }
    }

    public void TriggerEventCustom(string EventName, Object Value)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == EventName)
            {
                for (int f = 0; f < Events[i].Eventlistener.Count; f++)
                {
                    Events[i].Eventlistener[f].TriggerEventCustom(Value, EventName);
                }
                break;
            }
        }
    }


    public void AddListener(string Value, SGT_EventManagerListener listener)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName==Value)
            {
                Events[i].Eventlistener.Add(listener);
                break;
            }
        }
    }


    public void RemoveListener(string Value, SGT_EventManagerListener listener)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].EventName == Value)
            {
                Events[i].Eventlistener.Remove(listener);
                break;
            }
        }
    }

}

[System.Serializable]
public class EventsType
{
    [BoxGroup("$EventName")]
    public string EventName = "";
    [BoxGroup("$EventName")]
    public bool Debug = false;
    //[BoxGroup("$EventName")]
    //public ParameterForEvents[] Parameters;
    [BoxGroup("$EventName")]
    [ShowIf("Debug")]
    public List<SGT_EventManagerListener> Eventlistener = new List<SGT_EventManagerListener>();

    [BoxGroup("$EventName")]
    [Button]
    public void TestEvent()
    {
        for (int i = 0; i < Eventlistener.Count; i++)
        {
            for (int f = 0; f < Eventlistener[i].EventListeners.Length; f++)
            {
                if (Eventlistener[i].EventListeners[f].EventName== EventName)
                {
                    if (Eventlistener[i].EventListeners[f].Parameters == Listenerlist.ParameterType.None)
                    {
                        Eventlistener[i].TriggerEventNone(EventName);
                    }
                }

            }

           

        }

    }
}





