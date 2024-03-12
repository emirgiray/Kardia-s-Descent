using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    
    public List<SceneType> possibleNextScenes = new();
    
    public AllSceneTypes allSceneTypes;

    public SceneField firstLevel;
    public SceneField mainMenuLevel;

    public bool isOnMainMenu => SceneManager.GetActiveScene().name == mainMenuLevel.SceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
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

            UIManager.Instance.SpawnSceneTypeButtons(temp.typeImage, temp.typeName.ToString(), ()=>
            {
                allSceneTypes.RemoveSceneType(temp);
                //ChangeScene(possibleNextScenes[i].Scene.SceneName);
                StartCoroutine(ChangeSceneDelay(temp.Scene.SceneName)); 
                UIManager.Instance.ClearSceneTypeButtons();
                possibleNextScenes.Clear();
            });
        }
    }
    [Button, GUIColor(1f, 0.1f, 0.1f)]
    public void ClearOfferedScenes()
    {
        UIManager.Instance.ClearSceneTypeButtons();
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
        SaveLoadSystem.Instance.saveData.lastScene = Value;
        GameManager.Instance.ResetToDefault();
        //SaveLoadSystem.Instance.SaveGame();
        SceneManager.LoadSceneAsync(Value,LoadSceneMode.Single);
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
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    
    public void ReloadSameScene()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(CurrentSceneIndex);
    }
    
    public void LoadSceneWithIndex(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
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

