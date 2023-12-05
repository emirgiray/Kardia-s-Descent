using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LevelVariables : MonoBehaviour
{
    public LevelCharacters[] levelCharacters;
    // Start is called before the first frame update
    
    
    void Awake()
    {
        foreach (var character in levelCharacters)
        {
            if(character.weaponDataStart != null) character.Character.GetComponent<Inventory>().SpawnWeapon(character.weaponDataStart);
            if(character.characterSpriteStart != null) character.Character.characterSprite = character.characterSpriteStart;
            if(character.healthStart != 0) character.Character.GetComponent<SGT_Health>().Max = character.healthStart;
            if(character.actionPointsStart != 0) character.Character.actionPoints = character.actionPointsStart;
            if(character.maxActionPointsStart != 0) character.Character.maxActionPoints = character.maxActionPointsStart;
            if(character.initiativeStart != 0) character.Character.initiative = character.initiativeStart;
            
        }
    }
    
    [System.Serializable]
    public class LevelCharacters
    {
        [TabGroup("Characters")] public Character Character;
        [TabGroup("Characters")] public WeaponData weaponDataStart;
        [TabGroup("Characters")] public int healthStart;
        
        [TabGroup("Characters")] public Sprite characterSpriteStart;
        [TabGroup("Characters")] public int actionPointsStart;
        // [TabGroup("Characters")] public int remainingActionPoints;
        [TabGroup("Characters")] public int maxActionPointsStart;
        [TabGroup("Characters")] public int initiativeStart;//todo: this works in reverse!!!
        
       
        

    }
}
