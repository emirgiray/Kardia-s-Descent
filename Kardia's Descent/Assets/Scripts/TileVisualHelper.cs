using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileVisualHelper : MonoBehaviour
{
    [Title("ATAKAN BURAYA BAK ALOOOOOO")]
    
    [Button(ButtonSizes.Large), GUIColor(0.1f, 1f, 0.1f) ]
    public void TurnVisualsOn()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    [Button(ButtonSizes.Large), GUIColor(1f, 0.1f, 0.1f)]
    public void TurnVisualsOff()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    [Button(ButtonSizes.Large), GUIColor(1f, 1f, 0.1f)]
    public void UpdateVisuals()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Tile>().DeactivateVisualForEdior();
        }
    }
}
