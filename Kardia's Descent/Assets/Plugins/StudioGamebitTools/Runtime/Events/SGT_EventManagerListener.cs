using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SGT_EventManagerListener : MonoBehaviour
{
    public SGT_EventManager EventManager;
    [PropertyOrder(1)]
    public Listenerlist[] EventListeners;

    private void OnEnable()
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            EventManager.AddListener(EventListeners[i].EventName, this);
        }
        
    }

    private void OnDisable()
    {

        for (int i = 0; i < EventListeners.Length; i++)
        {
            EventManager.RemoveListener(EventListeners[i].EventName, this);
        }
        
    }


    public void TriggerEventNone(string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName== EventName)
            {
                EventListeners[i].NoneEvent.Invoke();
                break;
            }
        }
        
    }

    public void TriggerEventFloat(float Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].FloatEvent.Invoke(Value);
                break;
            }
        }

        
    }

    public void TriggerEventInt(int Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].IntEvent.Invoke(Value);
                break;
            }
        }

        
    }

    public void TriggerEventGameObject(GameObject Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].GameObjectEvent.Invoke(Value);
                break;
            }
        }

        
    }

    public void TriggerEventString(string Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].StringEvent.Invoke(Value);
                break;
            }
        }

        
    }

    public void TriggerEventBool(bool Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].BoolEvent.Invoke(Value);
                break;
            }
        }
        
    }

    public void TriggerEventCustom(Object Value, string EventName)
    {
        for (int i = 0; i < EventListeners.Length; i++)
        {
            if (EventListeners[i].EventName == EventName)
            {
                EventListeners[i].ObjectEvent.Invoke(Value);
                break;
            }
        }
        
    }


#if UNITY_EDITOR
    [Button("Easy Assign EventManager", ButtonSizes.Medium), GUIColor(0, 1, 0)]
    [PropertyOrder(0)]
    public void FindEventManager()
    {
        EventManager = (SGT_EventManager)AssetDatabase.LoadAssetAtPath("Assets/Scripts/EventManager.asset", typeof(SGT_EventManager));
    }
#endif



}




[System.Serializable]
public class Listenerlist
{
    [BoxGroup("$EventName")]
    [Required]
    public string EventName = "";
    [BoxGroup("$EventName")]
    public ParameterType Parameters = ParameterType.None;

    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.None)]
    public UnityEvent NoneEvent = new UnityEvent();
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.Float)]
    public UnityEvent<float> FloatEvent;
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.Int)]
    public UnityEvent<int> IntEvent;
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.GameObject)]
    public UnityEvent<GameObject> GameObjectEvent;
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.String)]
    public UnityEvent<string> StringEvent;
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.Bool)]
    public UnityEvent<bool> BoolEvent;
    [BoxGroup("$EventName")]
    [ShowIf("Parameters", ParameterType.Custom)]
    public UnityEvent<Object> ObjectEvent;
    public enum ParameterType { Int, Float, String, GameObject, Bool, Custom, None }
}
