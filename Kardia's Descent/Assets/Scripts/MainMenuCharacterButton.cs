using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCharacterButton : MonoBehaviour
{
    public GameObject characterPrefab;
    public Sprite characterImage;
    public Sprite selectedSprite;
    public string characterName;
    public int index = -1;
    
    public bool isUnlcoked;
    public AllPlayers allPlayers;
    [SerializeField] private Button button;
    [SerializeField] private Image selfImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private bool selected = false;
    
    
    private void Start()
    {
        if (isUnlcoked)
        {
            button.interactable = true;
            lockImage.gameObject.SetActive(false);
        }
        else
        {
            button.interactable = false;
            lockImage.gameObject.SetActive(true);
        }
    
        selfImage.sprite = characterImage;
    }

    private void SelectCharacter()
    {
        if (GameManager.Instance.AddToSelectedPlayers(characterPrefab))
        {
            selected = true;
            index = GameManager.Instance.SelectedPlayers.Count - 1;
            MainMenuController.Instance.AddToSelected(this);
        }
    }
    private void RemoveCharacter()
    {
        if (GameManager.Instance.RemoveFromSelectedPlayers(characterPrefab))
        {
            selected = false;
            MainMenuController.Instance.RemoveFromSelected(this);
            index = -1;
        }
    }

    public void ButtonClicked()
    {
        if (selected)
        {
            RemoveCharacter();
        }
        else
        {
            SelectCharacter();
        }
    }
}
