using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    
    

    public void ChangeScene(string Value)
    {
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

}