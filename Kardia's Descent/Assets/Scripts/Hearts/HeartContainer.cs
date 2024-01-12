using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartContainer : MonoBehaviour
{
    public HeartData heartData;
    [SerializeField] private Character character;
    public bool isPowerUnlocked = false;
    public bool isEquipped = false;
    [SerializeField] private SkillButton heartButton;

    public void SetHeartButton(SkillButton heartButtonIn)
    {
        heartButton = heartButtonIn;
        SetTooltipInfo();
    }
    

    private void OnDisable()
    {
        if (heartData != null && heartData.heartRarity != HeartData.HeartRarity.Legendary)
        {
            character.characterStats.RemoveStrength(heartData.BonusStrength);
            character.characterStats.RemoveDexterity(heartData.BonusDexterity);
            character.characterStats.RemoveConstitution(heartData.BonusConstitution);
            character.characterStats.RemoveAiming(heartData.BonusAiming);
        }
    }

    public void PickUpHeart(HeartData heartDataIn)
    {
        character.inventory.heartsInInventory.Add(heartDataIn);
        
        if (character.heartContainer.heartData == null)
        {
            EquipHeart(heartDataIn);
        }

    }
    
    public void EquipHeart(HeartData heartDataIn)
    {
        isEquipped = true;
        this.heartData = heartDataIn;
        character.inventoryUI.SetHeartIcon(heartDataIn.HeartSprite);
        if (heartDataIn.isPowerUnlocked)
        {
            isPowerUnlocked = true;
        }

        if (heartDataIn.heartRarity != HeartData.HeartRarity.Legendary)
        {
            character.characterStats.AddStrength(heartDataIn.BonusStrength);
            character.characterStats.AddDexterity(heartDataIn.BonusDexterity);
            character.characterStats.AddConstitution(heartDataIn.BonusConstitution);
            character.characterStats.AddAiming(heartDataIn.BonusAiming);
            
            character.AssignSkillValues();
        }
        SetTooltipInfo();
    }
    
    public void UnequipHeart()
    {
        if (heartData.heartRarity != HeartData.HeartRarity.Legendary)
        {
            character.characterStats.RemoveStrength(heartData.BonusStrength);
            character.characterStats.RemoveDexterity(heartData.BonusDexterity);
            character.characterStats.RemoveConstitution(heartData.BonusConstitution);
            character.characterStats.RemoveAiming(heartData.BonusAiming);
            
            character.AssignSkillValues();
        }
        character.inventoryUI.SetHeartIcon(null);
        isEquipped = false;
        isPowerUnlocked = false;
        heartData = null;
    }

    public void SetTooltipInfo()
    {
        if (heartData != null)
        {
            heartButton.SetHeader(heartData.heartName);
            heartButton.SetContent(heartData.heartDescription);
            
            if (heartData.heartRarity != HeartData.HeartRarity.Legendary)
            {
                heartButton.AddToContent("");
                heartButton.AddToContent($"Extra Strength: {heartData.BonusStrength}");
                heartButton.AddToContent($"Extra Dexterity: {heartData.BonusDexterity}");
                heartButton.AddToContent($"Extra Constitution: {heartData.BonusConstitution}");
                heartButton.AddToContent($"Extra Aiming: {heartData.BonusAiming}");
            }
        }
        else
        {
            heartButton.SetHeader("Heart Slot");
            heartButton.SetContent("Empty");
        }
    }
}
