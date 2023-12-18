using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public HeartData heartData;
    public bool isPowerUnlocked = false;
    public bool isEquipped = false;

    public void EquipHeart(HeartData heartDataIn)
    {
        isEquipped = true;
        this.heartData = heartDataIn;
        if (heartDataIn.isPowerUnlocked)
        {
            isPowerUnlocked = true;
        }
    }
}
