using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HeartData", menuName = "ScriptableObjects/Hearts/HeartData", order = 0)]
public class HeartData : ScriptableObject
{
    [PreviewField(Height = 200, Alignment = ObjectFieldAlignment.Left)]
    public Sprite HeartSprite;
    [Space][BoxGroup("Heart Info")] public string heartName = "";
    [BoxGroup("Heart Info")] public int heartIndex = 0;
    [Multiline] [BoxGroup("Heart Info")]public string heartDescription = "";
    
    public enum HeartRarity
    {
        Common, Rare, Legendary
    }

    [BoxGroup("Heart Info")]
    public HeartRarity heartRarity;
    
    [HideIf("heartRarity", HeartRarity.Legendary)]
    public int BonusStrength;
    [HideIf("heartRarity", HeartRarity.Legendary)]
    public int BonusDexterity;
    [HideIf("heartRarity", HeartRarity.Legendary)]
    public int BonusConstitution;
    [HideIf("heartRarity", HeartRarity.Legendary)]
    public int BonusAiming;
    
    [Multiline][ShowIf("heartRarity", HeartRarity.Legendary)]
    public string unlockCondition = "";
    
    [ShowIf("heartRarity", HeartRarity.Legendary)]
    public bool isPowerUnlocked = false;
}
