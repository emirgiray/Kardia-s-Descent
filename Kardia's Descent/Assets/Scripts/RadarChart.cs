
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarChart : MonoBehaviour {

    [SerializeField] private Material radarMaterial;
    [SerializeField] private Texture2D radarTexture2D;

    private CharacterStats characterStats; 
    
    private CanvasRenderer radarMeshCanvasRenderer;

    private void Awake() {
        radarMeshCanvasRenderer = transform.Find("radarMesh").GetComponent<CanvasRenderer>();
    }

    public void SetStats(CharacterStats stats) {
        this.characterStats = stats;
        UpdateStatsVisual();
    }

    private void Stats_OnStatsChanged(object sender, System.EventArgs e) {
        UpdateStatsVisual();
    }

    private void UpdateStatsVisual() {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3 * 5];

        float angleIncrement = 360f / 5;
        float radarChartSize = 145f;
        
        float total = characterStats.Strength + characterStats.Dexterity + characterStats.Constitution + characterStats.Aiming;
        
        Vector3 attackVertex = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * (float)characterStats.Strength / 10;
        int attackVertexIndex = 1;
        Vector3 defenceVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * (float)characterStats.Dexterity / 10;
        int defenceVertexIndex = 2;
        Vector3 speedVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * (float)characterStats.Constitution / 10;
        int speedVertexIndex = 3;
        Vector3 manaVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * (float)characterStats.Aiming / 10;
        int manaVertexIndex = 4;
        Vector3 healthVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * (float)total/4 / 10;
        int healthVertexIndex = 5;

        vertices[0] = Vector3.zero;
        vertices[attackVertexIndex]  = attackVertex;
        vertices[defenceVertexIndex] = defenceVertex;
        vertices[speedVertexIndex]   = speedVertex;
        vertices[manaVertexIndex]    = manaVertex;
        vertices[healthVertexIndex]  = healthVertex;

        uv[0]                   = Vector2.zero;
        uv[attackVertexIndex]   = Vector2.one;
        uv[defenceVertexIndex]  = Vector2.one;
        uv[speedVertexIndex]    = Vector2.one;
        uv[manaVertexIndex]     = Vector2.one;
        uv[healthVertexIndex]   = Vector2.one;

        triangles[0] = 0;
        triangles[1] = attackVertexIndex;
        triangles[2] = defenceVertexIndex;

        triangles[3] = 0;
        triangles[4] = defenceVertexIndex;
        triangles[5] = speedVertexIndex;

        triangles[6] = 0;
        triangles[7] = speedVertexIndex;
        triangles[8] = manaVertexIndex;

        triangles[9]  = 0;
        triangles[10] = manaVertexIndex;
        triangles[11] = healthVertexIndex;

        triangles[12] = 0;
        triangles[13] = healthVertexIndex;
        triangles[14] = attackVertexIndex;


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshCanvasRenderer.SetMesh(mesh);
        radarMeshCanvasRenderer.SetMaterial(radarMaterial, radarTexture2D);
    }

}
