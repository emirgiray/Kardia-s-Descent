using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace SGT_Tools.Variables
{

    public class SGTScoreUpdater : MonoBehaviour
    {

        public SGTIntVariables Score;
        public TextMeshPro Text1;
        public TextMeshProUGUI Text2;
        public TextMesh Text3;




        public void UpdateText()
        {

            if (Text1!=null)
            {
                Text1.text = Score.Value.ToString();
            }

            if (Text2 != null)
            {
                Text2.text = Score.Value.ToString();
            }

            if (Text3 != null)
            {
                Text3.text = Score.Value.ToString();
            }

        }


    }
}
