using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HeartData", menuName = "ScriptableObjects/Hearts", order = 0)]
public class HeartData : ScriptableObject
{
    [PreviewField(Height= 200,Alignment =ObjectFieldAlignment.Left)]
    public GameObject HeartModel;
    [Space][BoxGroup("Heart Info")] public string heartName = "";
    [BoxGroup("Heart Info")] public int heartIndex = 0;
    [Multiline] [BoxGroup("Heart Info")]public string heartDescription = "";
    
    public enum HeartRarity
    {
        Common, Uncommon, Rare, Epic, Legendary
    }

    [BoxGroup("Heart Info")]
    public HeartRarity heartRarity;
    
    [Multiline][ShowIf("heartRarity", HeartRarity.Legendary)]
    public string unlockCondition = "";
    
    [ShowIf("heartRarity", HeartRarity.Legendary)]
    public bool isPowerUnlocked = false;
}
