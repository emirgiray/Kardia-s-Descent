using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Tools
{



public class SGT_Trigger : MonoBehaviour
{
    public SGT_Policy Policy;
    

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventGameObject TriggerEnter;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventGameObject TriggerExit;

    




    private void OnTriggerEnter(Collider other)
    {
        if (Policy !=null&& other != null && !(SGT_Policy.Check(other.gameObject, Policy)))
        {
            TriggerEnter.Invoke(other.gameObject);
           
        }
         if (Policy==null)
        {
            TriggerEnter.Invoke(other.gameObject);
            
        }
        
    }





    private void OnTriggerExit(Collider other)
    {
        if (Policy != null && other != null && !(SGT_Policy.Check(other.gameObject, Policy)))
        {
            TriggerExit.Invoke(other.gameObject);
        }      
    }

    

    

}
}