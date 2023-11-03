
using UnityEngine;
using System.Collections;


[CreateAssetMenu(menuName = "SGT/Weapons/Weapon")]
public class SGT_Weapon : ScriptableObject
{
    [Tooltip("Buraya bu silahın kaç güçte vurabileceğini yazacağız")]
    public int Attack;
    [Multiline]
    public string DeveloperDescription;




}