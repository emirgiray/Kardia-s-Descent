﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGT_Tools.AI
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);

    }
}

