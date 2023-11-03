

using UnityEditor;
using UnityEngine;
namespace SGT_Tools.Tools
{

    //Bu script sahnedeki objeleri belirlediğin obje ile değiştirmek için yazıldı. 2018 h.g.

#if UNITY_EDITOR

    public class SGT_ReplaceEditMode : MonoBehaviour
    {

        [Tooltip("Bu obje ile değiştir.")]
        public GameObject ReplaceWithThis;

        [Tooltip("Değiştirelecek objeleri ata")]
        public GameObject[] SelectReplaceObject;



        [ContextMenu("ReplaceObjects")]
        public void ReplaceObjects()
        {


            for (int i = 0; i < SelectReplaceObject.Length; i++)
            {

                GameObject TempObject1 = (GameObject)PrefabUtility.InstantiatePrefab(ReplaceWithThis);


                TempObject1.transform.position = SelectReplaceObject[i].transform.position;
                TempObject1.transform.rotation = SelectReplaceObject[i].transform.rotation;
                TempObject1.transform.localScale = SelectReplaceObject[i].transform.lossyScale;
                if (SelectReplaceObject[i].transform.parent != null)
                {
                    print("Bu objelerin parenti var :" + SelectReplaceObject[i].name);
                    TempObject1.transform.parent = SelectReplaceObject[i].transform.parent;
                }


                print("This Object : " + SelectReplaceObject[i].name + " replace with this object : " + TempObject1.name);

            }









        }






        [ContextMenu("DeleteObjects")]
        public void DeleteObject()
        {

            for (int i = 0; i < SelectReplaceObject.Length; i++)
            {

                DestroyImmediate(SelectReplaceObject[i]);
            }

            System.Array.Resize(ref SelectReplaceObject, 0); //Array'i sıfırlamak için kullanıyorum.
            ReplaceWithThis = null;
        }



        [ContextMenu("!!!! Replace And Clear!!!!")]
        public void ReplaceAndClear()
        {

            ReplaceObjects();
            DeleteObject();

        }

    }

#endif
}