using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Interface;

public class SGT_Selected : MonoBehaviour, ISelectable<GameObject>
{


    public GameObject SelectedObject = null;
    public GameObject Selector = null;
    public bool SelectedNow = false;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject SelectedEvent = new SGTEventGameObject();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject NotSelectedEvent = new SGTEventGameObject();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject OnClickedEvent = new SGTEventGameObject();

    private bool SelectOnce = false;
    public void Selected(GameObject Value)
    {
        if (!SelectOnce)
        {
            SelectedObject = gameObject;
            Selector = Value;
            SelectedNow = true;
            SelectedEvent.Invoke(Value);

            SelectOnce = true;
        }

    }

    public void NotSelected(GameObject Value)
    {
        if (SelectOnce)
        {
            SelectedObject = null;
            Selector = null;
            SelectedNow = false;
            NotSelectedEvent.Invoke(Value);
            SelectOnce = false;
        }
      
    }

    public void OnClicked(GameObject Value)
    {
        SelectedObject = Value;
        OnClickedEvent.Invoke(Value);
    }



}
