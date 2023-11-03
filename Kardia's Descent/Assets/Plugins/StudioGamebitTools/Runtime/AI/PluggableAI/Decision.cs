using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SGT_Tools.AI
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController controller);

    }
}

