using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CharacterFreeRoamAnim : VersionedMonoBehaviour
{
    [SerializeField] private Animator anim;
    
    [SerializeField] private GameObject endOfPathEffect;

    bool isAtDestination;

    IAstarAI ai;
    Transform tr;

    protected override void Awake () {
        base.Awake();
        ai = GetComponent<IAstarAI>();
        tr = GetComponent<Transform>();
    }

    /// <summary>Point for the last spawn of <see cref="endOfPathEffect"/></summary>
    protected Vector3 lastTarget;


    void OnTargetReached () {
        if (endOfPathEffect != null && Vector3.Distance(tr.position, lastTarget) > 1) {
            GameObject.Instantiate(endOfPathEffect, tr.position, tr.rotation);
            lastTarget = tr.position;
        }
        
        anim.SetBool("Walk", false);
    }

    protected void Update () 
    {
        if (ai.reachedEndOfPath) 
        {
            if (!isAtDestination)
            {
                OnTargetReached();
            }
            isAtDestination = true;
        }
        else
        {
            isAtDestination = false;
            anim.SetBool("Walk", true);
        }

        
    }
}

