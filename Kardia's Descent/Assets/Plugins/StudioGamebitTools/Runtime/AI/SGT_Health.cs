using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGT_Tools.Bridge;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.AI;
public class SGT_Health : MonoBehaviour {


    //          !!!!!Eğer Karakter disable ve enable yapıldıysa değerler sıfıra resetlenir !!!!!!!
     
    [Tooltip("Eğer karakter öldüyse true değeri alır")]
    [PropertyOrder(1)]
    public bool isDead;
    [BoxGroup("Current Health")]
    public int Max = 100;
    [BoxGroup("Current Health")]
    public int Min=0;
    public SGT_Diffuculty Diffuculty;
    [Tooltip("Eğer enable disableda resetlenmesini istemiyorsun OFF yap. ")]
    public bool ResetOnEnable = false;

    [Tooltip("Eğer startda maximum health atanmasını istemiyorsa OFF yap. ")]
    public bool ResetOnStart = true;

    [FoldoutGroup("AnimBars")]
    public Image Bar01;
    [FoldoutGroup("AnimBars")]
    public Image Bar02;
    
    [FoldoutGroup("AnimBars")]
    public Image Bar01_2;
    [FoldoutGroup("AnimBars")]
    public Image Bar02_2;

    [FoldoutGroup("AnimBars")]
    public float BarAnimSpeed = 10;
    [FoldoutGroup("AnimBars")]
    public float Bar02Delay = 0.4f;


    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public UnityEvent Dead;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public UnityEvent TakeDamage;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventInt TakeDamageWithValue = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventString TakeDamageWithString = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventInt HealthDecreaseValue = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventInt HealthAdded = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventString HealthAddedWithString = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventInt GetCurrentHealth = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventFloat GetCurrentHealthNormalized = null;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(2)]
    public SGTEventString MissEvent = null;


    //Eğer düşmanın health'i belirlenen limiti geçtiyse limit koymak için eklendi.
    [ShowInInspector]
    [BoxGroup("Current Health")]
    [ProgressBar("Min","Max", ColorGetter = "GetHealthBarColor")]
    [PropertyOrder(0)]
    public int _Health;

    [SerializeField] private SGT_HealthAnim _HealthAnim;
    [SerializeField] private SGT_HealthAnim _HealthAnim_2;

    public int HealthAi
    {
        get
        {
            return _Health;
        }

        set
        {
            if (value >= Max) { _Health = Max; }
            else if (value <= Min) { _Health = Min; /*isDead = true;*/ }
            else { _Health = value; }

        }

    }
    private void Start()
    {
        if (ResetOnStart)
        {
            HealthAi = Max;
        }
        
        Init();
        
    }

    public void Init()
    {
        if (Bar02!=null) //If added bar2, we will add another component to health script for animation
        {
            if (gameObject.GetComponent<SGT_HealthAnim>()==null)
            {
                _HealthAnim = gameObject.AddComponent<SGT_HealthAnim>();
            }
            if (_HealthAnim!=null)
            {
                _HealthAnim.Bar01 = Bar01;
                _HealthAnim.Bar02 = Bar02;
                _HealthAnim.BarSpeed = BarAnimSpeed;
                _HealthAnim.BarDelay = Bar02Delay;
            }
            
        }

        if (Bar01_2 != null) 
        {
            if (gameObject.transform.GetChild(0).GetComponent<SGT_HealthAnim>() == null)
            {
                _HealthAnim_2 = gameObject.transform.GetChild(0).gameObject.AddComponent<SGT_HealthAnim>();
            }
            if (_HealthAnim_2 != null)
            {
                _HealthAnim_2.Bar01 = Bar01_2;
                _HealthAnim_2.Bar02 = Bar02_2;
                _HealthAnim_2.BarSpeed = BarAnimSpeed;
                _HealthAnim_2.BarDelay = Bar02Delay;
            }

        }
    }

    private void OnEnable()
    {
        if (ResetOnEnable)
        {
            HealthAi = Max;
        }
       

        if (isDead)
        {
            isDead = false;
            //oneTime = false;
        }

    }

    //Bu fonksiyon dışarıdan health düşürmek için kullanılabilir.
    public void HealthDecrease(int value)
    {
        if (Diffuculty!=null)
        {
            int HealthBeforeDecrease = HealthAi;
            float TempValue = value * Diffuculty.DifficultyResult();
            HealthAi -= (int)TempValue;
            HealthDecreaseValue.Invoke(value);
            TakeDamage.Invoke();
            TakeDamageWithValue.Invoke(HealthAi);
            TakeDamageWithString.Invoke(value.ToString());
            GetCurrentHealth.Invoke(HealthAi);
            GetCurrentHealthNormalized.Invoke(SGT_Math.Remap(HealthAi, Min, Max,0,1));
            if (Bar01!=null)
            {
                Bar01.fillAmount = SGT_Math.Remap(HealthAi, Min, Max, 0, 1);
            }
            if (Bar02!=null)
            {
                if (_HealthAnim!=null)
                {
                    _HealthAnim.DelayBar();
                }
               
               
            }
            if (Bar01_2!=null)
            {
                Bar01_2.fillAmount = SGT_Math.Remap(HealthAi, Min, Max, 0, 1);
            }
            if (Bar02_2!=null)
            {
                if (_HealthAnim_2!=null)
                {
                    _HealthAnim_2.DelayBar();
                }
               
               
            }
            
            CheckDeadOrNot();
        }
        else
        {
            print("!!!!!Oyunun Zorluk seviyesinin atanması gerekiyor.!!! Naau_Diffuculty!!");
        }
       

    }

    //Bu fonksiyon dışardan health artırmak için kullanılabilir.
    public void HealthIncrease(int addHealth)
    {

       
        HealthAi += addHealth;
        HealthAdded.Invoke(addHealth);
        GetCurrentHealth.Invoke(HealthAi);
        HealthAddedWithString.Invoke(addHealth.ToString());
        GetCurrentHealthNormalized.Invoke(SGT_Math.Remap(HealthAi, Min, Max, 0, 1));
        CheckDeadOrNot();
    }



    //Yaşam seviyesi kontrol ediliyor.
    private void CheckDeadOrNot()
    {

        if (Max <= Min || HealthAi <= Min)
        {
            if (!isDead)
            {
                Dead.Invoke();
                isDead = true;
            }
            
            
        }
        else if (HealthAi > Min)
        {
            isDead = false;
           
        }


    }


    public void UpdateHealthBar()
    {
        if (Bar01!=null)
        {
            Bar01.fillAmount = SGT_Math.Remap(HealthAi, Min, Max, 0, 1);
        }
        if (Bar02!=null)
        {
            if (_HealthAnim!=null)
            {
                _HealthAnim.DelayBar();
            }
        }
        if (Bar01_2!=null)
        {
            Bar01_2.fillAmount = SGT_Math.Remap(HealthAi, Min, Max, 0, 1);
        }
        if (Bar02_2!=null)
        {
            if (_HealthAnim_2!=null)
            {
                _HealthAnim_2.DelayBar();
            }
        }
        
    }


    //Normalized değerlirini bildirir.
    public void UpdateNormalized()
    {
        GetCurrentHealthNormalized.Invoke(SGT_Math.Remap(HealthAi, Min, Max, 0, 1));

    }

    [PropertyOrder(-1)]
    [ButtonGroup("Settings")]
    public void Increase10Health()
    {
        HealthIncrease(10);
    }
    [PropertyOrder(-1)]
    [ButtonGroup("Settings")]
    public void Decrease10Health()
    {
        HealthDecrease(10);
    }
    [PropertyOrder(-1)]
    [ButtonGroup("Settings")]
    public void Kill()
    {
        HealthDecrease(Max);
    }

    private Color GetHealthBarColor(float Value)
    {

        return Color.Lerp(Color.red, Color.green, Mathf.Pow(Value / 100f, 2));
    }


    public void OnPrint(string Value)
    {
        print(Value);

    }

    public void OnPrintFloat(float Value)
    {
        print("Float Değeri : " + Value);

    }

    public void Miss()
    {
        MissEvent.Invoke("MISS");
    }
   
}
