using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRoundCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image image;
    [SerializeField] private Image borderImage;
    [SerializeField] private Character character;

    [SerializeField] private Color playerColor;
    [SerializeField] private Color enemyColor;
    [SerializeField] private RoundInfo roundInfo;
    

    public void Init(Character characterIn, RoundInfo roundInfoIn)
    {
        character = characterIn;
        roundInfo = roundInfoIn;
        nameText.text = characterIn.name;
        //image.sprite = character.characterSprite;

        if (characterIn is Player)
        {
            borderImage.color = playerColor;
        }
        else
        {
            borderImage.color = enemyColor;
        }
        
    }

    public void Rearrange()
    {
        roundInfo.Rearrange();
    }
    
}
