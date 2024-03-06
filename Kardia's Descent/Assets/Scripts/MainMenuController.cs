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
    
    [BoxGroup("Character")] [SerializeField]
    private GameObject spawnedCharacter;
    [BoxGroup("Character")] [SerializeField]
    private List<GameObject> SpawnedSkillSprites = new();

    [BoxGroup("Skills")] [SerializeField]
    private Transform skillLayoutTransform;
    
    [BoxGroup("Buttons")] [SerializeField]
    private Button startButton;
    [BoxGroup("Buttons")] [SerializeField]
    public Button selectButton;
    [BoxGroup("Buttons")] [SerializeField]
    private Button newGameButton;
    [BoxGroup("Buttons")] [SerializeField]
    public Button continueButton;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private Vector3 lastMousePosition;
    private float rotationSpeed = 0f;
    [SerializeField] private float rotateMultiplier = 0.5f;
    [SerializeField] private float smoothnessDuration = 1f; // Duration over which to smooth the rotation

    private void Update()
    {
        RotateCharacterWithMouse();
    }

    private void RotateCharacterWithMouse()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            rotationSpeed = delta.x * rotateMultiplier;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(SmoothRotation());
        }

        characterGOSpawnTransform.Rotate(0, -rotationSpeed, 0, Space.World);
        lastMousePosition = Input.mousePosition;
    }

    private IEnumerator SmoothRotation()
    {
        float timeElapsed = 0;

        while (timeElapsed < smoothnessDuration)
        {
            rotationSpeed = Mathf.Lerp(rotationSpeed, 0, timeElapsed / smoothnessDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        rotationSpeed = 0;
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
            charButton.isUnlocked = playerScript.isUnlocked;
            charButton.characterImage = playerScript.characterSprite;

        }
        
        SpawnPlayerPreview(selectionLayoutTransform.GetChild(0).GetComponent<MainMenuCharacterButton>()); //spawn first character
        selectButton.onClick.AddListener(() =>  selectionLayoutTransform.GetChild(0).GetComponent<MainMenuCharacterButton>().EquipCharacter());
        
    }

    public void SpawnPlayerPreview(MainMenuCharacterButton button)
    {
        if (spawnedCharacter) Destroy(spawnedCharacter);
        spawnedCharacter = Instantiate(button.characterPrefab, characterGOSpawnTransform.position, characterGOSpawnTransform.rotation, characterGOSpawnTransform);

        Inventory inventory = spawnedCharacter.GetComponent<Inventory>();
        
        for (int i = 0; i < inventory.testWeaponData.SkillsDataList.Count; i++)
        {
            skillLayoutTransform.GetChild(i).GetComponent<SkillButton>().InitForMenuButton(inventory.testWeaponData.SkillsDataList[i]);
            
        }
        
        foreach (var component in spawnedCharacter.GetComponents<Component>())
        {
            if (!(component is Transform))
            {
                Destroy(component);
            }
        }
    }
    
    public void AddToSelected(MainMenuCharacterButton button)
    {
        selectedLayoutTransform.transform.GetChild(button.index).GetComponent<Image>().sprite = button.characterImage;
        selectedList.Add(button.gameObject);
        startButton.interactable = true;
        //SpawnPlayerPreview(button);
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

    public void InitNewGameAndContinue()
    {
        continueButton.interactable = SaveLoadSystem.Instance.GetDoesSaveExist();
    }
    
    public void NewGame()
    {
        if (SaveLoadSystem.Instance.GetDoesSaveExist())
        {
            SaveLoadSystem.Instance.DeleteSaveFile();
        }
        
        
    }

    public void Continue()
    {
        SaveLoadSystem.Instance.LoadGame();
        SceneChanger.Instance.ChangeScene(SaveLoadSystem.Instance.saveData.lastScene);
    }
}
