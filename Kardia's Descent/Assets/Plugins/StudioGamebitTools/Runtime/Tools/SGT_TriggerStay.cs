using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SGT_TriggerStay : MonoBehaviour
{
    //Performans gerekçesiyle ayrı yapıldı sadece ihtiyaç duyulduğunda kullanılacak.Not: Bütün layerlarla temas etmemisi iyi olur. Sadece ihtiyaç duyduğu objelerle temas etmesi perfomansı etkiler.

    public SGT_Policy Policy;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventGameObject TriggerStay;

    public void ResetDoOnce()
    {
        doOnce = false;
    }

    bool doOnce = false;
    private void OnTriggerStay(Collider other)
    {
        if (Policy != null && other != null && !(SGT_Policy.Check(other.gameObject, Policy)))
        {
            if (doOnce == false)
            {
                doOnce = true;
                TriggerStay.Invoke(other.gameObject);
            }

            
        }

    }



}
