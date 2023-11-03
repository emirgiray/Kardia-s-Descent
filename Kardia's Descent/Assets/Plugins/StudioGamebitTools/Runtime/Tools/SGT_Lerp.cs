using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using SGT_Tools;


public class SGT_Lerp : MonoBehaviour
{


    public float CurrentFloat;
    public float TargetFloat;
    public float LerpTime;
    public float Delay = 0;
    public Ease Easy;
    public LoopType Loop;
    public int LoopTime = 0;
    [Header("Move Object")]
    public Transform MoveThisObject = null;
    public Vector3 TargetMovePosition;

    [Space]
    [Space]
    [Space]
    [Space]
    public UnityEvent Start;
    
    public SGTEventFloat Moving;
    
    public UnityEvent Completed;
    
    public UnityEvent Pause;
    private int RandomId;

    //For Float Lerping
    public void LerpFloat()
    {
        RandomId = Random.Range(0, 90000000);
        DOTween.To(() => CurrentFloat, x => CurrentFloat = x, TargetFloat, LerpTime).SetDelay(Delay).OnComplete(OnComplete).OnStart(OnStart).OnPause(OnPause).OnUpdate(OnMoving).SetEase(Easy).SetLoops(LoopTime, Loop).SetId(RandomId);

    }


    public void PauseLerpFromID()
    {
        DOTween.Pause(RandomId);

    }


    public void RewindLerp()
    {
        DOTween.Rewind(RandomId);

    }


    public void RestartLerp()
    {

        DOTween.Restart(RandomId);
    }


    public void MoveObject()
    {
        MoveThisObject.transform.DOMove(TargetMovePosition, LerpTime).SetDelay(Delay).OnComplete(OnComplete).OnStart(OnStart).OnPause(OnPause).OnUpdate(OnMoving).SetEase(Easy).SetLoops(LoopTime, Loop);

    }

    public void RotateObject()
    {
        MoveThisObject.transform.DORotate(TargetMovePosition, LerpTime).SetDelay(Delay).OnComplete(OnComplete).OnStart(OnStart).OnPause(OnPause).OnUpdate(OnMoving).SetEase(Easy).SetLoops(LoopTime, Loop);

    }

    public void LocalMoveObject()
    {
        MoveThisObject.transform.DOLocalMove(TargetMovePosition, LerpTime).SetDelay(Delay).OnComplete(OnComplete).OnStart(OnStart).OnPause(OnPause).OnUpdate(OnMoving).SetEase(Easy).SetLoops(LoopTime, Loop);

    }

    public void LocalRotateObject()
    {
        MoveThisObject.transform.DOLocalRotate(TargetMovePosition, LerpTime).SetDelay(Delay).OnComplete(OnComplete).OnStart(OnStart).OnPause(OnPause).OnUpdate(OnMoving).SetEase(Easy).SetLoops(LoopTime, Loop);

    }


    public static float LerpFloatRemote(float _StartValue,float _TargetValue,float _time)
    {
        DOTween.To(() => _StartValue, x => _StartValue = x, _TargetValue, _time);
        return _StartValue;
    }


    public void SetMovePosition(Vector3 Value)
    {
        TargetMovePosition = Value;
    }


    private void OnStart()
    {
        Start.Invoke();

    }



    private void OnComplete()
    {

        Completed.Invoke();
    }


    private void OnPause()
    {

        Pause.Invoke();
    }


    public void OnMoving()
    {

        Moving.Invoke(CurrentFloat);
    }



    public void SetCurrenFloat(float Value)
    {
        CurrentFloat = Value;
    }


    public void SetTargetFloat(float Value)
    {
        TargetFloat = Value;
    }

    public void SetLerpTime(float Value)
    {
        LerpTime = Value;
    }

    public void SetDelay(float Value)
    {
        Delay = Value;
    }
}
