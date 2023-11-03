using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Random =UnityEngine.Random;





namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/PatrolWithWaitAction")]
    public class PatrolWithWaitAction : Action
    {

        //public float WaitTime;

        [MinMaxSlider(0, 10)]
        [SerializeField] public Vector3 WaitTime ;
        [SerializeField] private float AgentSpeed=3.5f;
        [Tooltip("Eğer Mother Flower aktif değilse , patrol yapar.Bu seçenek sadece Mother flower için kullanılabilir.")]
        [SerializeField] private bool MotherFlowerActive = false; // Bu kısmı sadece mother flower için kullan.
        [Tooltip("Karakterin ne kadar uzaklıklara gideceğini gösterir.")]
        [SerializeField] private Vector2 MotherFlowerPosDistLimit = Vector2.zero;
        [SerializeField] private Vector3 MotherFlowerNewPositionLeft = Vector3.zero;
        [SerializeField]private Vector3 MotherFlowerNewPositionRight = Vector3.zero;

        public override void Act(StateController controller)
	    {
		    Patrol(controller);
	    }
	    private float Timer;
        private Vector3 thePosition;
        private Vector3 thePosition2;
        private void Patrol(StateController controller)
	    {
            if (controller.wayPointList.Count == 0 && MotherFlowerActive)
            {
                GameObject LeftPatrolPos = new GameObject();
                LeftPatrolPos.name = "LeftPatrolPos";
                thePosition = controller.gameObject.transform.TransformPoint(Vector3.right * Random.Range(MotherFlowerPosDistLimit.x, MotherFlowerPosDistLimit.y));
                LeftPatrolPos.transform.position = thePosition;
                controller.wayPointList.Add(LeftPatrolPos.transform);

                GameObject RightPatrolPos = new GameObject();
                RightPatrolPos.name = "RightPatrolPos";
                thePosition2 = controller.gameObject.transform.TransformPoint(Vector3.left * Random.Range(MotherFlowerPosDistLimit.x, MotherFlowerPosDistLimit.y));
                RightPatrolPos.transform.position = thePosition2;
                controller.wayPointList.Add(RightPatrolPos.transform);
            }
            NavMeshHit hit;
            if (controller.navMeshAgent.pathPending && NavMesh.SamplePosition(controller.wayPointList[controller.nextWayPoint].position, out hit,2.0f,NavMesh.AllAreas))
            {
                controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
                controller.navMeshAgent.isStopped = false;
                controller.navMeshAgent.speed = AgentSpeed;
            }
        

		    if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.slowdownDistance && !controller.navMeshAgent.pathPending)
		    {
           
                controller.navMeshAgent.isStopped = true;
			    Timer += Time.deltaTime;

			    if (Timer>=Random.Range(WaitTime.x,WaitTime.y)) {


                    MotherFlower(controller);
                    WaitHere(controller);
              

                }

		    }
	    }


	    private void WaitHere(StateController controller){
		
	
		    controller.navMeshAgent.isStopped = false;
		    controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
	
		    Timer = 0;
	    }


	    //Eğer Mother Flower seçeneği aktif ise bu kısım çalışır. WayPoint'e sadece iki seçenek eklersen yönünü sürekli olarak Local'e göre ayarlar ve waypointdeki pozisyonları değiştirir.
	    private void MotherFlower(StateController controller){
	
		    if (MotherFlowerActive) {

			    if (controller.nextWayPoint==1) {
                    thePosition = controller.gameObject.transform.TransformPoint(Vector3.right * Random.Range(MotherFlowerPosDistLimit.x, MotherFlowerPosDistLimit.y));
                    controller.wayPointList [0].position = thePosition;
                }
                else if (controller.nextWayPoint == 0)
                {
                    thePosition2 = controller.gameObject.transform.TransformPoint(Vector3.left * Random.Range(MotherFlowerPosDistLimit.x, MotherFlowerPosDistLimit.y));
                    controller.wayPointList [1].position = thePosition2;
                }
            }
	
	
	    }


    }
}





