using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace CodeMonkey.CameraSystem {

    public class CameraSystem : MonoBehaviour {

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] float zoomSpeed = 1;
        [SerializeField] float dragMoveSpeed = 0.5f;
        [SerializeField] float rotateSpeed = 50;
        [SerializeField] Vector2 rotateMinMax;
        [SerializeField] private bool useEdgeScrolling = false;
        [SerializeField] private bool useDragPan = true;
        
        [Tooltip("Best values (5, 15), (10, 20), (10, 25)")]
        [SerializeField] Vector2 followOffsetYMinMax = new Vector2(10f, 25);
        [SerializeField] float staticFollowRangey = 20;
        [Tooltip("Best values (10, 20), (15, 25), (15, 30)")]
        [SerializeField] Vector2 followOffsetMinMax = new Vector2(15, 30);
        [Tooltip("Best values (10, 30)")]
        [SerializeField] Vector2 fieldOfViewMinMax = new Vector2(10, 30);
        
        /*[SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float fieldOfViewMax = 50;*/

        private bool dragPanMoveActive;
        private Vector2 lastMousePosition;
        private float targetFieldOfView = 50;
        private Vector3 followOffset;

        private void OnEnable()
        {
            Interact.Instance.CharacterSelectedAction += OnCharacterSelected;
        }

        private void OnDisable()
        {
            Interact.Instance.CharacterSelectedAction -= OnCharacterSelected;
        }

        public void OnCharacterSelected(Tile characterTile)
        {
            Vector3 targetPosition = characterTile.transform.position;
            transform.DOMove(targetPosition, 0.5f).SetEase(Ease.Linear);


            followOffset.y =  staticFollowRangey;
                Mathf.Clamp(followOffset.y, followOffsetYMinMax.x, followOffsetYMinMax.y);
            
            //followOffset.y = Mathf.Clamp(followOffset.y, followOffsetYMinMax.x, followOffsetYMinMax.y);
            
            
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
        }

        private void Awake() {
            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Update() {
            HandleCameraMovement();

            if (useEdgeScrolling) {
                HandleCameraMovementEdgeScrolling();
            }

            if (useDragPan) {
                HandleCameraMovementDragPan();
            }

            HandleCameraRotation();

            //HandleCameraZoom_FieldOfView();
           // HandleCameraZoom_MoveForward();
            HandleCameraZoom_LowerY();
        }

        private void HandleCameraMovement() {
            Vector3 inputDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        private void HandleCameraMovementEdgeScrolling() {
            Vector3 inputDir = new Vector3(0, 0, 0);

            int edgeScrollSize = 20;

            if (Input.mousePosition.x < edgeScrollSize) {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.y < edgeScrollSize) {
                inputDir.z = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) {
                inputDir.x = +1f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) {
                inputDir.z = +1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            // float moveSpeed = 50f;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            
            // transform.position = Vector3.Lerp(transform.position, moveDir ,  moveSpeed * Time.deltaTime);
            
            //transform.position = Vector3.Lerp(transform.position, moveDir, Time.deltaTime * moveSpeed);

            // transform.DOMove(transform.position += moveDir * moveSpeed * Time.deltaTime, 0.5f).SetEase(Ease.InBack);

        }
        
        Vector3 prevDampingValues =  new Vector3();

        private void HandleCameraMovementDragPan() {
            Vector3 inputDir = new Vector3(0, 0, 0);

            if (Input.GetMouseButtonDown(2)) 
            { 
                prevDampingValues =  new Vector3(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping, cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping, cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping);
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0.1f;
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0.1f;
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0.1f;
                
                dragPanMoveActive = true;
                lastMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(2)) 
            {
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = prevDampingValues.x;
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = prevDampingValues.y;
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = prevDampingValues.z;
                
                dragPanMoveActive = false;
            }

            if (dragPanMoveActive) {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

                float dragPanSpeed = 1f;
                inputDir.x = mouseMovementDelta.x * -dragPanSpeed;
                inputDir.z = mouseMovementDelta.y * -dragPanSpeed;

                lastMousePosition = Input.mousePosition;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            
            transform.position += moveDir * dragMoveSpeed * Time.deltaTime;
        }

        private void HandleCameraRotation() {
            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotateDir = -1f;
            if (Input.GetKey(KeyCode.E)) rotateDir =  1f;
            //transform.eulerAngles += new Vector3(0,rotateDir * rotateSpeed * Time.deltaTime , 0);

            // Correct for deltaTime so your behaviour is framerate independent.
            // (You may need to increase your speed as it's now measured in degrees per second, not per frame)
            float angularIncrement = rotateSpeed * rotateDir * Time.deltaTime;

            // Get the current rotation angles.
            Vector3 eulerAngles = transform.localEulerAngles;

            // Returned angles are in the range 0...360. Map that back to -180...180 for convenience.
            if (eulerAngles.y > 180f)
                eulerAngles.y -= 360f;

            // Increment the pitch angle, respecting the clamped range.
            eulerAngles.y = Mathf.Clamp(eulerAngles.y - angularIncrement, rotateMinMax.x, rotateMinMax.y);

            // Orient to match the new angles.
            transform.localEulerAngles = eulerAngles;
        }

        private void HandleCameraZoom_FieldOfView() {
            if (Input.mouseScrollDelta.y > 0) {
                targetFieldOfView -= 5;
            }
            if (Input.mouseScrollDelta.y < 0) {
                targetFieldOfView += 5;
            }

            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMinMax.x, fieldOfViewMinMax.y);

            float zoomSpeed = 10f;
            cinemachineVirtualCamera.m_Lens.FieldOfView =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_MoveForward() {
            Vector3 zoomDir = followOffset.normalized;

            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0) {
                followOffset -= zoomDir * zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0) {
                followOffset += zoomDir * zoomAmount;
            }

            if (followOffset.magnitude < followOffsetMinMax.x) {
                followOffset = zoomDir * followOffsetMinMax.x;
            }

            if (followOffset.magnitude > followOffsetMinMax.y) {
                followOffset = zoomDir * followOffsetMinMax.y;
            }

            
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_LowerY() {
            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0) {
                followOffset.y -= zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0) {
                followOffset.y += zoomAmount;
            }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetYMinMax.x, followOffsetYMinMax.y);
            
            
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);

        }

    }

}