using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/FireAction")]
    public class FireAction : Action
    {
        [Tooltip("Eğer player belirlediğimiz mesafe dışında hareket ederse player pozisyonunu update eder.")]
        [SerializeField] private GameObject FireObject = null;
        [MinMaxSlider(0, 1000)]
        [SerializeField] public Vector2 FireSpeed;
        [SerializeField] private float FireOffset = 0;

        public override void Act(StateController controller)
        {
            Fire(controller);
        }


        private void Fire(StateController controller)
        {
            controller.FireSpawner(FireObject,FireSpeed,FireOffset);

        }




    }
}


