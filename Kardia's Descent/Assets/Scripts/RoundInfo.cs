using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    
    public void Rearrange(GameObject obj)
    {
        //set the first child of this gameobject to the last child
        obj.transform.SetAsLastSibling();
        //also move the first element to last
        /*objects.Add(obj);
        objects.Remove(obj);*/
    }
    
    public void AddObject(GameObject obj)
    {
        objects.Add(obj);
    }
}
