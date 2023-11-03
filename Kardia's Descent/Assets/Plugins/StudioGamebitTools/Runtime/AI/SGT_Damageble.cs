using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SGT_Tools.Interface;
using Sirenix.OdinInspector;
using SGT_Tools.Variables;
namespace SGT_Tools.AI
{
    public class SGT_Damageble : MonoBehaviour, IDamageble<GameObject,Vector3, Vector3>
    {

        public SGT_Health health;
        public int MultiplyDamage = 1;
        public int Defend = 0;
        public SGTIntVariables DefendWithVariable;
        public bool CanGetDamage = true;

        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public UnityEvent DamageEvent = new UnityEvent();
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public UnityEvent AfterDieDamage = new UnityEvent();
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public SGTEventVector3Vector3 GetDamagePointNormal = new SGTEventVector3Vector3();




        public void Damage(GameObject Weapon,Vector3 DamagePoint, Vector3 Normal)
        {
            if (health!=null)
            {
                if (!health.isDead&&gameObject.activeInHierarchy&& CanGetDamage)
                {
                    if (Weapon.GetComponent<SGT_WeaponSelect>()!=null&& Weapon.GetComponent<SGT_WeaponSelect>().CanGiveDamage)
                    {
                        int attack = Weapon.GetComponent<SGT_WeaponSelect>().Weapon.Attack;
                        int FinalAttack;

                        if (DefendWithVariable!=null) //Remove defend number from attack
                        {
                            FinalAttack = attack - DefendWithVariable.Value;
                        }
                        else
                        {
                            FinalAttack = attack - Defend;
                        }
                        
                        health.HealthDecrease(FinalAttack * MultiplyDamage);
                        DamageEvent.Invoke();
                        GetDamagePointNormal.Invoke(DamagePoint,Normal);
                    }
                
                }

                if (health.isDead)
                {
                    AfterDieDamage.Invoke();
                }

            }
        

        }





        public void CanGetDamageControl(bool Value)
        {
            CanGetDamage = Value;

        }

        public void SetMultiplyDamage(int Value)
        {
            MultiplyDamage = Value;
        }


    }

}
