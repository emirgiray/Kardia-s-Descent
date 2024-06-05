using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    [SerializeField] private AllEnemyStats allEnemyStats;
    
    
    public List<SceneType> possibleNextScenes = new();
    public AllSceneTypes allSceneTypes;
    public List<Door> EndDoorsInScene = new();
    
    public SceneField firstLevel;
    public SceneField mainMenuLevel;
    public SceneField currentScene;

    [BoxGroup("Loading Screen")] [SerializeField]
     private GameObject laodingScreen;
    [BoxGroup("Loading Screen")] [SerializeField]
    private Image loadingBar;
    [BoxGroup("Loading Screen")] [SerializeField]
    private TextMeshProUGUI loadingText;

    public UnityEvent SceneIsChangingEvent;
    public bool isOnMainMenu => SceneManager.GetActiveScene().name == mainMenuLevel.SceneName;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().name;
        allEnemyStats.ResetToDefault();
    }

    [Button, GUIColor(1f, 1f, 1f)]
    public void OfferRandomScenes()
    {
        int offerCount = Random.Range(2, 4);

        for (int i = 0; i < offerCount; i++)
        {
            int randomScene = Random.Range(0, allSceneTypes.remainingSceneTypes.Count);
            if (possibleNextScenes.Contains(allSceneTypes.remainingSceneTypes[randomScene]))
            {
                i--;
                continue;
            }
            
            SceneType temp = allSceneTypes.remainingSceneTypes[randomScene];
            possibleNextScenes.Add(temp);

            everythingUseful.UIManager.SpawnSceneTypeButtons(temp.typeImage, temp.typeName.ToString(), temp.bgColor, ()=>
            {
                allSceneTypes.RemoveSceneType(temp);
                //ChangeScene(possibleNextScenes[i].Scene.SceneName);
                StartCoroutine(ChangeSceneDelay(temp.Scene.SceneName)); 
                everythingUseful.UIManager.ClearSceneTypeButtons();
                possibleNextScenes.Clear();
            });
        }
    }
    
    public void DecideRandomScenes()
    {
        StartCoroutine(DecideRandomScenesDelay());
    }

    private IEnumerator DecideRandomScenesDelay()
    {
        if (allSceneTypes.remainingSceneTypes.Count == 0)
        {
            Debug.Log("Not enough scenes to offer");
            DisableAvailableDoors();
            yield break;
        }
        
        int filledDoors = 0;
        for (int i = 0; i < EndDoorsInScene.Count; i++)
        {
            int randomScene = Random.Range(0, allSceneTypes.remainingSceneTypes.Count);
            if (possibleNextScenes.Contains(allSceneTypes.remainingSceneTypes[randomScene]))
            {
                i--;
                Debug.Log($"Scene already in list {allSceneTypes.remainingSceneTypes[randomScene].Scene.SceneName}");
                yield return new WaitForSeconds(.1f);
                continue;
            }

            if (EndDoorsInScene[i].CheckIfAvailable())
            {
                SceneType temp = allSceneTypes.remainingSceneTypes[randomScene];
                possibleNextScenes.Add(temp);
            
                EndDoorsInScene[i].SetSceneType(temp);
                filledDoors++;
            }

            if (filledDoors == allSceneTypes.remainingSceneTypes.Count) // if there are more empty doors than scenes
            {
                Debug.Log("Not enough scenes to offer");

                DisableAvailableDoors();
                
                yield break;
            }
           
            
        }
    }

    public void DisableAvailableDoors()
    {
        foreach (var door in EndDoorsInScene)
        {
            if (door.CheckIfAvailable())
            {
                Destroy(door.GetComponent<Collider>());
                door.EndDoor.gameObject.SetActive(false);
                Destroy(door);
            }
                    
        }
    }
    
    [Button, GUIColor(1f, 0.1f, 0.1f)]
    public void ClearOfferedScenes()
    {
       // everythingUseful.UIManager.ClearSceneTypeButtons();
        possibleNextScenes.Clear();
    }

    private IEnumerator ChangeSceneDelay(string Value)
    {
        yield return new WaitForSecondsRealtime(1);
        
        ChangeScene(Value);
    }

    public void LoadFirstLevel()
    {
        ChangeScene(firstLevel);
    }
    
    public void ChangeScene(string Value)
    {
        allEnemyStats.IncreaseStats();
        
        everythingUseful.MainPrefabScript.ClearPrevious();
        everythingUseful.SaveLoadSystem.inGameSaveData.lastScene = Value;
        everythingUseful.GameManager.ResetToDefault();
        currentScene = Value;
        // SceneManager.LoadSceneAsync(Value,LoadSceneMode.Single);
        ClearOfferedScenes();
        SceneIsChangingEvent?.Invoke();
        
        StartCoroutine(LoadSceneAsync(Value));
        StartCoroutine(LoadingTextAnim());
        
    }
    AsyncOperation asyncLoad;
    private IEnumerator LoadSceneAsync(string value)
    {
        loadingBar.fillAmount = 0;
        laodingScreen.SetActive(true); 
        asyncLoad = SceneManager.LoadSceneAsync(value, LoadSceneMode.Single); 
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingBar.fillAmount = progress;
            if (progress >= 1)
            {
                loadingBar.fillAmount = 1;
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        
        StopCoroutine(LoadingTextAnim());
        laodingScreen.SetActive(false);
        loadingBar.fillAmount = 0;
        
    }

    private IEnumerator LoadingTextAnim()
    {
        while (!asyncLoad.isDone)
        {
            loadingText.text = "Loading";
            yield return new WaitForSecondsRealtime(0.1f);
            loadingText.text = "Loading.";
            yield return new WaitForSecondsRealtime(0.1f);
            loadingText.text = "Loading..";
            yield return new WaitForSecondsRealtime(0.1f);
            loadingText.text = "Loading...";
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    
    public void ApplicationQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
#endif
    }
    
    public void ChangeNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            // SceneManager.LoadScene(nextSceneIndex);
            string sceneName = SceneManager.GetSceneByBuildIndex(nextSceneIndex).name;
            ChangeScene(sceneName);
        }
    }
    
    public void ReloadSameScene()
    {
        //everythingUseful.MainPrefabScript.ClearPrevious();
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneIsChangingEvent?.Invoke();
        //ClearOfferedScenes();
        //SceneManager.LoadScene(CurrentSceneIndex);
        ChangeScene(GetCurrentScene());
    }
    
    public void LoadSceneWithIndex(int indexScene)
    {
        // SceneManager.LoadScene(indexScene);
        string sceneName = SceneManager.GetSceneByBuildIndex(indexScene).name;
        ChangeScene(sceneName);
    }

    public void LoadMainMenu()
    {
        //everythingUseful.MainPrefabScript.ClearPrevious();
        everythingUseful.MainPrefabScript.PartyRoundCardsParent.SetActive(false);
        //everythingUseful.MainPrefabScript.SelectedPlayers.Clear();
        ChangeScene(mainMenuLevel);
    }

    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

}
[System.Serializable]
public class SceneField
{
    [SerializeField]
    private Object m_SceneAsset;
    [SerializeField]
    private string m_SceneName = "";
    public string SceneName
    {
        get { return m_SceneName; }
        set => m_SceneName = value;
    }
    //string to SceneField
    public static implicit operator SceneField(string sceneField)
    {
        return new SceneField { SceneName = sceneField };
    }

    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.SceneName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, GUIContent.none, _property);
        SerializedProperty sceneAsset = _property.FindPropertyRelative("m_SceneAsset");
        SerializedProperty sceneName = _property.FindPropertyRelative("m_SceneName");
        _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(_position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
            if (sceneAsset.objectReferenceValue != null)
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
            }
        }
        EditorGUI.EndProperty();
    }
}
#endif

