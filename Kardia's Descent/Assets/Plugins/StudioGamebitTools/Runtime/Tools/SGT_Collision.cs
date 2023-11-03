
using UnityEngine;
using UnityEngine.Events;
public class SGT_Collision : MonoBehaviour
{

    public SGT_Policy Policy;
    public bool DisableComponent;
    public SGTEventGameObject CollisionEnter = null;
    public SGTEventGameObject CollisionExit = null;
    public SGTEventGameObject CollisionStay = null;
    public SGTEventDamageAI SentDamageAI = null;

    //Bunu scripti açıp kapatabilmek için yaptım.
    private void OnEnable()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && !(SGT_Policy.Check(collision.gameObject, Policy))&& !DisableComponent)
        {

            CollisionEnter.Invoke(collision.gameObject);

            Quaternion rote = Quaternion.FromToRotation(Vector3.back, collision.contacts[0].normal);
            SendDamageAI(collision.gameObject, rote, collision.contacts[0].point, collision.contacts[0].normal);
        }


      
    }



    private void OnCollisionExit(Collision collision)
    {
        if (collision != null && !(SGT_Policy.Check(collision.gameObject, Policy)) && !DisableComponent)
        {
            CollisionExit.Invoke(collision.gameObject);
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision != null && !(SGT_Policy.Check(collision.gameObject, Policy)) && !DisableComponent)
        {
            CollisionStay.Invoke(collision.gameObject);
        }
    }



    public void SendDamageAI(GameObject other, Quaternion Rote, Vector3 CollisionPoint, Vector3 Normal)
    {
        SentDamageAI.Invoke(other,Rote,CollisionPoint,Normal);
    }

    //Dışarıdan component'i disable etmek için kullanılabilir.
    public void DisableEnableComponent(bool Value)
    {

        DisableComponent = Value;
    }



}




