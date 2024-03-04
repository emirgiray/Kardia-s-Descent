using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Character Stats", order = 0)]

public class CharacterStats : ScriptableObject
{
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Aiming;
    
    [Space(30)]
    [SerializeField] private int DefaultStrength;
    [SerializeField] private int DefaultDexterity;
    [SerializeField] private int DefaultConstitution;
    [SerializeField] private int DefaultAiming;

    public void AddStrength(int value)
    {
        Strength += value;
    }
    public void AddDexterity(int value)
    {
        Dexterity += value;
    }
    public void AddConstitution(int value)
    {
        Constitution += value;
    }
    public void AddAiming(int value)
    {
        Aiming += value;
    }
    public void RemoveStrength(int value)
    {
        Strength -= value;
    }
    public void RemoveDexterity(int value)
    {
        Dexterity -= value;
    }
    public void RemoveConstitution(int value)
    {
        Constitution -= value;
    }
    public void RemoveAiming(int value)
    {
        Aiming -= value;
    }

    public void ResetValues()
    {
        Strength = DefaultStrength;
        Dexterity = DefaultDexterity;
        Constitution = DefaultConstitution;
        Aiming = DefaultAiming;
        
    }
   
}
