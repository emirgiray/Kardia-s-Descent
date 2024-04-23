using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllPlayers", menuName = "ScriptableObjects/AllPlayers", order = 0)]
public class AllPlayers : ScriptableObject
{
    public List<Players> allPlayers = new();
}

[Serializable]
public class Players
{
    public GameObject playerPrefab;
    public int playerID;
    public bool isUnlocked = false;
}
