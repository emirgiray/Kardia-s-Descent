using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class UIManager : MonoBehaviour
{

    [SerializeField] private EverythingUseful everythingUseful;
    [BoxGroup("Turn And Round")] [SerializeField]
    private int turn = 1;
    [BoxGroup("Turn And Round")] [SerializeField]
    private int round = 1;
    [BoxGroup("Turn And Round")] [SerializeField]
    private GameObject turnAndRoundGO;
    [BoxGroup("Turn And Round")] [SerializeField]
    private TextMeshProUGUI turnText;
    [BoxGroup("Turn And Round")] [SerializeField]
    private TextMeshProUGUI roundText;

    [BoxGroup("Pause Menu")] [SerializeField]
    private GameObject PauseScreenGO;
    
    [BoxGroup("End Game")] [SerializeField]
    private GameObject WinLoseScreenGO;
    [BoxGroup("End Game")] [SerializeField]
    private GameObject WinTextGO;
    [BoxGroup("End Game")] [SerializeField]
    private GameObject LoseTextGO;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI scoreText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalDamageDealtText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalDamageTakenText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalKillsText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalHeartsCollectedText;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalPlayTimeText;
    [BoxGroup("End Game")] [SerializeField]
    private Image bestHeartImage;
    [BoxGroup("End Game")] [SerializeField]
    private TextMeshProUGUI totalTurnsText;

    [BoxGroup("Possible Next Scenes")] [SerializeField]
    private Transform possibleNextScenesParent;
    [BoxGroup("Possible Next Scenes")] [SerializeField]
    private GameObject sceneTypesPrefab;
    [BoxGroup("Possible Next Scenes")] [SerializeField]
    private GameObject possibleNextScenesBG;

    TurnSystem TurnSystem;
    LevelManager LevelManager;
    
    private void Awake()
    {
        TurnSystem = everythingUseful.TurnSystem;
        LevelManager = everythingUseful.LevelManager;
    }

    private void OnEnable()
    {
        TurnSystem.TurnChange += UpdateTurn;
        TurnSystem.RoundChange += UpdateRound;
        TurnSystem.OnCombatStartAction += ()=> turnAndRoundGO.SetActive(true);
        TurnSystem.OnCombatEndAction += ()=> turnAndRoundGO.SetActive(false);
    }

    private void OnDisable()
    {
        TurnSystem.TurnChange -= UpdateTurn;
        TurnSystem.RoundChange -= UpdateRound;
        TurnSystem.OnCombatStartAction -= ()=> turnAndRoundGO.SetActive(true);
        TurnSystem.OnCombatEndAction -= ()=> turnAndRoundGO.SetActive(false);
    }

    private void Start()
    {
        UpdateTurn(TurnSystem.turn);
        UpdateRound(TurnSystem.round);
    }

    private void UpdateTurn(int turn)
    {
        turnText.text = turn.ToString();
    }

    private void UpdateRound(int round)
    {
        roundText.text = round.ToString();
    }

    public void GameOver(bool win, int score, int damageDealt, int damageTaken, int kills, int heartsCollected, string playTime)
    {
        WinLoseScreenGO.SetActive(true);
        // totalDamageDealtText.text = damageDealt.ToString();
        LerpUnscaled(totalDamageDealtText, damageDealt, 1);
        // totalDamageTakenText.text = damageTaken.ToString();
        LerpUnscaled(totalDamageTakenText, damageTaken, 3);
        // totalKillsText.text = kills.ToString();
        LerpUnscaled(totalKillsText, kills, 5);
        // totalHeartsCollectedText.text = heartsCollected.ToString();
        LerpUnscaled(totalHeartsCollectedText, heartsCollected, 7);
        //scoreText.text = score.ToString();
        LerpUnscaled(scoreText, score, 10);
        totalPlayTimeText.text = playTime;
        
        int prevRarity = 0;
        foreach (var player in LevelManager.players)
        {
            if (player.isUnlocked && player.heartContainer.heartData != null)
            {
                if ((int)player.heartContainer.heartData.heartRarity >= prevRarity)
                {
                    prevRarity = (int) player.heartContainer.heartData.heartRarity;
                    bestHeartImage.sprite = player.heartContainer.heartData.HeartSprite;
                }
            }
            bestHeartImage.transform.DOPunchScale(bestHeartImage.transform.localScale * 1.1f, 0.5f, 1, 1).SetDelay(9);

        }
        
        //totalTurnsText.text = TurnSystem.totalTurnsInGame.ToString();
        if (win)
        {
            WinTextGO.SetActive(true);
        }
        else
        {
            LoseTextGO.SetActive(true);
        }
    }

    private int current = 0;
    public void LerpUnscaled(TextMeshProUGUI text, int targetValue, float delay)
    {
        current = 0;
        DOTween.To(() => current, x => current = x, targetValue, 1).SetDelay(delay).OnComplete(()=> OnComplete(text)).OnUpdate(()=> OnUpdate(text, current)).SetUpdate(true);

    }

    public void OnComplete(TextMeshProUGUI text)
    {
        current = 0;
        text.transform.DOPunchScale(text.transform.localScale * 1.5f, 0.5f, 1, 1);
        current = 0;
    }

    public void OnUpdate(TextMeshProUGUI text, int current)
    {
        text.text = current.ToString();
    }

    //in unity c# write me a method that uses a coroutine and takes in TextMeshProUGUI text, float targetValue, float delay as parameterss, then increases the text's text value to targetValue in a given time
    /*private void IncreaseText(TextMeshProUGUI text, int targetValue, float delay)
    {
        StartCoroutine(IncreaseTextEnum(text, targetValue, delay));
    }

    private IEnumerator IncreaseTextEnum(TextMeshProUGUI text, int targetValue, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        int currentValue = 0;
        while (currentValue < targetValue)
        {
            currentValue++;
            text.text = currentValue.ToString();
            yield return new WaitForSecondsRealtime(0.0000000000001f);
        }
    }*/
    
    public void PauseGame(bool value)
    {
        PauseScreenGO.SetActive(value);
    }

    List<GameObject> spawnedSceneTypes = new();
    public void SpawnSceneTypeButtons(Sprite spriteIn, string nameIn, Color colorIn, UnityAction callbackIn)
    {
        possibleNextScenesBG.SetActive(true);
        GameObject temp = Instantiate(sceneTypesPrefab, possibleNextScenesParent);
        spawnedSceneTypes.Add(temp);
        temp.GetComponent<Image>().color = colorIn;
        temp.transform.GetChild(0).GetComponent<Image>().sprite = spriteIn;
        temp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = nameIn;
        temp.GetComponent<Button>().onClick.AddListener(callbackIn);
    }

    [Button, GUIColor(1f, 1f, 1f)]
    public void ClearSceneTypeButtons()
    {
        possibleNextScenesBG.SetActive(false);
        foreach (var sceneType in spawnedSceneTypes)
        {
            Destroy(sceneType);
        }
        spawnedSceneTypes.Clear();
    }
    
}
