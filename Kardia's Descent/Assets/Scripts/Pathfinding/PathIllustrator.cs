using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    [SerializeField] private /*const*/ float LineHeightOffset = 0.33f;
    LineRenderer line;
    private bool canIllustratePath = true;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path path)
    {
        if (canIllustratePath)
        {
            Character character = Interact.Instance.selectedCharacter;
            
            line.positionCount = path.tiles.Count;

            for (int i = 0; i < path.tiles.Count; i++)
            {
                Transform tileTransform = path.tiles[i].transform;
                line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
            }

            /*var gradient = new Gradient();
            var colors = new GradientColorKey[path.tiles.Count];
            var alphas = new GradientAlphaKey[path.tiles.Count];
            //Debug.Log($"{path.tiles.Count}");
            for (int i = 0; i < path.tiles.Count; i++)
            {
                Transform tileTransform = path.tiles[i].transform;
                line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
                alphas[i] = new GradientAlphaKey(1.0f, 0.0f);

                //Debug.Log($"i: {i} remainingActionPoints: {character.remainingActionPoints - i}");

                 /*if (character.remainingActionPoints - i == 0)
                {
                    colors[i] = new GradientColorKey(Color.red, i / line.positionCount);
                    line.colorGradient = gradient;
                }
                else if (character.remainingActionPoints - i > character.remainingActionPoints / 2 /*&& character.remainingActionPoints - i <= character.remainingActionPoints#2#)
                {
                    colors[i] = new GradientColorKey(Color.yellow, i / line.positionCount);
                    line.colorGradient = gradient;
                }
                else if (character.remainingActionPoints - i >= 0 && character.remainingActionPoints - i <= character.remainingActionPoints / 2)
                {
                    colors[i] = new GradientColorKey(Color.green, i / line.positionCount);
                    line.colorGradient = gradient;
                }#1#

                 if (i == 3)
                 {
                     colors[i] = new GradientColorKey(Color.green, i / line.positionCount);
                     line.colorGradient = gradient;
                 }

                 if (i == 2)
                 {
                     colors[i] = new GradientColorKey(Color.blue, i / line.positionCount);
                     line.colorGradient = gradient;
                 }

                 if (i == 0)
                 {
                     colors[i] = new GradientColorKey(Color.red, i / line.positionCount);
                     line.colorGradient = gradient;
                 }

                 if (i == 1)
                 {
                     colors[i] = new GradientColorKey(Color.yellow, i / line.positionCount);
                     line.colorGradient = gradient;
                 }

                /*if (character.remainingActionPoints - i == 0)
                {
                    colors[i] = new GradientColorKey(Color.yellow, 0.0f);
                    line.colorGradient = gradient;
                } #1#

                gradient.SetKeys(colors, alphas);
                //line.colorGradient = MakeColors();
            }*/
        }
    }

    [Button]
    public void ClearIllustratedPath()
    {
        line.positionCount = 0;
    }
    
    public void EnableIllustratePath(bool value)
    {
        canIllustratePath = value;
    }

    public Gradient MakeColors()
    {
        // if (Interact.Instance.RetrievePath(Interact.Instance.selectedCharacter, out Path newPath
        var gradient = new Gradient();

        // Blend color from blue at 0% to red at 100%
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.blue, 0.0f);
        colors[1] = new GradientColorKey(Color.red, 1.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colors, alphas);
        return gradient;
        // What's the color at the relative time 0.25 (25%) ?
        Debug.Log(gradient.Evaluate(0.25f));
    }
}
