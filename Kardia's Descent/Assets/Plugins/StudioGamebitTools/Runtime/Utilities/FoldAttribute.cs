

using UnityEngine;

//Örnek
//[Foldout("Saldırı kategorisi", true)]  // bu satırı ekledikten sonra altındakiler kategori içine alınacaktır.
//[SerializeField] private int Total;
//[SerializeField] private GameObject Karakter;


namespace SGT_Tools
{
	public class FoldoutAttribute : PropertyAttribute
	{
		public string name;
		public bool foldEverything;
 
		/// <summary>Adds the property to the specified foldout group.</summary>
		/// <param name="name">Name of the foldout group.</param>
		/// <param name="foldEverything">Toggle to put all properties to the specified group</param>
		public FoldoutAttribute(string name, bool foldEverything = false)
		{
			this.foldEverything = foldEverything;
			this.name = name;
		}
	}
}