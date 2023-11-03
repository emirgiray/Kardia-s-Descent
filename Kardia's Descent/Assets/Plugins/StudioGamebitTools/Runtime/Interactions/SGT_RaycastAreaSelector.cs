using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGT_Tools.Interface;
using Sirenix.OdinInspector;

public class SGT_RaycastAreaSelector : MonoBehaviour
{
    //Notes You need these script first. SGT_OverlapSphere , SGT_Raycast.

    [Tooltip("Select for direction object If its empty, It will be select this object")]
    public GameObject _SelectSourceObject = null;
    [Tooltip("Select for direction of head object")]
    public GameObject _SelectHeadObject = null;
    [Tooltip("Select linecast layers, If nothing select no effect for line cast")]
    public LayerMask lineCastMask;
    public float LookPercentage = 1;
    public float threshold = 0.97f;
    public float AngleGrab = 25;
    public float MaxDistance = 30f;
    public bool DisableComponent = false;
    public GameObject SelectedObject = null;


    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject SelectedObjectEvent = new SGTEventGameObject();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject NotSelectedObjectEvent = new SGTEventGameObject();

    private bool LineCastCheck;

    
    ISelectable<GameObject> _HitObject;
    ISelectable<GameObject> _PreviousObject;
    GameObject _Temp;
    public void Check(Collider[] selectables)
    {
       

        _HitObject = null;
        
    

    var closest = 0f;

        Vector3 fwd = transform.TransformDirection(transform.forward); 
        Vector3 Pos = _SelectSourceObject.transform.position;

        for (int i = 0; i < selectables.Length; i++)
        {
            ISelectable<GameObject> _CurrentHitObject;
            if (selectables[i].GetComponentInChildren<ISelectable<GameObject>>() != null) //Pickable kontrol ediyorum.
            {

                _CurrentHitObject = selectables[i].GetComponentInChildren<ISelectable<GameObject>>();
                




                var vector1 = fwd;
                var vector2 = selectables[i].transform.position - Pos;

                var lookPercentage = Vector3.Dot(vector1.normalized, vector2.normalized);

                Vector3 targetDir = selectables[i].transform.position - _SelectHeadObject.transform.position; //Angle hesaplaması yapılıyor
                float angle = Vector3.Angle(targetDir, _SelectHeadObject.transform.forward);
                float Distance = Vector3.Distance(selectables[i].transform.position, _SelectSourceObject.transform.position);

          
                    LineCastCheck = Physics.Linecast(_SelectHeadObject.transform.position, selectables[i].transform.position, lineCastMask); //eller ile obje arasında obje olup olmadığına bakar.
                


                if (LookPercentage > threshold && lookPercentage > closest  && angle < AngleGrab && !LineCastCheck && !DisableComponent && Distance < MaxDistance)
                {
                
                    closest = lookPercentage;
                   _HitObject = _CurrentHitObject;
                    _Temp = selectables[i].gameObject;

                }
            }
        }


        if (_PreviousObject != null)
        {
            if (_PreviousObject != _HitObject)
            {
                
                _PreviousObject.NotSelected(_SelectSourceObject == null ? gameObject : _SelectSourceObject);
                NotSelectedObjectEvent.Invoke(SelectedObject);
                SelectedObject = null;
                
            }

        }
        if (_HitObject!=null)
        {
            
            SelectObject(_HitObject, _Temp);
        }
        

        _PreviousObject = _HitObject;



    }



    public void SelectObject(ISelectable<GameObject> Value,GameObject Obje)
    {
        if (_HitObject!=_PreviousObject)
        {
            Value.Selected(_SelectSourceObject == null ? gameObject : _SelectSourceObject);
            SelectedObject = Obje;
            SelectedObjectEvent.Invoke(SelectedObject);
        }
        
        
    }

}
