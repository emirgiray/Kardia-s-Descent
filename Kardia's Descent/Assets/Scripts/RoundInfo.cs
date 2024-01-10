using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    
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

        if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Friendly)
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
        else if (TurnSystem.Instance.turnState == TurnSystem.TurnState.Enemy)
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
}
