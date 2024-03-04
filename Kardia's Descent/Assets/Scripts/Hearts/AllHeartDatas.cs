using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllHeartDatas", menuName = "ScriptableObjects/Hearts/All Heart Datas", order = 0)]
public class AllHeartDatas : ScriptableObject
{
    public List<HeartData> allHearts = new();
}
