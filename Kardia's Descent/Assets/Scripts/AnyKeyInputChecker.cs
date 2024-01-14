using System.Collections;
using System.Collections.Generic;
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
        }
    }
}
