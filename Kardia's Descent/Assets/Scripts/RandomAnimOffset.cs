using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimOffset : MonoBehaviour
{
    private Animator animator;
    
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        if (animator != null)
        {
            animator.SetFloat("Offset", Random.Range(0f, 1f));
            animator.speed = Random.Range(0.8f, 1.2f);
        }
    }

   
}
