using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGT_Tools.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/WaitTime")]
    public class WaitTime : Decision
    {

	    public float WaitTimer;

        public void ResetValue()
        {
            
            Timer = 0;
        }

        public override bool Decide(StateController controller)
        {
            return Wait(controller);
        }

        private float Timer;
        private bool Wait(StateController controller)
        {
       

		    if (Timer>= WaitTimer)

            {
                Timer = 0;
                return true;
            }
            else
            {
                Timer += Time.deltaTime;
                return false;
            }


        }

    }
}


