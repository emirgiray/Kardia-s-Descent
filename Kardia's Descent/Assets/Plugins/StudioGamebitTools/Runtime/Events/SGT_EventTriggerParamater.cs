using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SGT_EventTriggerParamater : MonoBehaviour
{
    [Required]
    public string EventName = "";
    public SGT_EventManager EventManager;


    public void EventFire(float Value)
    {

        EventManager.TriggerEventFloat(EventName,Value);
    }

    public void EventFired(int Value)
    {
        EventManager.TriggerEventInt(EventName, Value);
    }

    public void EventFired(GameObject Value)
    {
        EventManager.TriggerEventGameObject(EventName, Value);
    }
    public void EventFired(string Value)
    {
        EventManager.TriggerEventString(EventName, Value);
    }


    public void EventFired(bool Value)
    {
        EventManager.TriggerEventBool(EventName, Value);
    }

    public void EventFired(Object Value)
    {
        EventManager.TriggerEventCustom(EventName, Value);
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
