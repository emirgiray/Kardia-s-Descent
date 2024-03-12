using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

// todo add a satart and awake functions to all levels
// todo save() should look at maybe another function to get the values other than level manager
public class SaveLoadSystem : MonoBehaviour
{
    public bool loadOnAwake;
   
    public static SaveLoadSystem Instance { get; private set; }
    public string saveFileName = "saveFile";
    public bool encryptData;
    
    public AllPlayers allPlayers;
    public AllHeartDatas allHeartDatas;
    public AllSceneTypes allSceneTypes;
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

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        if (saveFileFullPath == null)
        {
            GenerateFileLocation();
        }

        //InitAwake();
    }

    public void InitAwake()
    {
        if (loadOnAwake)
        {
            LoadGame();
        }

        AssignValues();
        SaveGame();
    }
    
    private void GetValues()
    {
        foreach (var player in MainPrefabScript.Instance.SelectedPlayers)
        {
            int playerIndex = allPlayers.allPlayers.FindIndex(p => p.name.Equals(player.name/* + " Variant"*/));
            Player playerScript = player.GetComponent<Player>();
          //  Debug.Log($"Player name: {player.name} Player index: {playerIndex}");
            if (playerIndex == -1) continue;

            playerScript.AssignSkillValues();
            saveData.playerDatas.Add(new PLayerData
            {
                playerID = playerIndex,
                health = playerScript.health._Health,
                maxHealth = playerScript.health.Max,
                isUnlocked = playerScript.isUnlocked,
                Strength = playerScript.characterStats.Strength,
                Dexterity = playerScript.characterStats.Dexterity,
                Constitution = playerScript.characterStats.Constitution,
                Aiming = playerScript.characterStats.Aiming,
                heartID = playerScript.heartContainer.heartData?.heartIndex ?? -1
            });
        }

        saveData.startTime = GameManager.Instance.startTime.ToString();
        saveData.totalDamageDealt = GameManager.Instance.totalDamageDealt;
        saveData.totalDamageTaken = GameManager.Instance.totalDamageTaken;
        saveData.totalKills = GameManager.Instance.totalKills;
        saveData.totalHeartsCollected = GameManager.Instance.totalHeartsCollected;

        saveData.remainingSceneTypes.Clear();
        foreach (var types in allSceneTypes.remainingSceneTypes)
        {
            saveData.remainingSceneTypes.Add(types.Scene.SceneName);
        }
        
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
            
            //AssignValues();
        }
    }

    private void AssignValues()
    {
      //  Debug.Log($"expression");
        MainPrefabScript.Instance.SelectedPlayers.Clear(); // clear the list if it has values
        
        for (int i = 0; i < saveData.playerDatas.Count; i++) // first tell the mainprefabscript to spawn the players
        { 
            MainPrefabScript.Instance.SelectedPlayers.Add(allPlayers.allPlayers[saveData.playerDatas[i].playerID]);
        }
        
        GameManager.Instance.startTime = DateTime.Parse(saveData.startTime);
        GameManager.Instance.totalDamageDealt = saveData.totalDamageDealt;
        GameManager.Instance.totalDamageTaken = saveData.totalDamageTaken;
        GameManager.Instance.totalKills = saveData.totalKills;
        GameManager.Instance.totalHeartsCollected = saveData.totalHeartsCollected;
        
        MainPrefabScript.Instance.InitializeLevel();
        LevelManager.Instance.InitializeCharacters();
        
        
        for (int i = 0; i < saveData.playerDatas.Count; i++) // then assign the values to the players
        {
            Player player = MainPrefabScript.Instance.spawnedPlayerScripts[i];
            
            player.characterStats.Strength = saveData.playerDatas[i].Strength;
            player.characterStats.Dexterity = saveData.playerDatas[i].Dexterity;
            player.characterStats.Constitution = saveData.playerDatas[i].Constitution;
            player.characterStats.Aiming = saveData.playerDatas[i].Aiming;
            player.isUnlocked = saveData.playerDatas[i].isUnlocked;
            //player.AssignSkillValues(); // this is already done on character awake
            
            player.health.Max = saveData.playerDatas[i].maxHealth;
            player.health._Health = saveData.playerDatas[i].health;

            if (saveData.playerDatas[i].heartID != -1)
            {
                player.heartContainer.heartData = allHeartDatas.allHearts[saveData.playerDatas[i].heartID];
                player.heartContainer.hearthStatsApplied = true;
            }
        }
        
        allSceneTypes.remainingSceneTypes.Clear();
        foreach (var type in saveData.remainingSceneTypes)
        {
            allSceneTypes.remainingSceneTypes.Add(allSceneTypes.defaultAllSceneTypes.Find(x => x.Scene.SceneName.Equals(type)));
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
            allSceneTypes.ResetToDefault();
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

    public bool GetDoesSaveExist()
    {
        return File.Exists(saveFileFullPath);
    }
}

// This save data is for an individual run, not for the entire game
[Serializable]
public class SaveData
{
    public List<PLayerData> playerDatas = new();
    public string startTime;
    public int totalKills;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalHeartsCollected;
    
    public string lastScene;
    // add player items
    // add  all scene remainingSceneTypes types
    public List<string> remainingSceneTypes = new();
}

[Serializable]
public class PLayerData
{
    public int playerID;
    public int health;
    public int maxHealth;
    public bool isUnlocked;
    
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Aiming;
    // public List<Item> items;
    public int heartID;
    
    
}
