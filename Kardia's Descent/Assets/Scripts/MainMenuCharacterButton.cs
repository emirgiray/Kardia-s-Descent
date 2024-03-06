using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuCharacterButton : MonoBehaviour
{
    public GameObject characterPrefab;
    public Sprite characterImage;
    public string characterName;
    public int index = -1;
    public bool isUnlocked;
    [SerializeField] private Button button;
    [SerializeField] private Image selfImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private bool equipped = false;

    private void Start()
    {
        button.interactable = isUnlocked;
        lockImage.gameObject.SetActive(!isUnlocked);
        selfImage.sprite = characterImage;
    }

    private void SelectCharacter()
    {
        MainMenuController.Instance.SpawnPlayerPreview(this);
        MainMenuController.Instance.selectButton.onClick.RemoveAllListeners();
        MainMenuController.Instance.selectButton.onClick.AddListener(equipped ? RemoveCharacter : EquipCharacter);
        MainMenuController.Instance.selectButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = equipped ? "Remove" : "Select";
    }

    public void EquipCharacter()
    {
        if (GameManager.Instance.AddToSelectedPlayers(characterPrefab))
        {
            equipped = true;
            index = GameManager.Instance.SelectedPlayers.Count - 1;
            MainMenuController.Instance.AddToSelected(this);
            UpdateButton(RemoveCharacter, "Remove");
        }
    }

    private void RemoveCharacter()
    {
        if (GameManager.Instance.RemoveFromSelectedPlayers(characterPrefab))
        {
            equipped = false;
            MainMenuController.Instance.RemoveFromSelected(this);
            index = -1;
            UpdateButton(EquipCharacter, "Select");
        }
    }
    
    private void UpdateButton(UnityEngine.Events.UnityAction action, string text)
    {
        MainMenuController.Instance.selectButton.onClick.RemoveAllListeners();
        MainMenuController.Instance.selectButton.onClick.AddListener(action);
        MainMenuController.Instance.selectButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ButtonClicked()
    {
        SelectCharacter();
    }
}