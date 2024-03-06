using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }
    
    public AllPlayers allPlayers;
    [BoxGroup("Lights")] [SerializeField]
    private List<GameObject> Lights = new();
    [BoxGroup("Lights")] [SerializeField]
    private float lightsDelay = 0.5f;

    [BoxGroup("Character")] [SerializeField]
    private Transform characterGOSpawnTransform;
    [BoxGroup("Character")] [SerializeField]
    private GameObject characterSelectionUI;
    [BoxGroup("Character")] [SerializeField]
    private GameObject characterButtonPrefab;
    [BoxGroup("Character")] [SerializeField]
    private Transform selectionLayoutTransform;
    [BoxGroup("Character")] [SerializeField]
    private Transform selectedLayoutTransform;
    [BoxGroup("Character")] [SerializeField]
    private List<GameObject> selectedList = new();
    [BoxGroup("Character")] [SerializeField]
    private Sprite defaultUnselectedSprite;
    
    [BoxGroup("Start")] [SerializeField]
    private Button startButton;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void LightsControl()
    {
        StartCoroutine(LightsControlDelay());
    }

    private IEnumerator LightsControlDelay()
    {
        foreach (var light in Lights)
        {
            light.SetActive(!light.activeSelf);
            yield return new WaitForSeconds(lightsDelay);
        }
        StartSelection();
    }
    
    public void StartSelection()
    {
        characterSelectionUI.SetActive(true);

        foreach (var player in allPlayers.allPlayers)
        {
            Player playerScript = player.GetComponent<Player>();
            
            var temp = Instantiate(characterButtonPrefab, selectionLayoutTransform);
            MainMenuCharacterButton charButton = temp.GetComponent<MainMenuCharacterButton>();
            charButton.characterPrefab = playerScript.playerPrefab;
            charButton.characterName = player.name;
            charButton.isUnlcoked = playerScript.isUnlocked;
            charButton.characterImage = playerScript.characterSprite;

        }
        
    }

    public void AddToSelected(MainMenuCharacterButton button)
    {
        selectedLayoutTransform.transform.GetChild(button.index).GetComponent<Image>().sprite = button.characterImage;
        selectedList.Add(button.gameObject);
        startButton.interactable = true;
    }
    
    public void RemoveFromSelected(MainMenuCharacterButton button)
    {
        selectedLayoutTransform.transform.GetChild(button.index).GetComponent<Image>().sprite = defaultUnselectedSprite;
        selectedList.Remove(button.gameObject);
        
        if (selectedList.Count == 0)
        {
            startButton.interactable = false;
        }
    }

    public void StartButtonPressed()
    {
        GameManager.Instance.StartRun();
    }
    
}
