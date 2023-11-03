using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Tools
{

public class SGT_TriggerTag : MonoBehaviour
{



    public string Tag = "";

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

        if (other.gameObject.CompareTag(Tag))
        {

            TriggerEnter.Invoke(other.gameObject);
        }

    }



    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag(Tag))
        {

            TriggerExit.Invoke(other.gameObject);
        }

    }



        public void DebugTest()
        {


            print("Trigger Test: " +gameObject.name);

        }

}
}