using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "SGT/Weapons/Diffuculty")]
public class SGT_Diffuculty : ScriptableObject
{
    //1  Easy
    //2 Normal
    //3 Hard
    //4 Experimental

    public int GameDifficulty = 2;
    [Header("Select Diffuculty Level")]
    [SerializeField] float Easy = 1.5f;
    [SerializeField] float Normal = 1;
    [SerializeField] float Hard = 0.5f;
    public bool ExperimentalMode;
    [Multiline]
    [SerializeField] string DeveloperDescription;

    public float DifficultyResult()
    {

        if (GameDifficulty == 1)
        {
            return Easy;
        }

        if (GameDifficulty == 2)
        {
            return Normal;
        }

        if (GameDifficulty == 3)
        {
            return Hard;
        }
        else
        {
            return 0;
        }

        
    }



    /// <summary>
    /// Dışardan değeri set edebilmek için kullanabilirsin.
    /// </summary>
    /// <param name="Value"></param>
    public void SetDifficulty(int Value)
    {

        GameDifficulty = Value;

    }


}
