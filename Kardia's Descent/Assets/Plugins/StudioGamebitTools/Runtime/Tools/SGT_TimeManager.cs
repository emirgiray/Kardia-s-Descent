
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.Tools
{
public class SGT_TimeManager : MonoBehaviour
{

    public int TargetFrameRate = 30;
    void Start()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = TargetFrameRate;
    }
}
}

