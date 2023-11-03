using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Transforms
{


public class SGT_AngleCheck : MonoBehaviour
{
    [Tooltip("Hangi objeden açı bakacağımızı belirler.")]
    public Transform Source = null;

    [Tooltip("Hangi hedefin target'da olacağını belirleyebilirsin.Player'i atacağız genelde.")]
    public Transform Target = null;
    public float MaxAngle = 180;
    [Tooltip("Eğer Distance belirlemek istersen ayarlayabilirsin.Eğer 0 üzeri olursa distance'i dikkate alır.0 ise almaz.")]
    public float MaxDistance = 0;
    [Tooltip("Bu Compenent'i kapatmak istersen disable et.")]
    public bool DisableThisComponent = true;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent InAngleTrue = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent InAngleFalse = new UnityEvent();

    private bool AngleTrueOnce;
    private bool AngleFalseOnce;
    private float CurrentDistance;
    

    public void UpdateThis()
    {


        Vector3 targetDir = Target.position - Source.position;
        float angle = Vector3.Angle(targetDir, Source.forward);

        CurrentDistance = Vector3.Distance(Target.position, Source.position);




        if (angle < MaxAngle && CurrentDistance <= MaxDistance)
        {
            if (!AngleTrueOnce)
            {

                InAngleTrue.Invoke();

                AngleTrueOnce = true;
                AngleFalseOnce = false;





            }

        }
        else
        {
            if (!AngleFalseOnce)
            {

                InAngleFalse.Invoke();

                AngleFalseOnce = true;
                AngleTrueOnce = false;


            }

        }
    }



    //Target'i değiştirmek için kullanılabilir.
    public void ChangeTarget(Transform newTarget)
    {

        Target = newTarget;
    }


    //Source'i değiştirmek için kullanılabilir.
    public void ChangeSource(Transform newSource)
    {

        Source = newSource;
    }


    //MaxAngle'i değiştirmek için kullanılabilir.
    public void ChangeMaxAngle(float NewAngle)
    {

        MaxAngle = NewAngle;
    }
  }
}
