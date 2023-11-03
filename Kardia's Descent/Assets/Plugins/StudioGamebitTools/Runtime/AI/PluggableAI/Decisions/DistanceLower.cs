using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/DistanceLower")]
    public class DistanceLower : Decision

    {
 
        public float LowerDistance;
    
        [Tooltip("Duvar olarak eklenmesini istediğin layerları seçmelisin. Eğer bu özelliği kullanmak istemezsen layerden nothing seçili olması yeterli")]
        public LayerMask FriendlyLayer;
        RaycastHit hit;
        [Multiline]
        public string DeveloperDescription = "Eğer belirlediğimiz Distance'dan küçükse işlemler gerçekleşir";
        public override bool Decide(StateController controller)
        {
            return Scan(controller);
        }

        private bool Scan(StateController controller)
        {


            if (Vector3.Distance(controller.PlayerCenterPosition.transform.position, controller.transform.position) < LowerDistance && !Physics.Linecast(controller.Player.transform.position, controller.LineCastPosition.position, out hit, FriendlyLayer))

            {
                return true;


            }
            else
            {
                return false;
            }


        }

    }
}

