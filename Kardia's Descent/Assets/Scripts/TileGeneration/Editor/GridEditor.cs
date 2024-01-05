using UnityEditor;
using UnityEngine;

public class GridEditor : EditorWindow
{
    #region member fields
    GameObject parent;
    GameObject tile;
    Vector3 gridPosition;
    Vector2Int gridSize = new Vector2Int(15, 12);
    #endregion

    [MenuItem("Window / Tools / Grid Generator")]

    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(GridEditor));
    }

    void OnGUI()
    {
        Fields();
        Buttons();
    }

    void Fields()
    {
        tile = (GameObject)EditorGUILayout.ObjectField("Tile", tile, typeof(GameObject), true);

        if (parent != null)
            GUILayout.Label("Parent grid: " + parent.name);
        else
            GUILayout.Label("No active grid focused!");

        EditorGUILayout.Space(10f);

        gridPosition = EditorGUILayout.Vector3Field("Grid Start Position", gridPosition);

        gridSize.x = Mathf.Clamp(EditorGUILayout.IntField("Width", gridSize.x), 0, 99);
        gridSize.y = Mathf.Clamp(EditorGUILayout.IntField("Length", gridSize.y), 0, 99);

        EditorGUILayout.Space(20f);
    }

    void Buttons()
    {
        GUILayout.Label("Grid selection");

        if (GUILayout.Button("Create new grid"))
            CreateNewParent();


        if (GUILayout.Button("Focus Grid"))
            FocusOnGrid();

        EditorGUILayout.Space(20f);
        GUILayout.Label("Grid manipulation");

        if (tile == null || parent == null)
            return;

        if (GUILayout.Button("(re)Generate"))
            if (tile != null)
                GenerateGrid();
            else
                Debug.Log("Select a tile to generate");

        if (GUILayout.Button("Connect two tiles"))
            CreateLadder();

        if (GUILayout.Button("Connect character and tile"))
            SetCharacterStartTile();

    }
    
    TileGenerator tg;
    
    /// <summary>
    /// Generate a grid, with or without a new parent
    /// </summary>
    /// <param name="CreateNew"></param>
    void GenerateGrid()
    {
        /*TileGenerator tg;*/

        if (parent == null)
            CreateNewParent();

        parent.transform.position = gridPosition;

        if (!parent.GetComponent<TileGenerator>())
            tg = parent.AddComponent<TileGenerator>();
        else
            tg = parent.GetComponent<TileGenerator>();

        //tg.GenerateGrid(tile, gridSize);
        GenerateGrid(tile, gridSize);
    }

    void ClearGrid()
    {
        for (int i = tg.transform.childCount; i >= tg.transform.childCount; i--)
        {
            if (tg.transform.childCount == 0)
                break;

            int c = Mathf.Clamp(i - 1, 0, tg.transform.childCount);
            DestroyImmediate(tg.transform.GetChild(c).gameObject);
        }
    }

    Vector2 DetermineTileSize(Bounds tileBounds)
    {
        return new Vector2((tileBounds.extents.x * 2) * 0.75f, (tileBounds.extents.z * 2));
    }

    public void GenerateGrid(GameObject tile, Vector2Int gridsize)
    {
        ClearGrid();
        Vector2 tileSize = DetermineTileSize(tile.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = tg.transform.position;

        for (int x = 0; x < gridsize.x; x++)
        {
            for (int y = 0; y < gridsize.y; y++)
            {
                position.x = tg.transform.position.x + tileSize.x * x;
                position.z = tg.transform.position.z + tileSize.y * y;

                position.z += UnevenRowOffset(x, tileSize.y);

                CreateTile(tile, position, new Vector2Int(x, y));
            }
        }
    }

    float UnevenRowOffset(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    void CreateTile(GameObject t, Vector3 pos, Vector2Int id)
    {
        // GameObject newTile = Instantiate(t.gameObject, pos, Quaternion.identity, tg.transform);
        GameObject newTile = PrefabUtility.InstantiatePrefab(t) as GameObject;
        newTile.transform.position = pos;
        newTile.transform.SetParent(tg.transform);
        
        newTile.name = "Tile " + id;

        Debug.Log("Created a tile!");
    }
    
    void CreateNewParent()
    {
        parent = new GameObject("Grid");
    }

    void SetCharacterStartTile()
    {
        GameObject character = Selection.activeTransform.gameObject;

        if (Physics.Raycast(character.transform.position, Vector3.down, out RaycastHit hit, 5f))
        {
            character.GetComponent<Player>().characterTile = hit.transform.GetComponent<Tile>();
            Debug.Log("Successfully coupled character with a tile");
        }

    }

    void CreateLadder()
    {
        GameObject[] tiles = Selection.gameObjects;
        if (tiles.Length != 2)
            return;

        if (tiles[0].GetComponent<Tile>() && tiles[1].GetComponent<Tile>())
        {
            tiles[0].GetComponent<Tile>().connectedTile = tiles[1].GetComponent<Tile>();
            tiles[1].GetComponent<Tile>().connectedTile = tiles[0].GetComponent<Tile>();
            Debug.Log($"Created ladder between tile {tiles[0].name} and {tiles[1].name}");
        }
    }

    void FocusOnGrid()
    {
        GameObject grid = Selection.activeGameObject;
        if (!grid.GetComponent<TileGenerator>())
            return;

        parent = grid;
        gridPosition = parent.transform.position;

        Debug.Log("Focusing on grid with name: " + parent.name);
    }
}
