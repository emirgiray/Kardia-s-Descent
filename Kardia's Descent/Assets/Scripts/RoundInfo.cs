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
        objects.Add(objects[0]);
        objects.RemoveAt(0);//todo çalışmıyo alo
    }
    
    public void AddObject(GameObject obj)
    {
        objects.Add(obj);
    }
}
