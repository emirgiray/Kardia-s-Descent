using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[DefaultExecutionOrder(100)]
public class SGT_AddItemToPooler : MonoBehaviour
{

    //Example how to use 

    //Add this first and assign  "public SGT_PoolerManagerCenter ManagerCenter;"
    // GameObject Temp =  ManagerCenter.Pooler.GetPooledObject(Particle);
    // You should reposition
    //Temp.transform.position = transform.position;
    //Activate it for visible in new position.
    // Temp.SetActive(true);



    //public SGT_PoolerManagerCenter PoolerManagerCenter;

    [InfoBox("If you want to add a object to current pooler in the scene, use it from this script. This script add too pooler this object")]
    public ObjectPoolItem[] Items;


    private void Awake()
    {
        AddItemToPooler();
    }


    public void AddItemToPooler()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            //PoolerManagerCenter.Pooler.AddItemToPooler(Items[i]);
            SGT_ObjectPooler.instance.AddItemToPooler(Items[i]);
        }

        
        
    }

}
