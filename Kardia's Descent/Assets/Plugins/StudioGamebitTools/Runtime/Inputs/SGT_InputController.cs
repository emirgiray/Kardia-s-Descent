using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;

namespace SGT_Tools.Inputs
{


public class SGT_InputController : MonoBehaviour
{
        [InfoBox("This gameobject should be local space When move it gives float value to events")]
        [SerializeField] private bool X;
        [SerializeField] private bool Y;
        [SerializeField] private bool Z;
        [SerializeField] private bool XYZ;

         public Vector2 MaxMinX = Vector2.zero;
         public Vector2 MaxMinY = Vector2.zero;
         public Vector2 MaxMinZ = Vector2.zero;

        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventFloat XInput = new SGTEventFloat();
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventFloat YInput = new SGTEventFloat();
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventFloat ZInput = new SGTEventFloat();
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventVector3 XYZInput = new SGTEventVector3();




        //Objeleri hareket ettirerek input çıkarıyoruz.
        public void UpdateThis()
    {
        if (X)
        {

            float CurrentX = SGT_Math.Remap(transform.localPosition.x, MaxMinX.x, MaxMinX.y, -1, 1);

            float LimitCurrentX = Mathf.Clamp(CurrentX, -1, 1);
            XInput.Invoke(LimitCurrentX);
            
        }
        if (Y)
        {

            float CurrentY = SGT_Math.Remap(transform.localPosition.y, MaxMinY.x, MaxMinY.y, -1, 1);

            float LimitCurrentY = Mathf.Clamp(CurrentY, -1, 1);
            YInput.Invoke(LimitCurrentY);
            
        }
        if (Z)
        {

            float CurrentZ = SGT_Math.Remap(transform.localPosition.z, MaxMinZ.x, MaxMinZ.y, -1, 1);

            float LimitCurrentZ = Mathf.Clamp(CurrentZ, -1, 1);
            ZInput.Invoke(LimitCurrentZ);
            
        }
            if (XYZ)
            {
                float CurrentX = SGT_Math.Remap(transform.localPosition.x, MaxMinX.x, MaxMinX.y, -1, 1);
                float LimitCurrentX = Mathf.Clamp(CurrentX, -1, 1);
                float CurrentY = SGT_Math.Remap(transform.localPosition.y, MaxMinY.x, MaxMinY.y, -1, 1);
                float LimitCurrentY = Mathf.Clamp(CurrentY, -1, 1);
                float CurrentZ = SGT_Math.Remap(transform.localPosition.z, MaxMinZ.x, MaxMinZ.y, -1, 1);
                float LimitCurrentZ = Mathf.Clamp(CurrentZ, -1, 1);
                Vector3 NewPos = new Vector3(LimitCurrentX, LimitCurrentY, LimitCurrentZ);
                XYZInput.Invoke(NewPos);
            }


    }


    }
}