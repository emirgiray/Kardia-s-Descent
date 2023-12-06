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
    
    [SerializeField] private Image Bar01_2;
    [SerializeField] private Image Bar02_2;

    public void Init(Character characterIn, RoundInfo roundInfoIn)
    {
        character = characterIn;
        roundInfo = roundInfoIn;
        nameText.text = characterIn.name;
        
        character.health.Bar01_2 = Bar01_2;
       // Debug.Log($"character bar {character.health.Bar01_2} bar01_2: {Bar01_2} ");
        character.health.Bar02_2 = Bar02_2;
        image.sprite = character.characterSprite;

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
        roundInfo.Rearrange(this.gameObject);
    }

    public void SelectCharacter()
    {
        if (character is Player)
        {
            Interact.Instance.characterSelected = true;
            Interact.Instance.selectedCharacterSkillContainer = Interact.Instance.selectedCharacter.GetComponent<SkillContainer>();
            
            Interact.Instance.selectedCharacter = character; //select character
            Interact.Instance.lastSelectedCharacter = Interact.Instance.selectedCharacter;
            if (Interact.Instance.selectedCharacter.characterState == Character.CharacterState.WaitingTurn || Interact.Instance.selectedCharacter.characterState == Character.CharacterState.Idle)
            {
                if (Interact.Instance.selectedCharacter.remainingActionPoints > 0)
                {
                    Interact.Instance. EnableMovement(true);
                }
                Interact.Instance.HighlightReachableTiles(); //highlight tiles within range
            }
            Interact.Instance.selectedCharacter.GetComponent<Inventory>().ShowSkillsUI(true); //show skills ui
        }
        
    }
    
}
