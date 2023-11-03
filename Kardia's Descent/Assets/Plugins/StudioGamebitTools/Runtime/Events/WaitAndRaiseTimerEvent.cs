
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class WaitAndRaiseTimerEvent : MonoBehaviour
{

    public float Timer;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent OnRaiseEvents = new UnityEvent();

    private bool Starter;
    private float _timer;


    void Update()
    {

        if (Starter&&gameObject.activeInHierarchy)
        {
             _timer += Time.deltaTime;
            if (_timer>=Timer)
            {
                OnRaiseEvents.Invoke();
                
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


    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void TestNow()
    {
        OnRaiseEvents.Invoke();
    }
   
}
