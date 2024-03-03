using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllPlayers", menuName = "ScriptableObjects/AllPlayers", order = 0)]
public class AllPlayers : ScriptableObject
{
    public List<GameObject> allPlayers = new();
}
