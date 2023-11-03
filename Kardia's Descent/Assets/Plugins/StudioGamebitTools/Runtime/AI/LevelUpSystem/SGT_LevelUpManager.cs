using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Variables;
using TMPro;
using SGT_Tools.Bridge;
using System.Collections;

public class SGT_LevelUpManager : MonoBehaviour
{
    //This is the list of Sources. You can take a look at table for understand level numbers
    //This script not tested very well, It can be some problems on it. Test it before use it. We changed litle after use it in Create Monster.

    public List<int> LevelUpSystem = new List<int>();

    [FoldoutGroup("Values only for Copy")]
    public List<int> Attack = new List<int>() { 25, 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425, 450, 475, 500, 525, 550, 575, 600, 625, 650, 675, 700, 725, 750, 775, 800, 825, 850, 875, 900, 925, 950, 975, 1000, 1025, 1050, 1075, 1100, 1125, 1150, 1175, 1200, 1225, 1250, 1275, 1300, 1325, 1350, 1375, 1400, 1425, 1450, 1475, 1500, 1525, 1550, 1575, 1600, 1625, 1650, 1675, 1700, 1725, 1750, 1775, 1800, 1825, 1850, 1875, 1900, 1925, 1950, 1975, 2000, 2025, 2050, 2075, 2100, 2125, 2150, 2175, 2200, 2225, 2250, 2275, 2300, 2325, 2350, 2375, 2400, 2425, 2450, 2475, 2500, 2525 };
    [FoldoutGroup("Values only for Copy")]
    public List<int> Health = new List<int>() { 100, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000, 12500, 13000, 13500, 14000, 14500, 15000, 15500, 16000, 16500, 17000, 17500, 18000, 18500, 19000, 19500, 20000, 20500, 21000, 21500, 22000, 22500, 23000, 23500, 24000, 24500, 25000, 25500, 26000, 26500, 27000, 27500, 28000, 28500, 29000, 29500, 30000, 30500, 31000, 31500, 32000, 32500, 33000, 33500, 34000, 34500, 35000, 35500, 36000, 36500, 37000, 37500, 38000, 38500, 39000, 39500, 40000, 40500, 41000, 41500, 42000, 42500, 43000, 43500, 44000, 44500, 45000, 45500, 46000, 46500, 47000, 47500, 48000, 48500, 49000, 49500, 50000 };
    [FoldoutGroup("Values only for Copy")]
    public List<int> Defend = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 205, 210, 215, 220, 225, 230, 235, 240, 245, 250, 255, 260, 265, 270, 275, 280, 285, 290, 295, 300, 305, 310, 315, 320, 325, 330, 335, 340, 345, 350, 355, 360, 365, 370, 375, 380, 385, 390, 395, 400, 405, 410, 415, 420, 425, 430, 435, 440, 445, 450, 455, 460, 465, 470, 475, 480, 485, 490, 495, 500, 505 };
    [Tooltip("Set Level from Inspector for manuel situation like for enemies. You can select level from inspector and It will be level set in start")]
    public int SetLevelFromHere = 0;
    [Tooltip("Current Value Name, You should write a value name as Attack, Defend, Health something like it")]
    public string CurrentSaveName = "";
    [Tooltip("If It is thick true, then It will save this values.")]
    public bool _SaveCurrentXP = false;
    [ReadOnly]
    public int CurrentLevel;
    [ReadOnly]
    public int _CurrentValue;
    [Tooltip("Assign LevelUpManager here for price, You can create another LevelUpManager and assign level price or only same value for all levels,this also take special level price")]
    public SGT_LevelUpManager _PriceOfValue;
    public SGTIntVariables Money;
    [Tooltip("Add a number here for how much number needed for increase")]
    public int IncreaseNumber;
    [FoldoutGroup("Change Values")]
    [Tooltip("Add health system If you need power up system on health system. Example In Create Monster!")]
    public SGT_Health _health;
    [FoldoutGroup("Change Values")]
    public SGTIntVariables NumberValue;
    [FoldoutGroup("Change Values")]
    [Tooltip("If you want to change Weapon power or increase weapon value, You need to assign these weapons here If not It will skip")]
    public List<SGT_Weapon> Weapon = new List<SGT_Weapon>();
    [FoldoutGroup("Outputs")]
    [Tooltip("Show progress bar between min and max level numbers")]
    public Image Bar;
    [FoldoutGroup("Outputs")]
    public TextMeshProUGUI[] LevelNames;
    [FoldoutGroup("Outputs")]
    public TextMeshProUGUI _CurrenValueText;
    [FoldoutGroup("Outputs")]
    public TextMeshProUGUI _CurrenValueNextLevelText;
    [FoldoutGroup("Outputs")]
    public TextMeshProUGUI _CurrentPriceText;
    //public ScoreAnim _ScoreAnim;

    [FoldoutGroup("Events")]
    public UnityEvent<int> AddedXP;
    [FoldoutGroup("Events")]
    public UnityEvent<int> RemoveFromXP;
    [FoldoutGroup("Events")]
    public UnityEvent<int> UpdatedLevel;
    [FoldoutGroup("Events")]
    public UnityEvent NoEnoughMoney;
    [FoldoutGroup("Events")]
    public UnityEvent NoEnoughMoneyInStart;
    [FoldoutGroup("Events")]
    public UnityEvent HaveEnoughMoney;
    [FoldoutGroup("Events")]
    public UnityEvent SuccessForNextLevel;

    private int TempLevelForCheckSuccess;


    private void Awake()
    {
        LoadCurrentXP();
    }

    private void OnEnable()
    {
        if (Money != null)
        {
            if (Money.Value > _PriceOfValue.LevelUpSystem[CurrentLevel])
            {
                HaveEnoughMoney.Invoke();
                
            }
        }
    }






    public void Start()
    {
        if (SetLevelFromHere != 0)
        {
            _CurrentValue = ReturnLevelMin(CurrentSaveName, SetLevelFromHere);
        }

        CheckCurrentValueAndUpdateLevel(_CurrentValue, CurrentSaveName);
            if (_CurrentValue==0)
            {
                _CurrentValue = ReturnLevelMax(CurrentSaveName,CurrentLevel);
            }
            


        if (_health != null) //Set Health
        {
         
                    _health.Max = _CurrentValue;
                    _health.HealthAi = _CurrentValue;
                
           
            }
        if (NumberValue != null)
        {
            NumberValue.Value = _CurrentValue;
        }

            for (int i = 0; i < Weapon.Count; i++)
            {
                Weapon[i].Attack = _CurrentValue;
            }



        for (int i = 0; i < LevelNames.Length; i++)
        {
            LevelNames[i].text = CurrentLevel.ToString();
        }

        if (Bar != null)
        {
            if (CurrentLevel == 0)
            {
                Bar.fillAmount = SGT_Math.Remap(_CurrentValue, ReturnLevelMin(CurrentSaveName, CurrentLevel), ReturnLevelMin(CurrentSaveName, CurrentLevel+1 ), 0, 1);
            }
            else
            {
                Bar.fillAmount = SGT_Math.Remap(_CurrentValue, ReturnLevelMin(CurrentSaveName, CurrentLevel), ReturnLevelMax(CurrentSaveName, CurrentLevel), 0, 1);
            }

        }

        if (_CurrenValueText != null)
        {
            _CurrenValueText.text = _CurrentValue.ToString();
        }

        if (_CurrenValueNextLevelText != null)
        {
            _CurrenValueNextLevelText.text = ReturnLevelMax(CurrentSaveName, CurrentLevel).ToString();
        }

            if (_CurrentPriceText!=null)
            {
                _CurrentPriceText.text = _PriceOfValue.LevelUpSystem[CurrentLevel].ToString();
            }
            

        if (Money != null)
        {
            if (Money.Value < _PriceOfValue.LevelUpSystem[CurrentLevel])
            {
                NoEnoughMoneyInStart.Invoke();
                return;
            }
        }

        
    }


   
    public void IncreaseMore()
    {
        if (Money!=null)
        {
            if (Money.Value<_PriceOfValue.LevelUpSystem[CurrentLevel])
            {
                NoEnoughMoney.Invoke();
                return;
            }

           
        }

        if (Money != null)
        {
            if (Money.Value > _PriceOfValue.LevelUpSystem[CurrentLevel])
            {
                HaveEnoughMoney.Invoke();

            }
        }

        TempLevelForCheckSuccess = CurrentLevel;

        AddXP(IncreaseNumber, CurrentSaveName);
        if (_health!=null) //Set Health
        {
            _health.Max = _CurrentValue;
            _health.HealthAi= _CurrentValue;
            }
        if (NumberValue!=null)
        {
            NumberValue.Value = _CurrentValue;
        }

            for (int i = 0; i < Weapon.Count; i++)
            {
                Weapon[i].Attack = _CurrentValue;
            }



        for (int i = 0; i < LevelNames.Length; i++)
        {
            LevelNames[i].text = CurrentLevel.ToString();
        }

        if (_CurrenValueText!=null)
        {
            _CurrenValueText.text = _CurrentValue.ToString();
        }


        if (Bar != null)
        {
            if (CurrentLevel == 0)
            {
                Bar.fillAmount = SGT_Math.Remap(_CurrentValue, ReturnLevelMin(CurrentSaveName, CurrentLevel), ReturnLevelMin(CurrentSaveName, CurrentLevel + 1), 0, 1);
            }
            else
            {
                Bar.fillAmount = SGT_Math.Remap(_CurrentValue, ReturnLevelMin(CurrentSaveName, CurrentLevel), ReturnLevelMax(CurrentSaveName, CurrentLevel), 0, 1);
            }

        }

        if (_CurrenValueNextLevelText!=null)
        {
            _CurrenValueNextLevelText.text = ReturnLevelMax(CurrentSaveName, CurrentLevel).ToString();
        }

        if (TempLevelForCheckSuccess<CurrentLevel)
        {
            SuccessForNextLevel.Invoke();
            
        }

        StartCoroutine(CheckHaveEnougMoneyDelay());
      
    }


    IEnumerator CheckHaveEnougMoneyDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (Money != null)
        {
            if (Money.Value < _PriceOfValue.LevelUpSystem[CurrentLevel])
            {
                NoEnoughMoney.Invoke();

            }


        }

    }





    private void SaveCurrentXP()
    {
        if (_SaveCurrentXP)
        {
            PlayerPrefs.SetInt(CurrentSaveName, _CurrentValue);
        }


    }

    private void LoadCurrentXP()
    {
        if (_SaveCurrentXP)
        {
            if (PlayerPrefs.HasKey(CurrentSaveName))
            {
                _CurrentValue = PlayerPrefs.GetInt(CurrentSaveName);
            }
        }



    }




   
    private void AddXP(int Value, string LevelUpName)
    {

        _CurrentValue += Value;
        CheckCurrentValueAndUpdateLevel(_CurrentValue, LevelUpName);
        AddedXP.Invoke(_CurrentValue);
        SaveCurrentXP();
    }

   
    private void RemoveXP(int Value, string LevelUpName)
    {

        _CurrentValue -= Value;
        CheckCurrentValueAndUpdateLevel(_CurrentValue, LevelUpName);
        RemoveFromXP.Invoke(_CurrentValue);
        SaveCurrentXP();
    }



    private void CheckCurrentValueAndUpdateLevel(int Value,string LevelUpName)
    {

        CurrentLevel = ReturnLevelFromCurrentValue(Value, LevelUpName);
        UpdatedLevel.Invoke(CurrentLevel);


    }



    int _CurrentLevelFinder;
    /// <summary>
    /// When you sent XP, It return to Level Result
    /// </summary>
    /// <param name="XP"></param>
    /// <returns></returns>
    private int ReturnLevelFromCurrentValue(int XP,string LevelUpName)
    {
        List<int> List = LevelUpSystem;
        //LevelUpSystemList.TryGetValue(LevelUpName, out List);

        for (int i = 0; i < List.Count; i++)
        {
            if (XP < List[i])
            {
                if (i != 0)
                {
                    _CurrentLevelFinder = i - 1;
                }

                break;
            }

        }
        return _CurrentLevelFinder;
    }


    private int ReturnLevelMin(string Value, int Level)
    {
        List<int> List = LevelUpSystem;
        //LevelUpSystemList.TryGetValue(Value, out List);
        if (Level == 0)
        {
            return 0;
        }
        return List[Level];
    }



    private int ReturnLevelMax(string Value, int Level)
    {
        List<int> List = LevelUpSystem;
        //LevelUpSystemList.TryGetValue(Value, out List);
            if (Level==0)
            {
                return List[Level];
            }
        return List[Level + 1];
    }




}

