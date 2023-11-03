
using System.Collections.Generic;
using UnityEngine;


public class SGT_Policy : MonoBehaviour
{
    //Alt Satırda nasıl kullanacağına dair örnek var.


    //public SGT_Policy Policy;  Bu scripti bunu yazdığın yere ekleyerek limit koyabilirsin.

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision != null && !(Naau_Policy.Check(collision.gameObject, Policy)))
    //    {

    //        print("Sadece izinli objelere dokununca çalışır");
    //    }

    //}



    //private void OnTriggerEnter(Collider other)
    //{

    //    if (other != null && !(Naau_Policy.Check(other.gameObject, Policy)))
    //    {
    //        print("Sadece izinli objelere dokununca çalışır");
    //    }

    //}







    /// <summary>
    /// The operation to apply on the list of identifiers.
    /// </summary>
    /// <param name="Ignore">Will ignore any game objects that contain either a tag or script component that is included in the identifiers list.</param>
    /// <param name="Include">Will only include game objects that contain either a tag or script component that is included in the identifiers list.</param>
    public enum OperationTypes
    {
        Ignore,
        Include
    }



    /// <summary>
    /// The types of element that can be checked against.
    /// </summary>
    /// <param name="Tag">The tag applied to the game object.</param>
    /// <param name="Script">A script component added to the game object.</param>
    /// <param name="Layer">A layer applied to the game object.</param>
    public enum CheckTypes
    {
        Tag = 1,
        Script = 2,
        Layer = 4
    }

    [Tooltip("The operation to apply on the list of identifiers.")]
    public OperationTypes operation = OperationTypes.Ignore;
    [Tooltip("The element type on the game object to check against.")]
    public CheckTypes checkType = CheckTypes.Tag;
    [Tooltip("A list of identifiers to check for against the given check type (either tag or script).")]
    public List<string> identifiers = new List<string>() { "" };
    [Tooltip("Ignore these objects with this object.It is only work in start 'OnEnable function'")]
    public Collider[] IgnoreThisObject;

    [Multiline]
    public string DeveloperDescription = "";



    public virtual bool Find(GameObject obj)
    {
        if (operation == OperationTypes.Ignore)
        {
            return TypeCheck(obj, true);
        }
        else
        {
            return TypeCheck(obj, false);
        }
    }


    private void OnEnable()
    {
        IgnoreTheseObjects();


    }

    //Burdaki atanmış objeleri arasında colliderları ignore edicek.
    private void IgnoreTheseObjects()
    {

        for (int i = 0; i < IgnoreThisObject.Length; i++)
        {
            if (IgnoreThisObject[i]!=null) { 
            Physics.IgnoreCollision(IgnoreThisObject[i].GetComponent<Collider>(), GetComponent<Collider>());
            }
        }

    }


    /// <summary>
    /// The Check method is used to check if a game object should be ignored based on a given string or policy list.
    /// </summary>
    /// <param name="obj">The game object to check.</param>
    /// <param name="list">The policy list to use for checking.</param>
    /// <returns>Returns true of the given game object matches the policy list or given string logic.</returns>
    public static bool Check(GameObject obj, SGT_Policy list)
    {
        if (list != null)
        {
            return list.Find(obj);
        }
        return false;
    }

    protected virtual bool ScriptCheck(GameObject obj, bool returnState)
    {
        for (int i = 0; i < identifiers.Count; i++)
        {
            if (obj.GetComponent(identifiers[i]))
            {
                return returnState;
            }
        }
        return !returnState;
    }

    protected virtual bool TagCheck(GameObject obj, bool returnState)
    {
        if (returnState)
        {
            return identifiers.Contains(obj.tag);
        }
        else
        {
            return !identifiers.Contains(obj.tag);
        }
    }

    protected virtual bool LayerCheck(GameObject obj, bool returnState)
    {
        if (returnState)
        {
            return identifiers.Contains(LayerMask.LayerToName(obj.layer));
        }
        else
        {
            return !identifiers.Contains(LayerMask.LayerToName(obj.layer));
        }
    }

    protected virtual bool TypeCheck(GameObject obj, bool returnState)
    {
        var selection = 0;

        if (((int)checkType & (int)CheckTypes.Tag) != 0)
        {
            selection += 1;
        }
        if (((int)checkType & (int)CheckTypes.Script) != 0)
        {
            selection += 2;
        }
        if (((int)checkType & (int)CheckTypes.Layer) != 0)
        {
            selection += 4;
        }

        switch (selection)
        {
            case 1:
                return TagCheck(obj, returnState);
            case 2:
                return ScriptCheck(obj, returnState);
            case 3:
                if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                {
                    return returnState;
                }
                if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                {
                    return returnState;
                }
                break;
            case 4:
                return LayerCheck(obj, returnState);
            case 5:
                if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                {
                    return returnState;
                }
                if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                {
                    return returnState;
                }
                break;
            case 6:
                if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                {
                    return returnState;
                }
                if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                {
                    return returnState;
                }
                break;
            case 7:
                if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                {
                    return returnState;
                }
                if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                {
                    return returnState;
                }
                if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                {
                    return returnState;
                }
                break;
        }

        return !returnState;
    }
}








