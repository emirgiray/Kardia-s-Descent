using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuCharacterButton : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    public GameObject characterPrefab;
    public Sprite characterImage;
    public string characterName;
    public int index = -1;
    public bool isUnlocked;
    public bool equipped = false;
    public Button equipButton;
    public GameObject selectedImage;
    public GameObject equippedImage;
    [SerializeField] private Button button;
    [SerializeField] private Image selfImage;
    [SerializeField] private Image lockImage;
    
    private void Start()
    {
        button.interactable = isUnlocked;
        lockImage.gameObject.SetActive(!isUnlocked);
        selfImage.sprite = characterImage;
    }

    private void SelectCharacter()
    {
        MainMenuController.Instance.SpawnPlayerPreview(this);
        MainMenuController.Instance.selectedChar = this;
        
        MainMenuController.Instance.radarChart.SetStats(characterPrefab.GetComponent<Player>().characterStats);
        // MainMenuController.selectButton.onClick.RemoveAllListeners();
        // MainMenuController.selectButton.onClick.AddListener(equipped ? RemoveCharacter : EquipCharacter);
        // MainMenuController.selectButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = equipped ? "Remove" : "Select";
        equipButton.gameObject.SetActive(!equipped);
        selectedImage.SetActive(true);
    }

    public void EquipCharacter()
    {
        if (everythingUseful.GameManager.AddToSelectedPlayers(characterPrefab))
        {
            equipped = true;
            index = everythingUseful.GameManager.SelectedPlayers.Count - 1;
            MainMenuController.Instance.AddToSelected(this);
            UpdateButton(RemoveCharacter, "Remove");
            equippedImage.SetActive(true);
        }
    }

    public void RemoveCharacter()
    {
        if (everythingUseful.GameManager.RemoveFromSelectedPlayers(characterPrefab))
        {
            if (MainMenuController.Instance.spawnedCharacter.name == characterPrefab.name + "(Clone)")
            {
                UpdateButton(EquipCharacter, "Select");
            }
            equipped = false;
            MainMenuController.Instance.RemoveFromSelected(this);
            index = -1;
            MainMenuController.Instance.SelectedCharacterChanged();
            equippedImage.SetActive(false);
        }
    }
    
    public void UpdateButton(UnityEngine.Events.UnityAction action, string text)
    {
        // MainMenuController.selectButton.onClick.RemoveAllListeners();
        // MainMenuController.selectButton.onClick.AddListener(action);
        // MainMenuController.selectButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ButtonClicked()
    {
        SelectCharacter();
        MainMenuController.Instance.SelectedCharacterChanged();
    }
}