using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private bool loadOnAwake;
    
    public static SaveLoadSystem Instance { get; private set; }
    public string saveFileName = "saveFile";
    public bool encryptData;
    
    public AllPlayers allPlayers;
    public AllHeartDatas allHeartDatas;
    public SaveData saveData;
    
    // players
    // health 
    // items
    // hearts
    // total kills, start time etc
    
    private string saveFileExtension = ".json";
    [ReadOnly] [Multiline(2)]
    public string saveFilePath;
    [ReadOnly] [Multiline(2)]
    public string saveFileFullPath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        if (saveFileFullPath == null)
        {
            GenerateFileLocation();
        }
        
        if(loadOnAwake) LoadGame();
    }
    
    private void GetValues()
    {
        foreach (var player in LevelManager.Instance.players)
        {
            if (player.isUnlocked)
            {
                for (int i = 0; i < allPlayers.allPlayers.Count; i++)
                {
                    if (allPlayers.allPlayers[i].name.Equals(player.name + " Variant"))
                    {
                        saveData.playerDatas.Add(new PLayerData
                        {
                            playerID = i,
                            health = player.health._Health,
                            maxHealth = player.health.Max,
                            Strength = player.characterStats.Strength,
                            Dexterity = player.characterStats.Dexterity,
                            Constitution = player.characterStats.Constitution,
                            Aiming = player.characterStats.Aiming,
                            heartID = player.heartContainer.heartData != null ? player.heartContainer.heartData.heartIndex : -1
                            
                            /*playerID = i,
                            health = player.health._Health,
                            maxHealth = player.health.Max,
                            
                            heartID = player.heartContainer.heartData.heartIndex,
                            Strength = player.characterStats.Strength,
                            Dexterity = player.characterStats.Dexterity,
                            Constitution = player.characterStats.Constitution,
                            Aiming = player.characterStats.Aiming,*/
                        });
                    }
                }
            }
        }

        saveData.startTime = LevelManager.Instance.startTime.ToString();
        saveData.totalDamageDealt = LevelManager.Instance.totalDamageDealt;
        saveData.totalDamageTaken = LevelManager.Instance.totalDamageTaken;
        saveData.totalKills = LevelManager.Instance.totalKills;
        
    }
    
    [Button, GUIColor(1f, 0.1f, 1f)]
    public void SaveGame()
    {
        // clear the savedata list
        saveData.playerDatas.Clear();
        
        GetValues();
        
        if (!System.IO.Directory.Exists(saveFilePath))
        {
            System.IO.Directory.CreateDirectory(saveFilePath);
        }
    
        string json = JsonUtility.ToJson(saveData, true);

        if (encryptData)  json = Encrypt(json);

        System.IO.File.WriteAllText(saveFileFullPath, json);
    }

    [Button, GUIColor(0.1f, 1f, 1f)]
    public void LoadGame()
    {
        if (System.IO.File.Exists(saveFileFullPath))
        {
            string json = System.IO.File.ReadAllText(saveFileFullPath);

            if (encryptData) json = Decrypt(json);
            
            JsonUtility.FromJsonOverwrite(json, saveData);
            
            AssignValues();
        }
    }

    private void AssignValues()
    {
        MainPrefabScript.Instance.SelectedPlayers.Clear(); // clear the list if it has values
        
        for (int i = 0; i < saveData.playerDatas.Count; i++) // first tell the mainprefabscript to spawn the players
        { 
            MainPrefabScript.Instance.SelectedPlayers.Add(allPlayers.allPlayers[saveData.playerDatas[i].playerID]);
        }
        
        LevelManager.Instance.startTime = DateTime.Parse(saveData.startTime);
        LevelManager.Instance.totalDamageDealt = saveData.totalDamageDealt;
        LevelManager.Instance.totalDamageTaken = saveData.totalDamageTaken;
        LevelManager.Instance.totalKills = saveData.totalKills;
        
        MainPrefabScript.Instance.InitializeLevel();
        LevelManager.Instance.InitializeCharacters();

        for (int i = 0; i < saveData.playerDatas.Count; i++) // then assign the values to the players
        {
            Player player = MainPrefabScript.Instance.spawnedPlayerScripts[i];
            
            player.characterStats.Strength = saveData.playerDatas[i].Strength;
            player.characterStats.Dexterity = saveData.playerDatas[i].Dexterity;
            player.characterStats.Constitution = saveData.playerDatas[i].Constitution;
            player.characterStats.Aiming = saveData.playerDatas[i].Aiming;
            //player.AssignSkillValues(); // this is already done on character awake
            
            player.health.Max = saveData.playerDatas[i].maxHealth;
            player.health._Health = saveData.playerDatas[i].health;

            if (saveData.playerDatas[i].heartID != -1)
            {
                player.heartContainer.heartData = allHeartDatas.allHearts[saveData.playerDatas[i].heartID];
                player.heartContainer.hearthStatsApplied = true;
            }
        }
    }

    #region File Stuff

    private string Encrypt(string data)
    {
        char key = 'X'; // Key for XOR encryption
        string result = string.Empty;

        foreach (char c in data)
        {
            result += (char)(c ^ key);
        }

        return result;
    }

    private string Decrypt(string data)
    {
        return Encrypt(data); // XOR encryption is symmetric
    }

    [Button, GUIColor(1f, 0.1f, 0.1f)]
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFileFullPath))
        {
            File.Delete(saveFileFullPath);
        }
    }
    
    [Button, GUIColor(1f, 1f, 1f)]
    public void GenerateFileLocation()
    {
        saveFilePath = Application.persistentDataPath + "/Saves/";
        saveFileFullPath = saveFilePath + saveFileName + saveFileExtension;
    }
    
    [Button, GUIColor(1f, 1f, 1f)]
    public void OpenFileLocation()
    {
        if (File.Exists(saveFileFullPath))
        {
            string newPath = saveFileFullPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
            Process.Start("explorer.exe", "/select," + newPath);
        }
        else
        {
            Debug.Log("File does not exist: " + saveFileFullPath);
        }
    }
    

    #endregion

    [Button, GUIColor(1f, 1f, 1f)]
    public void LevelManagerAndMainPrefabInit()
    {
        MainPrefabScript.Instance.InitializeLevel();
        LevelManager.Instance.InitializeCharacters();
    }
    
}

// This save data is for an individual run, not for the entire game
[Serializable]
public class SaveData
{
    public List<PLayerData> playerDatas = new();
    public int totalKills;
    public string startTime;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalHeartsCollected;
    
    
    // add player stats, health, items, hearts etc
    // add current scene, all scene remainingSceneTypes types
}

[Serializable]
public class PLayerData
{
    public int playerID;
    public int health;
    public int maxHealth;
    
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Aiming;
    // public List<Item> items;
    public int heartID;
    
    
}
