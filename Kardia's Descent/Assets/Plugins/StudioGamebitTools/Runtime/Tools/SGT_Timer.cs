using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SGT_Tools.Bridge;

namespace SGT_Tools.Tools
{


public class SGT_Timer : MonoBehaviour
{
    public float Timer;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent OnRaiseEvents = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventFloat CurrentTimer = new SGTEventFloat();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventFloat RaiseWithTimer = new SGTEventFloat();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventFloat StopTimer = new SGTEventFloat();

        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventFloat CurrentTimerWithNormalized = new SGTEventFloat();

        private bool Starter;
    private float _timer;


    void Update()
    {

        if (Starter && gameObject.activeInHierarchy)
        {
            _timer += Time.deltaTime;
            CurrentTimer.Invoke(_timer);
                float Normalized = SGT_Math.Remap(_timer, 0, Timer, 0, 1);
                CurrentTimerWithNormalized.Invoke(Normalized);
                if (_timer >= Timer)
            {
                OnRaiseEvents.Invoke();
                RaiseWithTimer.Invoke(_timer);
                _timer = 0;
                Starter = false;
            }

        }

    }



    public void WaitAndRaiseTimer()
    {
        Starter = true;

    }


    public void PauseTimer()
    {
        Starter = false;

    }



        public void SetStarter(bool Value)
        {
            Starter = Value;
        }


    public void StopTimerFunc()
    {

        StopTimer.Invoke(_timer);
        _timer = 0;
        Starter = false;
    }



}
}