using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SGT_Tools.Tools
{


public class SGT_Instantiate : MonoBehaviour
{

        public GameObject SpawnThisObject;
        public GameObject[] SpawnRandomFromThisList;
        public Transform[] SpawnPoint;
        public bool rotation = false;
        [SerializeField] private bool ParentSpawnPoint = false;
        public List<GameObject> SpawnedObject; //Spawn edilmiş objeler


    GameObject TempObj;
    //Objeleri spawn eder.
    public void SpawnIt()
    {
            if (gameObject.activeInHierarchy)
            {
                 for (int i = 0; i < SpawnPoint.Length; i++)
                        {
                            TempObj = Instantiate(SpawnThisObject);
            
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

        /// <summary>
        /// Spawn a random object from array
        /// </summary>
        public void SpawnRandomObject()
        {
            if (gameObject.activeInHierarchy)
            {
                for (int i = 0; i < SpawnPoint.Length; i++)
                {
                    int Rndm = Random.Range(0,SpawnRandomFromThisList.Length);

                    TempObj = Instantiate(SpawnRandomFromThisList[Rndm]);

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


        public void SpawnItInRandomPoint()
        {
            if (gameObject.activeInHierarchy)
            {
                int RandomInt = Random.Range(0, SpawnPoint.Length);
                TempObj = Instantiate(SpawnThisObject);

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




        //Listede spawn edilmiş objeleri siler.
        public void RemoveThem()
    {

        for (int i = 0; i < SpawnedObject.Count; i++)
        {
            Destroy(SpawnedObject[i]);


        }
        SpawnedObject.Clear();
    }
 }
}