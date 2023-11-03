using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SGT_Tools.Interface;
using Sirenix.OdinInspector;

public class SGT_Raycast : MonoBehaviour
{
    [Tooltip("You can assign sender object or empty will send this gameobject")]
    public GameObject SendThis;
    public LayerMask _LayerMask;
    public Vector3 Direction = new Vector3(0,1,0);
    public float MaxDistance;
    public bool RaycastSelect = true;
    public SGT_Policy Policy;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject HitThis = new SGTEventGameObject();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventVector3 HitThisPointOnce = new SGTEventVector3();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventVector3 HitThisPointContinues = new SGTEventVector3();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent NotHit = new UnityEvent();

    ISelectable<GameObject> _HitObject;
    public void RayCastIt()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Direction) , out hit, MaxDistance, _LayerMask))
        {


            if (hit.collider.gameObject != null && !(SGT_Policy.Check(hit.collider.gameObject, Policy)))
            {
                if (hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>() != null)
                {
                    if (_HitObject!=null)
                    {
                        if (_HitObject != hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>())
                        {
                            if (RaycastSelect)
                            {
                                _HitObject.NotSelected(SendThis == null ? gameObject : SendThis);
                            }
                            
                            HitThis.Invoke(SendThis == null ? gameObject : SendThis);
                            HitThisPointOnce.Invoke(hit.point);
                        }
                           
                    }

                    //if (_HitObject!=null)
                    //{
                        

                    //    if (_HitObject == hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>())
                    //    {

                    //        //return;
                    //    }
                    //}

                   

                    _HitObject = hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>();
                    if (RaycastSelect)
                    {
                        hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>().Selected(SendThis == null ? gameObject : SendThis);
                    }
                        
                                       
                }
                if (hit.collider.gameObject.GetComponentInChildren<IDamageble<GameObject,Vector3, Vector3>>() != null)
                {
                    hit.collider.gameObject.GetComponentInChildren<IDamageble<GameObject, Vector3, Vector3>>().Damage(SendThis == null ? gameObject : SendThis, hit.point,hit.normal);
                }

                HitThisPointContinues.Invoke(hit.point);
            }
            

            if (_HitObject != hit.collider.gameObject.GetComponentInChildren<ISelectable<GameObject>>())
            {
                if (RaycastSelect)
                {
                    _HitObject.NotSelected(SendThis == null ? gameObject : SendThis);
                    _HitObject = null;
                }
            }

            }
        else
        {
            if (_HitObject != null)
            {
                if (RaycastSelect)
                {
                    _HitObject.NotSelected(SendThis == null ? gameObject : SendThis);
                }
                
                NotHit.Invoke();

            _HitObject = null;
            }
        }

    }


}
