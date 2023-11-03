using System.Collections;
using UnityEngine;


namespace SGT_Tools.Interface
{

    public interface IDamageble<T,T1,T2>
    {
        void Damage(T Weapon,T1 HitPoint,T2 Queternion);
        
    }

}