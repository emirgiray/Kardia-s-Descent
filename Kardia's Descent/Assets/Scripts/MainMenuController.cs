using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    public static MainMenuController Instance { get; private set; }
    
    public AllPlayers allPlayers;
    [BoxGroup("Lights")] [SerializeField]
    private List<GameObject> Lights = new();
    [BoxGroup("Lights")] [SerializeField]
    private float lightsDelay = 0.2f;

    [BoxGroup("Character")] [SerializeField]
    private Transform characterGOSpawnTransform;
    [BoxGroup("Character")] [SerializeField]
    private GameObject characterSelectionUI;
    [BoxGroup("Character")] [SerializeField]
    private GameObject characterButtonPrefab;
    [BoxGroup("Character")] [SerializeField]
    public List<MainMenuCharacterButton> allButtons = new();
    [BoxGroup("Character")] [SerializeField]
    public MainMenuCharacterButton selectedChar;
    [BoxGroup("Character")] [SerializeField]
    private Transform selectionLayoutTransform;
    [BoxGroup("Character")] [SerializeField]
    private Transform selectedLayoutTransform;
    [BoxGroup("Character")] [SerializeField]
    private List<GameObject> selectedList = new();
    [BoxGroup("Character")] [SerializeField]
    private Sprite defaultUnselectedSprite;
    
    [BoxGroup("Character")] [SerializeField]
    public GameObject spawnedCharacter;
    [BoxGroup("Character")] [SerializeField]
    private List<GameObject> SpawnedSkillSprites = new();

    [BoxGroup("Skills")] [SerializeField]
    private Transform skillLayoutTransform;
    [BoxGroup("Skills")][SerializeField] 
    public RadarChart radarChart;

    
    [BoxGroup("Buttons")] [SerializeField]
    private Button startButton;
    [BoxGroup("Buttons")] [SerializeField] [HideIf("onMainMenu")]
    private Button ExitButton;
    /*[BoxGroup("Buttons")] [SerializeField]
    public Button selectButton;*/
    [BoxGroup("Buttons")] [SerializeField]
    private Button newGameButton;
    [BoxGroup("Buttons")] [SerializeField]
    public Button continueButton;

    [SerializeField] private bool onMainMenu => everythingUseful.SceneChanger.isOnMainMenu;
    [HideIf("onMainMenu")]
    [SerializeField] private GameObject newCam ;
    private Transform cameraStartTransform ;
    
    private Quaternion spawnFirstTransform;
    
    private Vector3 lastMousePosition;
    private float rotationSpeed = 0f;
    [BoxGroup("Rotate")] [SerializeField] 
    private bool rotateCharacter = true;
    [BoxGroup("Rotate")] [SerializeField] 
    private float rotateMultiplier = 0.5f;
    [BoxGroup("Rotate")] [SerializeField] 
    private float smoothnessDuration = 1f; // Duration over which to smooth the rotation
    
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private GameObject table;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private Transform tableStartTransform;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private Transform tableEndTransform;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private float tableMoveFrontDuration = 1f;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private float tableMoveBackDuration = 1f;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    private Ease tableEase = Ease.InOutSine;
    bool goingBack = false;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    public UnityEvent TableMovedFrontEvent;
    [BoxGroup("Table")] [SerializeField] [HideIf("onMainMenu")]
    public UnityEvent TableMovedBackEvent;

    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private GameObject heartBG;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private ScrollRect heartScrollRect;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private Transform heartLayoutTransform;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private SkillButton equippedHeartButton;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private GameObject heartButtonPrefab;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private Sprite emptyHeartImage;
    [BoxGroup("Hearts")] [SerializeField] [HideIf("onMainMenu")]
    private List<PlayerAndHeart> playersAndHearts = new();
    private List<GameObject> spawnedHeartButtons = new();
    
    [FoldoutGroup("Events")]
    public UnityEvent SelectionStartedEvent;
    [FoldoutGroup("Events")]
    public UnityEvent SelectionEndedEvent;
    [FoldoutGroup("Events")] [HideIf("onMainMenu")]
    public UnityEvent HeartsSpawnedEvent;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        spawnFirstTransform = characterGOSpawnTransform.rotation;
    }
    private void Update()
    {
        RotateCharacterWithMouse();
    }

    private void RotateCharacterWithMouse()
    {
        if (!rotateCharacter) return;
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
    private float heartBgStartX;
    [Button, GUIColor(1f, 1f, 1f)]
    public void StartSelection()
    {
        
        SelectionStartedEvent.Invoke();
        characterSelectionUI.SetActive(true);
        startButton.interactable = false;
        List<string> previousSelectedCharNames = new();
        if (!onMainMenu)
        {
            everythingUseful.Interact.StopAllLogic();
            cameraStartTransform = everythingUseful.Interact.cameraTransform;
            everythingUseful.CineMachineCutRest(newCam, true);
            /*everythingUseful.Interact.MoveCameraAction?.Invoke(cameraTargetTransform, 1f);
            everythingUseful.Interact.ZoomCameraAction?.Invoke( -10f, 1);*/

            previousSelectedCharNames.Clear();
            foreach (var player in everythingUseful.MainPrefabScript.SelectedPlayers)
            {
                previousSelectedCharNames.Add(player.name);
            }
            
            everythingUseful.MainPrefabScript.ClearPlayers();
        }
        
        //Debug.Log($"allPlayers.allPlayers.Count: {allPlayers.allPlayers.Count}");
        foreach (var player in allPlayers.allPlayers)
        {
            Player playerScript = player.playerPrefab.GetComponent<Player>();
            
            var temp = Instantiate(characterButtonPrefab, selectionLayoutTransform);
            MainMenuCharacterButton charButton = temp.GetComponent<MainMenuCharacterButton>();
            allButtons.Add(charButton);
            charButton.characterPrefab = playerScript.playerPrefab;
            charButton.characterName = player.playerPrefab.name;
            charButton.characterImage = playerScript.characterSprite;
            charButton.isUnlocked = /*playerScript.isUnlocked*/ /*everythingUseful.AllPlayers.allPlayers[player.playerID].isUnlocked*/ everythingUseful.SaveLoadSystem.metaSaveData.UnlockableCharacterDatas[player.playerID].isUnlocked;
          
        }
        
        MainMenuCharacterButton firstButton = selectionLayoutTransform.GetChild(0).GetComponent<MainMenuCharacterButton>();
        if (onMainMenu)
        {
            firstButton.equipButton.gameObject.SetActive(true);
        }
        else
        {
            var firstChar = everythingUseful.MainPrefabScript.SelectedPlayers[0];
            for (int i = 0; i < selectionLayoutTransform.childCount; i++)
            {
                if (selectionLayoutTransform.GetChild(i).GetComponent<MainMenuCharacterButton>().characterPrefab.name == firstChar.name)
                {
                    firstButton = selectionLayoutTransform.GetChild(i).GetComponent<MainMenuCharacterButton>();
                    break;
                }
            }
        }
        SpawnPlayerPreview(firstButton); //spawn first character
        radarChart.SetStats(firstButton.characterPrefab.GetComponent<Player>().characterStats);
        firstButton.selectedImage.SetActive(true);
        selectedChar = firstButton;
        if (!onMainMenu)
        {
            heartBgStartX = heartBG.transform.localPosition.x;
            table.transform.position = tableStartTransform.position;
            //table.transform.rotation = tableStartTransform.rotation;
            table.SetActive(true);

            foreach (var playerName in previousSelectedCharNames)
            {
                foreach (var button in allButtons)
                {
                    if (button.characterPrefab.name == playerName)
                    {
                        button.EquipCharacter();
                    }
                }
            }
            

            SetHeartButtons();
            
        }
    }

  
    
    Tween tableTween;

    public void SpawnPlayerPreview(MainMenuCharacterButton button)
    {
        if (!onMainMenu && tableTween != null && tableTween.active)
        {
            tableTween.Pause();

            if (goingBack)
            {
                tableTween = table.transform.DOMove(tableEndTransform.position, tableMoveFrontDuration).SetEase(tableEase);
                goingBack = false;
                TableMovedFrontEvent.Invoke();
            }
            else
            {
                tableTween = table.transform.DOMove(tableStartTransform.position, tableMoveBackDuration).SetEase(tableEase);
                goingBack = true;
                TableMovedBackEvent.Invoke();
            }
            
           
        }
        if (spawnedCharacter != null && spawnedCharacter.name == button.characterPrefab.name + "(Clone)") return;
        
        if (spawnedCharacter)
        {
            if (onMainMenu)
            {
                Destroy(spawnedCharacter);
            }
            else
            {
                goingBack = true;
                tableTween = table.transform.DOMove(tableStartTransform.position, tableMoveBackDuration).SetEase(tableEase).OnComplete(
                    () =>
                    {
                        goingBack = false;
                        Destroy(spawnedCharacter);
                        tableTween = table.transform.DOMove(tableEndTransform.position, tableMoveFrontDuration).SetEase(tableEase);
                        spawnedCharacter = Instantiate(button.characterPrefab, characterGOSpawnTransform.position, characterGOSpawnTransform.rotation, characterGOSpawnTransform);
                        
                        SpawnedCharacterMiscFunc();
                        spawnedCharacter.GetComponent<Player>().animator.SetTrigger("Sleep");
                    });
            }
           
        }
        else // first time
        {
            if (!onMainMenu)
            {
                goingBack = false;
                tableTween = table.transform.DOMove(tableEndTransform.position, tableMoveFrontDuration).SetEase(tableEase);
                spawnedCharacter = Instantiate(button.characterPrefab, characterGOSpawnTransform.position, characterGOSpawnTransform.rotation, characterGOSpawnTransform);
                
                SpawnedCharacterMiscFunc();
                spawnedCharacter.GetComponent<Player>().animator.SetTrigger("Sleep");
                
            }
        }
        characterGOSpawnTransform.rotation = spawnFirstTransform;
        
        if (onMainMenu)
        {
            spawnedCharacter = Instantiate(button.characterPrefab, characterGOSpawnTransform.position, characterGOSpawnTransform.rotation, characterGOSpawnTransform);
            
            SpawnedCharacterMiscFunc();
        }
       
        
    }

    private void SpawnedCharacterMiscFunc()
    {
        Inventory inventory = spawnedCharacter.GetComponent<Inventory>();
        spawnedCharacter.GetComponent<Player>().findTileAtAwake = false;
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
        selectedLayoutTransform.transform.GetChild(button.index).GetChild(0).gameObject.SetActive(true);
        selectedLayoutTransform.transform.GetChild(button.index).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        selectedLayoutTransform.transform.GetChild(button.index).GetChild(0).GetComponent<Button>().onClick.AddListener(()=> button.RemoveCharacter());
        selectedList.Add(button.gameObject);
        startButton.interactable = true;
        //SpawnPlayerPreview(button);
    }
    
    public void RemoveFromSelected(MainMenuCharacterButton button)
    {
        selectedLayoutTransform.transform.GetChild(button.index).GetComponent<Image>().sprite = defaultUnselectedSprite;
        selectedList.Remove(button.gameObject);
        selectedLayoutTransform.transform.GetChild(button.index).GetChild(0).gameObject.SetActive(false);
        
        if (selectedList.Count == 0)
        {
            startButton.interactable = false;
        }
    }

    public void ExitButtonPressed()
    {
        everythingUseful.Interact.ContinueAllLogic();
        SelectionStartedEvent.Invoke();
        characterSelectionUI.SetActive(false);
        if (!onMainMenu)
        {
            //everythingUseful.Interact.MoveCameraAction?.Invoke(cameraStartTransform, 1f);
            everythingUseful.CineMachineCutRest(newCam, false);
        }
        
        everythingUseful.MainPrefabScript.SelectedPlayers = everythingUseful.GameManager.SelectedPlayers;
        
        
        foreach (var player in everythingUseful.GameManager.SelectedPlayers)
        {
            everythingUseful.LevelManager.AddPlayerToGame(player.GetComponent<Player>());
        }
        
        if (spawnedCharacter) Destroy(spawnedCharacter);
        
        everythingUseful.MainPrefabScript.InitializeLevel(/*charTiles*/ );
        
        for (int i = 0; i < everythingUseful.MainPrefabScript.spawnedPlayers.Count; i++) // then assign the values to the players
        {
            Player player = everythingUseful.MainPrefabScript.spawnedPlayerScripts[i];

            foreach (var savedPlayer in everythingUseful.SaveLoadSystem.inGameSaveData.inGamePLayerDatas)
            {
                int playerIndex = allPlayers.allPlayers.FindIndex(p => p.playerPrefab.name.Equals(player.name));
                if (playerIndex == savedPlayer.playerID)
                {
                    player.characterStats.Strength = savedPlayer.Strength;
                    player.characterStats.Dexterity = savedPlayer.Dexterity;
                    player.characterStats.Constitution = savedPlayer.Constitution;
                    player.characterStats.Aiming = savedPlayer.Aiming;
                    // player.isUnlocked = inGameSaveData.inGamePLayerDatas[i].isUnlocked;
                    player.isUnlocked = true;
                    
                    player.health.Max =  savedPlayer.maxHealth;
                    player.health._Health =  savedPlayer.health;

                    if (!onMainMenu)
                    {
                        if (playersAndHearts.Exists(p => p.playerID == playerIndex))
                        {
                            savedPlayer.equippedeartID = playersAndHearts.Find(p => p.playerID == playerIndex).heartID;
                        }
                    }
                    
                    if (savedPlayer.equippedeartID != -1)
                    {
                        player.heartContainer.heartData = everythingUseful.SaveLoadSystem.allHeartDatas.allHearts[savedPlayer.equippedeartID];
                        player.heartContainer.hearthStatsApplied = true;
                    }
            
                    player.inventory.heartsInInventory.Clear();
                    foreach (var heartIndex in savedPlayer.heartsInInventory)
                    {
                        player.inventory.heartsInInventory.Add(everythingUseful.SaveLoadSystem.allHeartDatas.allHearts[heartIndex]);
                    }
                }
                
            }
            
        }
        everythingUseful.LevelManager.InitializeCharacters();
        //everythingUseful.SaveLoadSystem.SaveGame();
        //everythingUseful.SaveLoadSystem.AssignValues();
      
        ClearPrevious();
    }

    private void ClearPrevious()
    {
        for (int i = 0; i < 4; i++)
        {
            selectedLayoutTransform.transform.GetChild(i).GetComponent<Image>().sprite = defaultUnselectedSprite;
            selectedLayoutTransform.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
     
        foreach (var button in allButtons)
        {
            Destroy(button.gameObject);
           
        }
        allButtons.Clear();

        foreach (var heartButton in spawnedHeartButtons)
        {
            Destroy(heartButton);
        }
        spawnedHeartButtons.Clear();
        
        selectedList.Clear();
    }

    private IEnumerator ClearPreviousEnum()
    {
        yield return new WaitForEndOfFrame();
        
    }

    public void StartButtonPressed()
    {
        everythingUseful.GameManager.StartRun();
    }

    public void InitNewGameAndContinue()
    {
        continueButton.interactable = everythingUseful.SaveLoadSystem.GetDoesSaveExist();
    }
    
    public void NewGame()
    {
        if (everythingUseful.SaveLoadSystem.GetDoesSaveExist())
        {
            everythingUseful.SaveLoadSystem.DeleteSaveFile();
        }
        
        
    }

    public void Continue()
    {
        everythingUseful.SaveLoadSystem.LoadGame();
        everythingUseful.SceneChanger.ChangeScene(everythingUseful.SaveLoadSystem.inGameSaveData.lastScene);
    }
    
    public void SelectedCharacterChanged()
    {
        foreach (var button in allButtons)
        { 
            button.equipButton.gameObject.SetActive(button == selectedChar && !button.equipped);
            button.selectedImage.SetActive(button == selectedChar);
        }
        
        SetHeartButtons();
    }

    bool heartsActivated = false;
    private void SetHeartButtons()
    {
        foreach (var heartButton in spawnedHeartButtons)
        {
            Destroy(heartButton);
        }
        spawnedHeartButtons.Clear();
        
       
        var player = selectedChar.characterPrefab;
        foreach (var savedPlayer in everythingUseful.SaveLoadSystem.inGameSaveData.inGamePLayerDatas)
        {
            int playerIndex = allPlayers.allPlayers.FindIndex(p => p.playerPrefab.name.Equals(player.name));
            if (playerIndex == savedPlayer.playerID)
            {
                
                if (savedPlayer.equippedeartID != -1)
                {
                    equippedHeartButton.InitForMenuButtonHeart(everythingUseful.SaveLoadSystem.allHeartDatas.allHearts[savedPlayer.equippedeartID]);
                    if (!heartsActivated)
                    {
                        heartBG.transform.DOLocalMoveX(-436, .25f ).SetEase(Ease.InOutQuad).SetRelative();
                        heartsActivated = true;
                    }

                }
                else
                {
                    equippedHeartButton.InitForMenuButtonHeart(null);
                    equippedHeartButton.SkillImage.sprite = emptyHeartImage;
                }
                    
                foreach (var heartIndex in savedPlayer.heartsInInventory)
                {
                    var heart = everythingUseful.SaveLoadSystem.allHeartDatas.allHearts[heartIndex];
                    GameObject temp = Instantiate(heartButtonPrefab, heartLayoutTransform);
                    temp.transform.localScale = Vector3.one;
                    spawnedHeartButtons.Add(temp);
                        
                    var skillButton = temp.GetComponent<SkillButton>();
                    skillButton.InitForMenuButtonHeart(heart);
                    skillButton.button.onClick.AddListener(() => EquipHeart(heart, playerIndex, heartIndex));
                        
                    var uiButton = temp.GetComponent<GenericUIButton>();
                    uiButton.SetHoverScale(Vector3.one, new Vector3(1.1f, 1.1f, 1.1f));
                }
                break;
            }
            else
            {
                if (heartsActivated)
                {
                    heartBG.transform.DOLocalMoveX(436, .5f ).SetEase(Ease.InOutQuad).SetRelative();
                    heartsActivated = false;
                }
                equippedHeartButton.InitForMenuButtonHeart(null);
                equippedHeartButton.SkillImage.sprite = emptyHeartImage;
            }
        }
        
    }
    Tween punchTween;
    private void EquipHeart(HeartData heartData, int PlayerID , int heartIndex)
    {
        equippedHeartButton.InitForMenuButtonHeart(heartData);
        punchTween?.Rewind();
        punchTween?.Kill();
        punchTween = equippedHeartButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 10, 1);
        if (playersAndHearts.Exists(p => p.playerID == PlayerID))
        {
            playersAndHearts.Find(p => p.playerID == PlayerID).heartID = heartIndex;
        }
        else
        {
            playersAndHearts.Add(new PlayerAndHeart() {playerID = PlayerID, heartID = heartIndex});
        }
        
    }
}
[Serializable]
public class PlayerAndHeart
{
    public int playerID;
    public int heartID = -1;
}
