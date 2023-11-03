using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGT_OverlapSphere : MonoBehaviour
{

    public float Radius = 1;
    public LayerMask OnlyCollideThisLayers;

    public SGTEventColliderArray SelectedColliders = new SGTEventColliderArray();

    /// <summary>
    /// Select Object in the Radius
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    void SelectRadius(Vector3 center)
    {
        Collider[] selectables;
        selectables = Physics.OverlapSphere(center, Radius, OnlyCollideThisLayers);
        SelectedColliders.Invoke(selectables);

    }

}
