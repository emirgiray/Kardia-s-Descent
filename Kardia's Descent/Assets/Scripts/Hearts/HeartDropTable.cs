using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeartDropTable", menuName = "ScriptableObjects/Hearts/HeartDropTable", order = 0)]
public class HeartDropTable : ScriptableObject
{
    public AllHeartDatas allHeartDatas;
    public GameObject interactablePrefab;
   
    public List<HeartRarityAndDatas> HeartRarityAndDatas = new();
    public List<HeartRarityAndChance> HeartRarityAndChances = new();

    public void SpawnRandomHeart(Vector3 dropPosition)
    {
        int random = UnityEngine.Random.Range(0, 100);
        HeartData.HeartRarity rarity = HeartData.HeartRarity.Common;
        foreach (var heartRarityAndChance in HeartRarityAndChances)
        {
            if (random <= heartRarityAndChance.chance)
            {
                rarity = heartRarityAndChance.heartRarity;
            }
        }
        var name = "";
        var selectedList = HeartRarityAndDatas.Find(x => x.heartRarity == rarity).hearts;
        switch (rarity)
        {
            case HeartData.HeartRarity.Common:
                selectedList = HeartRarityAndDatas.Find(x => x.heartRarity == rarity).hearts;
                name = "Common";
                break;
            case HeartData.HeartRarity.Rare:
                selectedList = HeartRarityAndDatas.Find(x => x.heartRarity == rarity).hearts;
                name = "Rare";
                break;
            case HeartData.HeartRarity.Legendary:
                selectedList = HeartRarityAndDatas.Find(x => x.heartRarity == rarity).hearts;
                name = "Legendary";
                break;
        }
        
        var spawnedInteractable = Instantiate(interactablePrefab, dropPosition, Quaternion.identity);
        spawnedInteractable.name = $"{name} Heart Interactable";
        var interactable = spawnedInteractable.GetComponent<Interactable>();
        interactable.heart = selectedList[UnityEngine.Random.Range(0, selectedList.Count)];
    }

    public void SpawnSelectedHeart(Vector3 dropPosition, HeartData heartData)
    {
        
        var spawnedInteractable = Instantiate(interactablePrefab, dropPosition, Quaternion.identity);
        spawnedInteractable.name = $"{heartData.heartName} Heart Interactable";
        var interactable = spawnedInteractable.GetComponent<Interactable>();
        interactable.heart = heartData;
    }
  
    
}

[Serializable]
public class HeartRarityAndDatas
{
    public HeartData.HeartRarity heartRarity;
    public List<HeartData> hearts;
}

[Serializable]
public class HeartRarityAndChance
{
    public HeartData.HeartRarity heartRarity;
    public float chance;
}
