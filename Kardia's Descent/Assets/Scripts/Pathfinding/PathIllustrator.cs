using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.33f;
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
            line.positionCount = path.tiles.Count;

            for (int i = 0; i < path.tiles.Count; i++)
            {
                Transform tileTransform = path.tiles[i].transform;
                line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
            }
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
}
