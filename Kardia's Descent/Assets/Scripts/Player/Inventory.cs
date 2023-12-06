using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // public delegate void SelectSkill();
    // public SelectSkill selectSkillDelegate;

    [SerializeField] private Player player;
    
    
    public List<WeaponData> weaponsDataList = new List<WeaponData>();
    public List<SkillsData> skillsDataList = new List<SkillsData>();
    public List<SkillsData> skillsToAdd = new List<SkillsData>();
    public List<WeaponPartData> weaponPartDataList = new List<WeaponPartData>();
    [SerializeField] private WeaponContainer weaponContainer;
    [SerializeField] private SkillContainer skillsContainer;
    

    [SerializeField] private GameObject spawnedWeapon;
    [SerializeField] private Transform hand;
    
    [BoxGroup("UI")] [SerializeField] private GameObject InventoryUI;
    [BoxGroup("UI")] [SerializeField] private GameObject InventoryUISlot;

    [BoxGroup("UI")] [SerializeField] [ReadOnly] private GameObject SpawnedInventoryUI;
    [BoxGroup("UI")] [SerializeField] [ReadOnly] private InventoryUI SpawnedInventoryUIScript;
    //[BoxGroup("UI")] [SerializeField] private GameObject skillsUI;
    //[BoxGroup("UI")] [SerializeField] private GameObject horizontalLayoutGroup;
    [BoxGroup("UI")] [SerializeField] private GameObject skillButtonPrefab;
    [BoxGroup("UI")] [SerializeField] public List<GameObject> spawnedSkillButtonPrefabs = new List<GameObject>();
    
    [BoxGroup("DEBUG")] [SerializeField] private bool spawnTestWeapon = true;
    [BoxGroup("DEBUG")] [SerializeField] private WeaponData testWeaponData;
    
    SGT_Health sgtHealth;
    
    
     void Start()
    {
        sgtHealth = GetComponent<SGT_Health>();
        player = GetComponent<Player>();

        if (this.GetComponent<Character>() is Player)
        {
            SpawnInventoryUI();
            SpawnedInventoryUIScript.SetPlayer(player);
            SpawnedInventoryUIScript.SetPlayerPortrait(player.GetPlayerPortrait());
            PopulateHealthUI();
            PopulatePointsUI();
            // PopulateSkillsUI();
        
            SpawnedInventoryUIScript.SubscribeToPlayerEvents();
            ShowSkillsUI(false);
        }
        
        
        
        if(testWeaponData != null && spawnTestWeapon ) SpawnTestWeapon();
        
        
    }

     public void SpawnInventoryUI()
     {
         SpawnedInventoryUI = Instantiate(InventoryUI, InventoryUISlot.transform);
         SpawnedInventoryUIScript = SpawnedInventoryUI.GetComponent<InventoryUI>();
         
     }

     public void BeforePopulateSkillsUI()
     {
         for (int i = 0; i < skillsToAdd.Count; i++)
         {
             spawnedSkillButtonPrefabs.Add(null);
             skillsContainer.skillButtons.Add(null);
             spawnedSkillButtonPrefabs[i] = Instantiate(skillButtonPrefab, SpawnedInventoryUIScript.GetHorizontalLayoutGroup().transform);
             skillsContainer.skillButtons[i] = spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();
             //skillsContainer.skillsList[i].skillButton = spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();//bu onemli
             
             // var i1 = i;
             // //spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>().InitButton(skillsToAdd[i] , ()=> skillsContainer.TrySelectSkill(skillsToAdd[i1]), skillsContainer);
             //
             // skillsContainer.skillButtons.Add(null);
             // skillsContainer.skillButtons[i] = spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();
             //
             //
             // //spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>().SetSkillContainer(skillsContainer);
         }
     }
     
     public void PopulateSkillsUI()
     {
         for (int i = 0; i < skillsToAdd.Count; i++)
         {
             // spawnedSkillButtonPrefabs.Add(null);
             // spawnedSkillButtonPrefabs[i] = Instantiate(skillButtonPrefab, SpawnedInventoryUIScript.GetHorizontalLayoutGroup().transform);
             // skillsContainer.skillsList[i].skillButton = spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();//bu onemli
             
             var i1 = i;
             //spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>().InitButton(skillsToAdd[i] , ()=> skillsContainer.TrySelectSkill(skillsToAdd[i1]), skillsContainer);
             
             // skillsContainer.skillButtons.Add(null);
             skillsContainer.skillButtons[i] = spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>();

             
             //spawnedSkillButtonPrefabs[i].GetComponent<SkillButton>().SetSkillContainer(skillsContainer);
         }
         SpawnedInventoryUIScript.SetSkillButtons(skillsContainer.skillButtons);
     }
     
     

     public void PopulateHealthUI()
     {
         sgtHealth.Bar01 = SpawnedInventoryUIScript.GetBar01();
         sgtHealth.Bar02 = SpawnedInventoryUIScript.GetBar02();
     }

     public void PopulatePointsUI()
     {
         //SpawnedInventoryUIScript.GetActionText().text = player.remainingActionPoints.ToString();
         SpawnedInventoryUIScript.UpdateActionPoints(player.remainingActionPoints, "+");
     }
    public void AddWeapon(WeaponData weaponData)
    {
        weaponsDataList.Add(weaponData);
    }

    public void AddSkill(SkillsData skillsData)
    {
        skillsDataList.Add(skillsData);
    }

    
    public void SpawnWeapon(WeaponData weaponData/*, Transform hand*/)
    {
        spawnedWeapon = /*Instantiate(*/weaponData.WeaponPrefab/*, hand)*/;
        weaponContainer.AddWeapon(weaponData,spawnedWeapon.GetComponent<Weapon>());
        skillsContainer.AddSkills(weaponData.SkillsDataList);
        skillsDataList.AddRange(weaponData.SkillsDataList);
        skillsToAdd = weaponData.SkillsDataList;
        weaponsDataList.Add(weaponData);
        
        if (GetComponent<Character>() is Player)
        {
            PopulateSkillsUI();
        }
    }
        
    [Button]
    public void SpawnTestWeapon()
    {
        spawnedWeapon = /*Instantiate(*/testWeaponData.WeaponPrefab/*, hand)*/;
        weaponContainer.AddWeapon(testWeaponData,spawnedWeapon.GetComponent<Weapon>());
        skillsToAdd = testWeaponData.SkillsDataList;
        if (GetComponent<Character>() is Player)
        {
            BeforePopulateSkillsUI();
        }

        skillsContainer.AddSkills(testWeaponData.SkillsDataList);
        skillsDataList.AddRange(testWeaponData.SkillsDataList);
        weaponsDataList.Add(testWeaponData);
        if (GetComponent<Character>() is Player)
        {
            PopulateSkillsUI();
        }

    }

    public void ShowSkillsUI(bool value)
    {
        SpawnedInventoryUI.SetActive(value);
    }

    public InventoryUI GetSpawnedInventoryUIScript()
    {
        return SpawnedInventoryUIScript;
    }

}
