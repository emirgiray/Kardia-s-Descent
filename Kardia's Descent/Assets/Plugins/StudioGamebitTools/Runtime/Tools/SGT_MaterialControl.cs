using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGT_MaterialControl : MonoBehaviour
{

    [Header("Materialların belirli isimde olanlarını değiştirmek için kullanılır.")]
    public string materialName = "";
    [Header("Materialların belirli isimde olanlarını color için değiştirmek için kullanılır.")]
    public string materialColorName = "";
    [Tooltip("Material'ın index numarasına göre değiştirmek içindir.Tek material varsa index 0 olmalıdır.")]
    public int MaterialIndex = 0;
    [Tooltip("Eğer Karakter skinmesh ise buna atamalısın yada boş bırakmalısın")]
    [SerializeField] private SkinnedMeshRenderer[] matSkin = null;
    [Tooltip("Eğer Karakter RenderMesh ise buna atamalısın yada boş bırakmalısın")]
    [SerializeField] private MeshRenderer[] matMesh = null;
    [SerializeField] private Material[] mat = null;
    
    [SerializeField]
    private Color Color;


    /// <summary>
    /// Objelerin materiallerinde ki özel noktaları string ile girerek değiştirmek için kullanılabilir.
    /// </summary>
    /// <param name="NewValue"></param>
    public void ChangeMaterial(float NewValue)
    {


        for (int i = 0; i < matSkin.Length; i++)
        {
            if (matSkin[i] != null)
            {
                matSkin[i].materials[MaterialIndex].SetFloat(materialName, NewValue);
            }

        }

        for (int i = 0; i < matMesh.Length; i++)
        {
            if (matMesh[i] != null)
            {
                matMesh[i].materials[MaterialIndex].SetFloat(materialName, NewValue);
            }
        }

        for (int i = 0; i < mat.Length; i++)
        {
            if (mat[i] != null)
            {
                mat[i].SetFloat(materialName, NewValue);
            }
        }

    }




    /// <summary>
    /// Material'ın rengini değiştirmek için
    /// </summary>
    public void ChangeColor()
    {


        for (int i = 0; i < matSkin.Length; i++)
        {
            if (matSkin[i] != null)
            {
                matSkin[i].materials[MaterialIndex].SetColor(materialName, Color);
            }

        }

        for (int i = 0; i < matMesh.Length; i++)
        {
            if (matMesh[i] != null)
            {
                matMesh[i].materials[MaterialIndex].SetColor(materialName, Color);
            }
        }




    }

     /// <summary>
     /// Color Rengini dışardan değiştirebiliriz.
     /// </summary>
     /// <param name="NewColor"></param>
    public void SetColor(Color NewColor)
    {
        Color = NewColor;

    }


    /// <summary>
    /// Material'ın rengini değiştirmek için
    /// </summary>
    public void ChangeColorByName()
    {


        for (int i = 0; i < matSkin.Length; i++)
        {
            if (matSkin[i] != null)
            {
                matSkin[i].materials[MaterialIndex].SetColor(materialColorName, Color);
            }

        }

        for (int i = 0; i < matMesh.Length; i++)
        {
            if (matMesh[i] != null)
            {
                matMesh[i].materials[MaterialIndex].SetColor(materialColorName, Color);
            }
        }




    }


}
