using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Transforms
{



public class SGT_MoveTowerds : MonoBehaviour
{


    [Tooltip("Hareket decek objeyi seçeceksiniz.")]
    public Transform MovingObject;
    [Tooltip("Gidilecek hedef seçilecek.")]
    public Transform TargetPosition;
    public float Speed = 1;

    [Tooltip("False yaparak disable edebilirsin.")]
    public bool MovingNow = true;
    [Tooltip("Eğer objenin scale olmasını istiyorsan true yap.")]
    public bool Scale = false;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StartMoving = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent Moving = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent TargetArriva = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public SGTEventGameObject ThisTargetArrive = new SGTEventGameObject();

    private bool OneTimeArrive;
    private bool OneTimeStart;

    private void OnEnable()
    {
        OneTimeArrive = false;


    }

    private void Start()
    {
        StartMoving.Invoke();
    }

    // Update is called once per frame
    public void UpdateThis()
    {
        if (MovingNow)
        {
            float step = Speed * Time.deltaTime; // calculate distance to move
            MovingObject.position = Vector3.MoveTowards(MovingObject.position, TargetPosition.position, step);
            if (Scale)
            {
                MovingObject.localScale = Vector3.MoveTowards(MovingObject.localScale, TargetPosition.localScale, step);
            }

            Moving.Invoke();


            if (Vector3.Distance(MovingObject.position, TargetPosition.position) < 0.001 && !OneTimeArrive)
            {
                TargetArriva.Invoke();
                ThisTargetArrive.Invoke(MovingObject.gameObject);
                OneTimeArrive = true;
            }

        }



    }

    //Moving object'i değiştirebilirsin.
    public void ChangeMovingObject(GameObject NewObject)
    {
        MovingObject = NewObject.transform;

    }



    //Target'i değiştirebilirsin.
    public void ChangeTargetObject(Transform NewObject)
    {

        TargetPosition = NewObject;
    }

    //Hızı değiştirebilirsin.
    public void ChangeSpeed(float NewSpeed)
    {

        Speed = NewSpeed;
    }


    //Objenin hareketini durdurur.
    public void MovingNowChangeAsStop()
    {

        MovingNow = false;
    }

    //Objenin hareketini tekrar başlatır.
    public void MovingNowChangeAsStart()
    {

        MovingNow = true;
        OneTimeArrive = false;
    }
}
}
