using System.Collections;
using UnityEngine;


namespace SGT_Tools.Interface
{

        public interface ISelectable<T>
        {
            void Selected(T damageTaken);
            void NotSelected(T Value);
        }

}