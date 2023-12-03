using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [BoxGroup("Move")] [SerializeField] private float speed = 10;
    [BoxGroup("Move")] [SerializeField] private float speedMultipliar = 2;
    
    [BoxGroup("Zoom")] [SerializeField] private Vector2 zoomMinMax = new Vector2(30, 110);
    
    [BoxGroup("Up & Down")] [SerializeField] private float upDownSpeed = 1;
    [BoxGroup("Up & Down")] [SerializeField] private float upDownDuration = 0.5f;
    [BoxGroup("Up & Down")] [SerializeField] private Ease upDownEase = Ease.Linear;
    
    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        float multipliar = 1;

        if (Input.GetKey(KeyCode.LeftShift))
            multipliar = speedMultipliar;
        
        Vector3 input = InputValues(out int yRotation).normalized;
        
        // cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + input.y * 2, zoomMinMax.x, zoomMinMax.y);
        // transform.parent.DOMoveY(transform.parent.position.y + input.y * speed * 30  * Time.deltaTime, 0.0f);
        
        transform.parent.Translate(input.Flat() * speed  * multipliar * Time.deltaTime);
        //transform.parent.Rotate(Vector3.up * yRotation * Time.deltaTime * speed * 4);

        //up and down //todo add collider or raycst check to prevent going through ground
        transform.parent.DOLocalMoveY( transform.parent.position.y + yRotation * upDownSpeed * multipliar, upDownDuration).SetEase(upDownEase);
        
        //rotate the object when middle mouse button is clicked and dragged using dotween
        if (Input.GetMouseButton(2))
        {
            Vector3 rotateAmount = new Vector3(-Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
            transform.parent.DORotate(transform.parent.rotation.eulerAngles + rotateAmount, 0.1f);
            
        }
        /*if (Input.GetMouseButton(2))
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * 2;
            float rotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * 2;
            transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
        }*/
    }

    private Vector3 InputValues(out int y)
    {
        //Move and zoom
        Vector3 values = new Vector3();
        values.x = Input.GetAxis("Horizontal");
        values.z = Input.GetAxis("Vertical");
        values.y = -Input.GetAxis("Mouse ScrollWheel");

        //Rotation
        y = 0;
        if (Input.GetKey(KeyCode.Q))
            y = -1;
        else if (Input.GetKey(KeyCode.E))
            y = 1;

        return values;
    }
}
