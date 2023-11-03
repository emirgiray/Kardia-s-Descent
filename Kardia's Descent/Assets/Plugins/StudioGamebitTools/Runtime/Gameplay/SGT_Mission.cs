using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.Gameplay
{

public class SGT_Mission : MonoBehaviour
{
    //

    [Tooltip("Enter How many mission completed")]
    public int MissionNumber;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    [Tooltip("Bütün görevler tamamlandığında Çalışır.")]
    public UnityEvent MissionCompleted;
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    [Tooltip("Görevlerden herhangi biri yapıldığında çalışır.")]
    public UnityEvent MissionIncreased; //Görevlerden herhangi biri yapıldığında çalışır.
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent MissionDecreased; //Görevlerden herhangi biri yapıldığında çalışır..
    public bool MissionCompletedOk;
    [Tooltip("!!!It shows us Mission Situation please Dont Change it !!!!!")]
    public int MissionIncreaserNum;
    [Tooltip("Dilediğin zaman bu scripti etkisiz hale getirebilirsin")]
    public bool MissionsCancel = false;
    

#if UNITY_EDITOR
#pragma warning disable
        // stuff that should not get warnings

        [Multiline]
    [SerializeField] private string DeveloperDescription = "Test";
#pragma warning restore
#endif

        //Bu scripti iptal eder.
        public void CancelMissionScript()
    {

        MissionsCancel = true;
    }

    //Bu scripti tekrar aktif eder.
    public void ActivateAgainMissionScript()
    {

        MissionsCancel = false;
    }

    //Özel bi durumda değişkenler resetlenebilir.
    public void ResetFunc()
    {

        MissionCompletedOk = false;
        MissionIncreaserNum = 0;
    }




    public void MissionComp()
    {

        MissionCompleted.Invoke();
        MissionCompletedOk = true;
    }


    //Bu fonksyion çalıştığında bir sayı artırıyor.
    public void MissionIncrease()
    {
        if (!MissionsCancel)
        {
            MissionIncreaserNum++;
            MissionIncreased.Invoke();
            if (MissionIncreaserNum == MissionNumber)
            {
                MissionComp();
            }
        }

    }

    //Reset Missions to Beginning
    public void ResetMission()
    {
        MissionCompletedOk = false;
        MissionIncreaserNum = 0;

    }


    //Görevi geri almak için kullanılır.
    public void MissionDecrease()
    {
        if (!MissionCompletedOk && !MissionsCancel)
        {
            MissionIncreaserNum--;
            MissionDecreased.Invoke();
        }


    }


    public void CheckMissionCompleteOrNot()
    {
        if (MissionIncreaserNum < MissionNumber)
        {

            ResetMission();
        }
    }
 }
}
