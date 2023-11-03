
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Variables;
using TMPro;


namespace SGT_Tools.AI
{



    public class SGT_LevelSystem : MonoBehaviour
    {
        public delegate void LevelSystemClickAction();
        public static event LevelSystemClickAction OnClicked;

        public LevelSystemManagerClass[] _LevelSystemManager;

        private void Awake()
        {
            LoadLevel();
        }


        void OnEnable()
        {
            SGT_LevelSystem.OnClicked += CheckMoneyForAllButton;
            CheckMoneyForAllButton();
        }


        void OnDisable()
        {
            SGT_LevelSystem.OnClicked -= CheckMoneyForAllButton;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateText();
            CheckMoneyForAllButton();
        }

        public void UpdatePlayerLevelsOnStart()
        {
            LoadLevel();
            UpdateText();
            CheckMoneyForAllButton();
        }

        [Button]
        public void Test()
        {
           Debug.Log( "Sonuç "+IsEnoughMoney("Life"));
        }

        //return LevelName is enough money or not
        public bool IsEnoughMoney(string LevelName)
        {
            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (_LevelSystemManager[i].LevelName == LevelName)
                {
                    if (_LevelSystemManager[i].Level.Value <= _LevelSystemManager[i].MaxLevel)
                    {
                        if ( _LevelSystemManager[i].Money.Value<_LevelSystemManager[i].Price.Value[_LevelSystemManager[i].Level.Value])
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
             
                }
            }

            return false;
        }

        /// <summary>
        /// Increase level by level name
        /// </summary>
        /// <param name="LevelName"></param>
        [Button]
        public void IncreaseLevel(string LevelName)
        {
            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (_LevelSystemManager[i].LevelName == LevelName)
                {
                    if (_LevelSystemManager[i].Money.Value <
                        _LevelSystemManager[i].Price.Value[_LevelSystemManager[i].Level.Value])
                    {
                        _LevelSystemManager[i].NoMoney.Invoke();
                        return;
                    }

                        if (_LevelSystemManager[i].Level.Value <= _LevelSystemManager[i].MaxLevel)
                        {
                            _LevelSystemManager[i].Money
                                .DecreaseValue((int)_LevelSystemManager[i].Price.Value[_LevelSystemManager[i].Level.Value]);
                            _LevelSystemManager[i].Level.IncreaseValue(1);
                            SaveLevel();
                            _LevelSystemManager[i].LevelValueUpdateEvent.Invoke(_LevelSystemManager[i].Level.Value);
                            _LevelSystemManager[i]._CurrentLevel = _LevelSystemManager[i].Level.Value;
                            UpdateText();
                            CheckMoneyForAllButton();
                        }
                        else
                        {
                            _LevelSystemManager[i].ReachMaxSize.Invoke();
                        }

                    return;
                }
            }
        }

        /// <summary>
        /// This invoke when rewarded video is completed
        /// </summary>
        /// <param name="LevelName"></param>
        public void IncreaseLevelWithRewarded(string LevelName)
        {
            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (_LevelSystemManager[i].LevelName == LevelName)
                {
                    

                    if (_LevelSystemManager[i].Level.Value <= _LevelSystemManager[i].MaxLevel)
                    {
                        
                        _LevelSystemManager[i].Level.IncreaseValue(1);
                        SaveLevel();
                        _LevelSystemManager[i].LevelValueUpdateEvent.Invoke(_LevelSystemManager[i].Level.Value);
                        _LevelSystemManager[i]._CurrentLevel = _LevelSystemManager[i].Level.Value;
                        UpdateText();
                       
                    }
                    else
                    {
                        _LevelSystemManager[i].ReachMaxSize.Invoke();
                    }

                    return;
                }
            }
        }



        private void SaveLevel()
        {
            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                PlayerPrefs.SetInt(_LevelSystemManager[i].LevelName, _LevelSystemManager[i].Level.Value);
            }



        }




        public void LoadLevel()
        {

            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (PlayerPrefs.HasKey(_LevelSystemManager[i].LevelName))
                {
                    _LevelSystemManager[i].Level.Value = PlayerPrefs.GetInt(_LevelSystemManager[i].LevelName);
                }
            }






        }


        public void UpdateText()
        {

            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (_LevelSystemManager[i].LevelPrice != null)
                {
                    if (_LevelSystemManager[i].Level.Value <= _LevelSystemManager[i].MaxLevel)
                    {
                        _LevelSystemManager[i].LevelPrice.text = _LevelSystemManager[i].Price.Value[_LevelSystemManager[i].Level.Value].ToString();    
                    }
                    
                }

                if (_LevelSystemManager[i].CurrentLevelText != null)
                {
               
                    int NewValue = _LevelSystemManager[i].Level.Value + 1;
                    
                    _LevelSystemManager[i].CurrentLevelText.text = RemoveLeadingZeros(NewValue.ToString());
                }

            }



        }


        [Button]
        public void DeleteAllFilesAndResetLevel()
        {

            PlayerPrefs.DeleteAll();

            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                _LevelSystemManager[i].Level.Value = 0;
            }


        }

        [Button]
        public void CheckMoneyForAllButton()
        {
            for (int i = 0; i < _LevelSystemManager.Length; i++)
            {
                if (_LevelSystemManager[i].Level.Value <= _LevelSystemManager[i].MaxLevel)
                {
                    if (_LevelSystemManager[i].Money.Value < _LevelSystemManager[i].Price.Value[_LevelSystemManager[i].Level.Value])
                    {
                        _LevelSystemManager[i].NoMoney.Invoke();

                    }
                    else
                    {
                        _LevelSystemManager[i].HasMoney.Invoke();
                    }
                }
               
            }

        }

        
        public string RemoveLeadingZeros(string str)
        {
            return str.TrimStart('0');
        }

    }

    [System.Serializable]
    public class LevelSystemManagerClass
    {
        [BoxGroup("Level System Manager")]
        public string LevelName = "Level Name";
        [BoxGroup("Level System Manager")]
        public SGTIntVariables Level;
        [BoxGroup("Level System Manager")]
        public SGTIntVariables Money;
        [BoxGroup("Level System Manager")]
        public SGTFloatListVariables Price;
        [BoxGroup("Level System Manager")]
        public int MaxLevel;
        [ReadOnly]
        [BoxGroup("Level System Manager")]
        public int _CurrentLevel;

        [BoxGroup("Level System Manager")]
        public TextMeshProUGUI LevelPrice;
        [BoxGroup("Level System Manager")]
        public TextMeshProUGUI CurrentLevelText;
        [FoldoutGroup("Events")]
        public UnityEvent<int> LevelValueUpdateEvent;
        [FoldoutGroup("Events")]
        public UnityEvent NoMoney = new UnityEvent();
        [FoldoutGroup("Events")]
        public UnityEvent ReachMaxSize = new UnityEvent();
        [FoldoutGroup("Events")]
        public UnityEvent HasMoney = new UnityEvent();


    }

}
