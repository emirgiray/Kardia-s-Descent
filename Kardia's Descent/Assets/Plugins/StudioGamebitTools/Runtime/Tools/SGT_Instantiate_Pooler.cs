
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SGT_Tools.Tools
{


    public class SGT_Instantiate_Pooler : MonoBehaviour
    {
        public SGT_PoolerManagerCenter PoolerCenter; 
        public GameObject SpawnThisObject;
        public GameObject[] SpawnRandomFromThisList;
        public Transform[] SpawnPoint;
        public bool rotation = false;
        [SerializeField]private bool ParentSpawnPoint = false;
        public List<GameObject> SpawnedObject; //Spawn edilmiş objeler


        GameObject TempObj;
        //Objeleri spawn eder.
        public void SpawnIt()
        {
            if (gameObject.activeInHierarchy)
            {
                for (int i = 0; i < SpawnPoint.Length; i++)
                {
                
                        TempObj = PoolerCenter.Pooler.GetPooledObject(SpawnThisObject);

                    if (TempObj != null)
                    {
                        SpawnedObject.Add(TempObj);

                        TempObj.transform.position = SpawnPoint[i].position;
                        if (rotation)
                        {
                            TempObj.transform.rotation = SpawnPoint[i].rotation;
                        }
                        if (ParentSpawnPoint)
                        {
                            TempObj.transform.parent = SpawnPoint[i];
                        }
                        TempObj.SetActive(true);
                    }
                    
                }
            }

        }



        /// <summary>
        /// Spawn a random object from array
        /// </summary>
        public void SpawnRandomObject()
        {
            if (gameObject.activeInHierarchy)
            {
                for (int i = 0; i < SpawnPoint.Length; i++)
                {
                    int Rndm = Random.Range(0, SpawnRandomFromThisList.Length);

                    TempObj = PoolerCenter.Pooler.GetPooledObject(SpawnRandomFromThisList[Rndm]);

                    if (TempObj!=null)
                    {
                        SpawnedObject.Add(TempObj);

                        TempObj.transform.position = SpawnPoint[i].position;
                        if (rotation)
                        {
                            TempObj.transform.rotation = SpawnPoint[i].rotation;
                        }
                        if (ParentSpawnPoint)
                        {
                            TempObj.transform.parent = SpawnPoint[i];
                        }
                    }
                   
                }
            }

        }

        public void SpawnItInRandomPoint()
        {
            if (gameObject.activeInHierarchy)
            {
                int RandomInt = Random.Range(0,SpawnPoint.Length);
             
                    TempObj = PoolerCenter.Pooler.GetPooledObject(SpawnThisObject);

                    if (TempObj!=null)
                    {
                        SpawnedObject.Add(TempObj);

                        TempObj.transform.position = SpawnPoint[RandomInt].position;
                        if (rotation)
                        {
                            TempObj.transform.rotation = SpawnPoint[RandomInt].rotation;
                        }
                        if (ParentSpawnPoint)
                        {
                            TempObj.transform.parent = SpawnPoint[RandomInt];
                        }
                    TempObj.SetActive(true);
                    }
                    
                

            }


        }


        public void SpawnInPoint(Vector3 Point, Vector3 Normal)
        {
            if (gameObject.activeInHierarchy)
            {
               

                    TempObj = PoolerCenter.Pooler.GetPooledObject(SpawnThisObject);
                if (TempObj!=null)
                {
                    SpawnedObject.Add(TempObj);

                    TempObj.transform.position = Point;
                    if (rotation)
                    {
                        TempObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, Normal);
                    }
                    TempObj.SetActive(true);
                }
                   
                
            }

        }




        //Listede spawn edilmiş objeleri siler.
        public void RemoveThem()
        {

            for (int i = 0; i < SpawnedObject.Count; i++)
            {
                SpawnedObject[i].SetActive(false);


            }
            SpawnedObject.Clear();
        }
    }
}