using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    [SerializeField] private Image[] SceneTypeImages;
    [SerializeField] private Image[] SceneTypeImageBGs;
    [SerializeField] private TextMeshProUGUI[] SceneTypeTexts;
    [OnValueChanged("EndDoorSwitch")]
    public bool isEndDoor = false;
    [SerializeField] public GameObject EndDoor;

    [Tooltip("Leave this empty if you want to use the random scene type from the scene changer, assign a scene type to force it for this door.")]
    [SerializeField] private SceneType SceneType;


    private void Awake()
    {
        if (SceneType != null)
        {
            SetSceneType(SceneType);
        }
    }

    public void EndDoorSwitch()
    {
        if (isEndDoor)
        {
            EndDoor.gameObject.SetActive(true);
        }
        else
        {
            EndDoor.gameObject.SetActive(false);
        }
    }

    public void InnerTrigger()
    {
        if (isEndDoor)
        {
            everythingUseful.SceneChanger.allSceneTypes.RemoveSceneType(SceneType);
            everythingUseful.SceneChanger.ChangeScene(SceneType.Scene.SceneName);
        }
    }

    public void SetSceneType(SceneType type)
    {
        SceneType = type;

        foreach (var image in SceneTypeImages)
        {
            image.sprite = type.typeImage;
        }
        
        foreach (var image in SceneTypeImageBGs)
        {
            image.color = type.bgColor;
        }

        foreach (var text in SceneTypeTexts)
        {
            text.text = type.typeName.ToString();
        }
    }

    public bool CheckIfAvailable()
    {
        return SceneType == null;
    }
}
