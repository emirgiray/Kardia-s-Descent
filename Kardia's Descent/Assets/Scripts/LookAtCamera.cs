using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    [SerializeField] private Transform cameraTransform;
    void Start()
    {
        cameraTransform = everythingUseful.Interact.mainCam.transform;
    }


    void Update()
    {
        gameObject.transform.LookAt(cameraTransform);
    }
}
