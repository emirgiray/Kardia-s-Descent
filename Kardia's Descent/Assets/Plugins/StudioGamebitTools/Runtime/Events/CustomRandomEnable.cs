
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class CustomRandomEnable : MonoBehaviour
{
    [MinMaxSlider(0, 100, true)]
    public Vector2 RandomStartTime;

#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StartEvent;



    
    void OnEnable()
    {
        StartCoroutine(StartRandom());

    }


    IEnumerator StartRandom()
    {


        yield return new WaitForSecondsRealtime(Random.Range(RandomStartTime.x, RandomStartTime.y));
        StartEvent.Invoke();
    }



}
