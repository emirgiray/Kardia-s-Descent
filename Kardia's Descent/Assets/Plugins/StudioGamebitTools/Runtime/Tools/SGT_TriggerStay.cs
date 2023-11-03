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




    private void OnTriggerStay(Collider other)
    {
        if (Policy != null && other != null && !(SGT_Policy.Check(other.gameObject, Policy)))
        {
            TriggerStay.Invoke(other.gameObject);

        }

    }



}
