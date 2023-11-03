using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;


[System.Serializable]

[DefaultExecutionOrder(-100)]
public class ObjectPoolItem {

  public GameObject objectToPool;
  public int amountToPool;
  public bool PoolExpand;
  public bool ParentObject = false;
  [Multiline]
  public string Aciklama;
}

public class SGT_ObjectPooler : MonoBehaviour
{
    public static SGT_ObjectPooler instance;
    public SGT_PoolerManagerCenter PoolerManagerCenter;
    public float PoolTime = 0;
    public Transform CreateUnderThisObject;
    public List<ObjectPoolItem> itemsToPool = new List<ObjectPoolItem>();
    public List<GameObject> pooledObjects= new List<GameObject>();



    private void Awake()
    {
        
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            PoolerManagerCenter.Pooler = this;
            gameObject.transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
       
    }


    // Use this for initializations
    void Start()
    {

        Invoke("PoolNow",PoolTime);

    }




    private void PoolNow()
    {

        for (int i = 0; i < itemsToPool.Count; i++)
        {


            for (int k = 0; k < itemsToPool[i].amountToPool; k++)
            {
                GameObject obj = (GameObject)Instantiate(itemsToPool[i].objectToPool);
                obj.SetActive(false);
                obj.transform.parent = gameObject.transform;
                obj.name = itemsToPool[i].objectToPool.name;
                if (itemsToPool[i].ParentObject) //If ParentObject true, Parent here, Otherwise no parent
                {
                    if (CreateUnderThisObject != null)
                    {
                        obj.transform.parent = CreateUnderThisObject;
                    }

                }
                pooledObjects.Add(obj);
            }

        }



    }







    public GameObject GetPooledObject(GameObject DesiredPoolObject)
    {
        

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i]!=null)
            {
                if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name == DesiredPoolObject.name)
                    if (pooledObjects[i].name == DesiredPoolObject.name)

                    {
                        return pooledObjects[i];
                    }
            }
            else
            {
                pooledObjects.RemoveAt(i);
            }
            
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.name == DesiredPoolObject.name)
            {
                if (item.PoolExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    if (item.ParentObject) //If ParentObject true, Parent here, Otherwise no parent
                    {
                        if (CreateUnderThisObject != null)
                        {
                            obj.transform.parent = CreateUnderThisObject;
                        }

                    }
                    obj.name = item.objectToPool.name;
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    public GameObject DestroyObject(GameObject gameobject)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(pooledObjects[i].activeInHierarchy && pooledObjects[i] == gameobject)
            {
                pooledObjects[i].SetActive(false);
                break;
            }
           
        }
           
        return null;

      
    }


    public void AddItemToPooler(ObjectPoolItem Items)
    {
        
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (Items.objectToPool.name==itemsToPool[i].objectToPool.name)
            {
                return;
            }

        }
        
        itemsToPool.Add(Items);

    }



    public void DestroyAllPooledObjects()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {

            Destroy(pooledObjects[i].gameObject);
        }
        
    }



    public void DisableEnableAllPooledObjects(bool Value)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {

            pooledObjects[i].SetActive(Value);
        }

    }




    /// <summary>
    /// If you do not want to lose parent pooled object before scene change, this event must Invoke before scene change.
    /// </summary>
    public void GetParentObjectChildOnMe()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i]!=null)
            {
                pooledObjects[i].transform.parent = gameObject.transform;
                pooledObjects[i].SetActive(false);
            }
          
        }
    }

}
