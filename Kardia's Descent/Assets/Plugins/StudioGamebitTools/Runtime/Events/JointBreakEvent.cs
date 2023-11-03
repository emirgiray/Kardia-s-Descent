using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JointBreakEvent : MonoBehaviour
{
    public SGTEventFloat BreakEvent;
    void OnJointBreak(float breakForce)
    {
        if (this.enabled)
        {
            BreakEvent.Invoke(breakForce);
        }
        
        
    }
}
