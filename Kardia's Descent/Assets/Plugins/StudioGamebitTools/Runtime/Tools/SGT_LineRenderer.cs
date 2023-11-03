using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Tools
{


public class SGT_LineRenderer : MonoBehaviour
{

    [Header("Create between two object")]
    [Tooltip("Assign a line renderer component require")]
    [SerializeField] private LineRenderer line;
    [Tooltip("Target Position for Line renderer")]
    [SerializeField] private Transform TargetLine;
    [Tooltip("Add offset")]
    [SerializeField] private Vector3 OffsetPosition = Vector3.zero;



    
    void UpdateThis()
    {

        line.SetPosition(0, transform.position);
        line.SetPosition(1, TargetLine.position + OffsetPosition);

    }


    public void AddTarget(GameObject Obje)
    {
        TargetLine = Obje.transform;

    }


    public void RemoveTarget(GameObject Obje)
    {
        TargetLine = null;

    }

    public void SetTargetPosition(Vector3 Value)
    {
        TargetLine.position = Value;
    }



 }
}