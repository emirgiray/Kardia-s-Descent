using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderVariables : MonoBehaviour
{
    public static PathfinderVariables Instance { get; private set; }
    
    public LayerMask tileMask;
    public LayerMask coverPointMask;
    public LayerMask tankCoverPointMask;

    public float coverPointRayLenght = 2.5f;
    public float characterYOffset = 0.5f;
    public float tileVerticalityLenght = 3f;
    [Tooltip("This is the offset of the hexagonal tiles, it is used to calculate the distance between tiles, default is 1.75f for hex scale (1, 1, 1) )")] 
    public float HEXAGONAL_OFFSET = 1.75f;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }
    
     
}
