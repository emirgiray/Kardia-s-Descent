using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Character Stats", order = 0)]

public class CharacterStats : ScriptableObject
{
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Aiming;
}
