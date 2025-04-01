using System.Collections.Generic;
using UnityEngine;

public class Marching : MonoBehaviour {

    public bool smoothTerrain;
    public bool flatShaded;

    public float terrainSurface;

    public int width
    {
        get
        {
            return GameManager.Instance.width; 
        }
    }
    public int height
    {
        get
        {
            return GameManager.Instance.height; 
        }
    }
    
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    public VoxelTerrain terrain;
    
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    public void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        
        transform.tag = "Terrain";
        terrain = new VoxelTerrain(terrainSurface, width, height );
        terrain.resetTerrain();
        terrain.generate();
        CreateMeshData();

    }

    public void CreateMeshData() {

        ClearMeshData();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < width; z++) {
                    MarchCube(new Vector3Int(x, y, z));
                }
            }
        }
        Init();
    }
    
    void MarchCube (Vector3Int position) {
        
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++) {
            cube[i] = terrain.getValue(position + CubesData.CornerTable[i]);
        }
        int configIndex = getConfigIdx(cube);
        if (configIndex == 0 || configIndex == 255)
            return;
        int edgeIndex = 0;
        while (true)
        {
            int indice = CubesData.TriangleTable[configIndex, edgeIndex];
            if (indice == -1)
                return;
                
            Vector3 vert1 = position + CubesData.CornerTable[CubesData.EdgeIndexes[indice, 0]];
            Vector3 vert2 = position + CubesData.CornerTable[CubesData.EdgeIndexes[indice, 1]];

            Vector3 vertPosition;
            if (smoothTerrain) {
                float vert1Sample = cube[CubesData.EdgeIndexes[indice, 0]];
                float vert2Sample = cube[CubesData.EdgeIndexes[indice, 1]];
                
                float difference = vert2Sample - vert1Sample;
                if (difference == 0)
                    difference = terrainSurface;
                else
                    difference = (terrainSurface - vert1Sample) / difference;
                
                vertPosition = vert1 + ((vert2 - vert1) * difference);

            } else {
                
                vertPosition = (vert1 + vert2) / 2f;
            }

            if (flatShaded) {

                vertices.Add(vertPosition);
                triangles.Add(vertices.Count - 1);

            } else
                triangles.Add(VertForIndice(vertPosition));

            edgeIndex++;
        }
        
    }

    int getConfigIdx (float[] cube) {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++) {
            if (cube[i] > terrainSurface)
                configurationIndex |= 1 << i;
        }
        return configurationIndex;
    }

    public void PlaceTerrain (Vector3 pos, int radius) {
        Vector3Int v3Int = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        terrain.setCloudValues(v3Int, 0f, radius);
        CreateMeshData();

    }

    public void RemoveTerrain (Vector3 pos, int radius) {
        Vector3Int v3Int = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        terrain.setCloudValues(v3Int, 1f, radius);
        CreateMeshData();
    }   
    int VertForIndice (Vector3 vert) {
        
        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i] == vert)
                return i;
        }
        vertices.Add(vert);
        return vertices.Count - 1;
    }

    void ClearMeshData () {

        vertices.Clear();
        triangles.Clear();

    }

    void Init () {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

}