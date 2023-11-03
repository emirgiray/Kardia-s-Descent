using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SGT_Tools.AI
{
    public class StateController : MonoBehaviour {

        public State currentstate; 
        public State remainState;
        [FoldoutGroup("Assigners")]
        public GameObject Player;
        [FoldoutGroup("Assigners")]
        public Transform LineCastPosition;
        [FoldoutGroup("Assigners")]
        public Transform PlayerCenterPosition;
        [FoldoutGroup("Assigners")]
        public Transform RootOfCharacter;
        [FoldoutGroup("Assigners")]
        public SGT_Health Health;
        
        

        [Space]
        [Header("Settings")]
        [FoldoutGroup("Assigners")]
        public Animator Anim;
        [FoldoutGroup("Assigners")]
        public Rigidbody rb;
        [FoldoutGroup("Assigners")]
        public AIPath navMeshAgent;
        
        

        [Tooltip("Eğer karakterin düşmana dönmesini istiyorsan bunu yönetebilirsin.")]
	    [HideInInspector] public bool TurnActive=true;

        [HideInInspector] public State previousState;
        [HideInInspector] public string previousAnim;
        [HideInInspector] public int nextWayPoint;
        [HideInInspector] public float stateTimeElapsed;
        [HideInInspector]public FinishAnim[] FinishAnim;
    
        [Tooltip("Buraya spawn Etmek istediğimiz karakterleri yada efektleri listeye he ekleyebiliriz.")]
        /*[HideInInspector]*/ public List<GameObject> Characters;
        
        private bool aiActive = true;
        [Header("Game Settings")]
    
    
        public Transform[] SpawnDirection;
        public List<Transform> wayPointList;
        public Transform CoverPoint;
        public Transform[] HidePoints;
        public bool TurnPlayer;
        [MinMaxSlider(0,100)]
        public Vector2 RandomWaitTimer;
        public bool AnimOver;
        public bool OneTimeAnim;
        public MyDecisionEvent StateChange;


        private void Start()
        {

            if (Anim!=null)
            {
                 FinishAnim = Anim.GetBehaviours<FinishAnim>();
                    for (int i = 0; i < FinishAnim.Length; i++)
                    {
                        FinishAnim[i].controller = this;  //Finish anim'e stateController'i gönderiyorum.
                    }
        
            }


            if (Player==null)
            {
                Player = GameObject.FindGameObjectWithTag("Player");

            }

            if (LineCastPosition==null)
            {
                LineCastPosition = Player.transform;
            }

            if (PlayerCenterPosition == null)
            {
                PlayerCenterPosition = Player.transform;
            }

   


        }


        bool OnceRun;
        void Update()
        {
            if (!aiActive)
                return;
            currentstate.UpdateState(this);
            if (!OnceRun)
            {
                currentstate.DoActionsOnce(this);
                OnceRun = true;
            }

        }






        public void TransitionToState(State nextState)
        {
            if (nextState != remainState)
            {
                previousState = currentstate;//Önceki State'i saklıyorum ki önceki state'e geri dönsün.
                currentstate = nextState;
                OnExitState();
             
            }
        }

        public void TransitionToRemainState(State nextState)
        {
           

                remainState = nextState;
               

            
        }



        private float WaitTimer;
        private void OnExitState()
        {
            stateTimeElapsed = 0;
            OneTimeAnim = false; //Animasyon onetime'ını resetliyor.
            OnceRun = false; //ActionOnce'ı sıfırlıyorum.
            AnimOver = false;
            if (Anim!=null)
            {
                if (previousAnim!="")
                {
                    Anim.ResetTrigger(previousAnim);
                }
                
            }
        
            WaitTimer = Random.Range(RandomWaitTimer.x, RandomWaitTimer.y);
        
        }

        void OnDrawGizmos()
        {
            if (currentstate != null && Player !=null)
            {
                Gizmos.color = currentstate.sceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position, 1);
            }
        }

       public bool CheckIfCountDownElapsed (float duration)
        {
            stateTimeElapsed += Time.deltaTime;
            return (stateTimeElapsed >= duration);
        }







        private float FireTimer;
        public void FireSpawner(GameObject FireObject,Vector2 FireSpeed,float FireOffset)
        {
            FireTimer += Time.deltaTime;
            if (FireTimer>=FireOffset)
            {
                GameObject TempFireObject = Instantiate(FireObject);
                TempFireObject.transform.position = transform.TransformPoint(SpawnDirection[0].localPosition);
                if (TempFireObject.GetComponent<Rigidbody>()!=null)
                {
                    TempFireObject.GetComponent<Rigidbody>().AddForce((Player.transform.position - TempFireObject.transform.position) * Random.Range(FireSpeed.x, FireSpeed.y));
                }
            

                FireTimer = 0;
            }



        }


        

        private int RandomAnim;
        public void AnimController(string[] AnimName, bool FinishAnim, bool DamOver)
        {

            if (!OneTimeAnim)
            {
                RandomAnim = Random.Range(0,AnimName.Length);
                Anim.SetTrigger(AnimName[RandomAnim]);
                previousAnim = AnimName[RandomAnim];
                OneTimeAnim = true;
            }

        
  
   
            if ( FinishAnim && Anim.GetCurrentAnimatorStateInfo(0).IsTag(AnimName[RandomAnim]) && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {

        
                AnimOver = true;

                //Bu kısmı kapattım damage aldıktan bi sorun vardı.

                //if (DamOver)
                //{
                //    DamageAI.DamageNow = false;
                //}
            }







        }







        public bool AnimFinish(string CurrentAnimName)
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnimName) && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f )
            {
                return true;
            }
            else
            {
                return false;
            }


        }





        private float RandomTimer;
        public bool RandomWait( )
        {


            if (RandomTimer >= WaitTimer)

            {
                RandomTimer = 0;
                return true;
            }
            else
            {
                RandomTimer += Time.deltaTime;
                return false;
            }


        }

        //Bu fonksiyon RotatePlayerAction'ı aktif yada kapalı yapıyor. Animation Eventlerle kontrol edilebilir.
        public void TurnPlayerFunc()
        {

            TurnPlayer = true;
        }

        public void NotTurnPlayerFunc()
        {

            TurnPlayer = false;
        }

        public bool alarm = false;
        public void AlarmFunc(bool Value)
        {
            alarm = Value;
        }







    }
    [System.Serializable]
public class MyDecisionEvent : UnityEvent<Decision> { }
}



