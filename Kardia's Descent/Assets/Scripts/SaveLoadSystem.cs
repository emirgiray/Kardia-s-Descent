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
    public static SaveLoadSystem Instance { get; private set; }
    public string saveFileName = "saveFile";
    public bool encryptData;
    
    public AllPlayers allPlayers;
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
    }
    
    private void GetValues()
    {
        foreach (var player in GameManager.Instance.players)
        {
            if (player.isUnlocked)
            {
                for (int i = 0; i < allPlayers.allPlayers.Count; i++)
                {
                    if (allPlayers.allPlayers[i].name.Equals(player.name + " Variant"))
                    {
                        saveData.playerIDs.Add(i);
                    }
                }
            }
        }

        saveData.startTime = GameManager.Instance.startTime.ToString();
        saveData.totalDamageDealt = GameManager.Instance.totalDamageDealt;
        saveData.totalDamageTaken = GameManager.Instance.totalDamageTaken;
        saveData.totalKills = GameManager.Instance.totalKills;
        
    }
    
    [Button, GUIColor(1f, 0.1f, 1f)]
    public void SaveGame()
    {
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
        MainPrefabScript.Instance.SelectedPlayers.Clear();
        
        for (int i = 0; i < saveData.playerIDs.Count; i++)
        { 
            MainPrefabScript.Instance.SelectedPlayers.Add(allPlayers.allPlayers[saveData.playerIDs[i]]);
        }
        
        GameManager.Instance.startTime = DateTime.Parse(saveData.startTime);
        GameManager.Instance.totalDamageDealt = saveData.totalDamageDealt;
        GameManager.Instance.totalDamageTaken = saveData.totalDamageTaken;
        GameManager.Instance.totalKills = saveData.totalKills;
        
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
}

[Serializable]
public class SaveData
{
    public List<int> playerIDs = new();
    public int totalKills;
    public string startTime;
    public int totalDamageDealt;
    public int totalDamageTaken;
    
    // add player stats, health, items, hearts etc
    // add current scene, all scene remainingSceneTypes types
}
