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
        image.sprite = character.characterSprite;

        if (character.health.Bar01_2 == null)
        {
            character.health.Bar01_2 = Bar01_2;
            // Debug.Log($"character bar {character.health.Bar01_2} bar01_2: {Bar01_2} ");
            character.health.Bar02_2 = Bar02_2;
            character.health.Init();

            if (characterIn is Player)
            {
                // borderImage.color = playerColor;
                Bar01_2.color = playerColor;
            }
            else
            {
                // borderImage.color = enemyColor;
                Bar01_2.color = enemyColor;
            }
        }
        else if (character.health.Bar01_3 == null)
        {
            character.health.Bar01_3 = Bar01_2;
            // Debug.Log($"character bar {character.health.Bar01_2} bar01_2: {Bar01_2} ");
            character.health.Bar02_3 = Bar02_2;
            character.health.Init();

            if (characterIn is Player)
            {
                // borderImage.color = playerColor;
                Bar01_2.color = playerColor;
            }
            else
            {
                // borderImage.color = enemyColor;
                Bar01_2.color = enemyColor;
            }
        }
        
    }

    public void Rearrange()
    {
        roundInfo.Rearrange(this.gameObject);
    }
    int click = 0;
    public void SelectCharacter()
    {
        click ++;
        StartCoroutine(Timer());
        
        
        if (click % 2 == 0)
        {
            character.Interact.CharacterSelectedAction?.Invoke(character.characterTile, 0.5f);

            if (character is Player)
            {
                character.Interact.TrySelectPlayer(character); 
            }
            
            return;
        }
        
        
        /*if (character is Player)
        {
            
            
            
        }

        if (character is Enemy)
        {
            if (click % 2 == 0)
            {
                Interact.Instance.CharacterSelectedAction?.Invoke(character.characterTile);
                return;
            }
        }*/
        
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        click = 0;
    }

    public Character GetCharacter()
    {
        return character;
    }

    
}
