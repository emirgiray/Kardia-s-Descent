using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RoundInfo : MonoBehaviour
{
    [SerializeField] private EverythingUseful everythingUseful;
    
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private DOTweenAnimation doTweenAnimation;
    
    
    public void Rearrange(GameObject obj)
    {
        //set the first child of this gameobject to the last child
        obj.transform.SetAsLastSibling();
        //also move the first element to last
        /*objects.Add(obj);
        objects.Remove(obj);*/
    }
    
    public void AddObject(GameObject obj, Character character)
    {
        objects.Add(obj);
        int index = objects.IndexOf(obj);

        if (everythingUseful.TurnSystem.turnState == TurnSystem.TurnState.Friendly)
        {
            if (character is Player)
            {
                obj.transform.SetAsFirstSibling();
            }
            if (character is Enemy)
            {
                for (int i = 0; i < objects.Count - 1; i++)
                {
                    if (objects[i].GetComponent<CharacterRoundCard>().GetCharacter() is Enemy)
                    {
                        index = objects[i].transform.GetSiblingIndex();
                    }

                }
                
                obj.transform.SetSiblingIndex(index + 1);
            }
        }
        else if (everythingUseful.TurnSystem.turnState == TurnSystem.TurnState.Enemy)
        {
            if (character is Player)
            {
                for (int i = 0; i < objects.Count - 1; i++)
                {
                    if (objects[i].GetComponent<CharacterRoundCard>().GetCharacter() is Player)
                    {
                        index = objects[i].transform.GetSiblingIndex();
                    }

                }
                
                obj.transform.SetSiblingIndex(index + 1);
            }
            if (character is Enemy)
            {
                obj.transform.SetAsFirstSibling();
            }
        }
    }

    public void RemoveObject(GameObject obj)
    {
        objects.Remove(obj);
    }
    
    bool firstTime = true;

    private void Start()
    {
        if (doTweenAnimation != null)
        {
            doTweenAnimation.DOPause();
        }
    }

    public void AddArrow()
    {
        if (firstTime)
        {
            firstTime = false;
            return;
        }
        
        objects[0].SetActive(true);
        var firstElement = objects[0];
        objects.RemoveAt(0);
        objects.Add(firstElement);

       // if (!doTweenAnimation.isActive)
        {
            doTweenAnimation.DOPlay();
        }
    }

    public void RemoveArrow()
    {
        objects[objects.Count-1].SetActive(false);
        var lastElement = objects[objects.Count-1];
        objects.RemoveAt(objects.Count-1);
        objects.Insert(0, lastElement);

        int disabledCount = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            if (!objects[i].activeSelf)
            {
                disabledCount++;
            }
        }
        
        if (disabledCount == objects.Count)
        {
            TurnOffAllArrows();
        }
    }

    public void TurnOffAllArrows()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
        }
        
        firstTime = true;

        doTweenAnimation.DOPause();
    }
}
