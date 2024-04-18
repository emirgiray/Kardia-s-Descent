using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    // [SerializeField] private float Speed = 5;
    
    [SerializeField] private float lifeTime = 1;
    private float timer;    
    private float halfTimer;
    public ParabolaController parabolaController;
    
    public void SetAndStartParabola(Transform TargetTransform)
    {
        Transform lastChild = parabolaController.ParabolaRoot.transform.GetChild(parabolaController.ParabolaRoot.transform.childCount - 1); 
        Transform middleChild = parabolaController.ParabolaRoot.transform.GetChild(parabolaController.ParabolaRoot.transform.childCount - 2); 
        
        lastChild.position = TargetTransform.position;
        middleChild.position = (lastChild.position + parabolaController.ParabolaRoot.transform.GetChild(0).position) / 2;
        middleChild.position = new Vector3(middleChild.position.x, middleChild.position.y + 1, middleChild.position.z);
        // parabolaController.RefreshTransforms(Speed);
        parabolaController.FollowParabola();
    }

    void Start()
    {
        timer = lifeTime;
        halfTimer = lifeTime / 2;
    }
    
    /*void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }*/
}
