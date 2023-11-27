using UnityEngine;

public class RandomizeAppearance : MonoBehaviour
{
    public GameObject[] hats; // Array containing hat options
    public GameObject[] facialHair; // Array containing facial hair options
    public GameObject[] masks; // Array containing mask options

    public Material[] skins; // Array containing skin options
    public Renderer characterRenderer;

    public GameObject playerObject;
    public float minScale = 0.1f;
    public float maxScale = 10.0f;

    void Start()
    {        
        RandomizePlayerAppearance();
        RandomizePlayerSkin();
        RandomizePlayerSize();
    }

    void RandomizePlayerAppearance()
    {
        // Randomly activate a hat
        if (hats.Length > 0)
        {
            int randomHatIndex = Random.Range(0, hats.Length);
            hats[randomHatIndex].SetActive(true);
        }

        // Randomly activate a mask
        if (masks.Length > 0)
        {
            int randomMaskIndex = Random.Range(0, masks.Length);
            masks[randomMaskIndex].SetActive(true);

            if (randomMaskIndex == 0)
            {
                // Activate facial hair only if the first mask is chosen
                foreach (GameObject hair in facialHair)
                {
                    bool activateHair = Random.Range(0f, 1f) > 0.5f; // 50% chance of activating
                    hair.SetActive(activateHair);
                }
            }
        }
    }
    void RandomizePlayerSkin()
    {
        // Randomly select a material
        if (skins.Length > 0 && characterRenderer != null)
        {
            int randomSkinIndex = Random.Range(0, skins.Length);
            Material selectedSkin = skins[randomSkinIndex];

            // Apply the selected material to the character's renderer
            characterRenderer.material = selectedSkin;
        }
    }
    void RandomizePlayerSize()
    {
        if (playerObject != null)
        {
            float randomScale = Random.Range(minScale, maxScale);
            playerObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
    }
}
