using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    
    public void Rearrange()
    {
        //set the first child of this gameobject to the last child
        objects[0].transform.SetAsLastSibling();
        //also move the first element to last
        objects.RemoveAt(0);
        objects.Add(objects[0]);
    }
    
    public void AddObject(GameObject obj)
    {
        objects.Add(obj);
    }
}
