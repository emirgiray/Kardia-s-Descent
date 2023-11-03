using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGT_CollisionIgnore : MonoBehaviour
{

    public GameObject[] Source;
    public GameObject[] Target;
    public bool AwakeActive = true;

    private void Awake()
    {
        if (AwakeActive)
        {
            IgnoreCollision();
        }
        


    }




    public void IgnoreCollision()
    {

        for (int i = 0; i < Source.Length; i++)
        {

            Collider[] SourceChild = Source[i].GetComponentsInChildren<Collider>();

            for (int f = 0; f < SourceChild.Length; f++)
            {

                for (int c = 0; c < Target.Length; c++)
                {

                    Collider[] TargetChild = Target[c].GetComponentsInChildren<Collider>();


                    for (int b = 0; b < TargetChild.Length; b++)
                    {

                        Physics.IgnoreCollision(SourceChild[f], TargetChild[b]);
                    }


                }

                
            }

        }


    }

}
