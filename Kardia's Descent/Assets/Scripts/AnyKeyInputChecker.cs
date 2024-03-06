using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class AnyKeyInputChecker : MonoBehaviour
{
   public UnityEvent anyKeyInputEvent;
    void Update()
    {
        if (Input.anyKey)
        {
            anyKeyInputEvent.Invoke();
            /*Debug.Log($"expression");
            Component component = GetComponent<AnyKeyInputChecker>();
            if (component != null)
            {
                Destroy(component);
            }*/
        }
    }

    [Button, GUIColor(1f, 1f, 1f)]
    public void ManualStart()
    {
        anyKeyInputEvent.Invoke();
    }
}
