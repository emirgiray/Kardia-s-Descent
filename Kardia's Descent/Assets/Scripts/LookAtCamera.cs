using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    void Start()
    {
        cameraTransform = Interact.Instance.mainCam.transform;
    }


    void Update()
    {
        gameObject.transform.LookAt(cameraTransform);
    }
}
