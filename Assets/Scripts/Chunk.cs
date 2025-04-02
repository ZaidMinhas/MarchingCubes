using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    
    
    public Vector3Int chunkPosition;
    private bool flatShaded = true;
    private bool smoothTerrain = false;
    
    int width => GameManager.Instance.width;
    int height => GameManager.Instance.width;
    int dim => GameManager.Instance.dim;
    float terrainSurface => GameManager.Instance.terrainSurface;
    
    //Obj properties
    public GameObject chunkObject;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public VoxelTerrain terrain;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    

    public Chunk(Vector3Int _chunkPosition)
    {
        chunkPosition = _chunkPosition;
        
        chunkObject = new GameObject { transform = { position = chunkPosition } };
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        var meshRenderer1 = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer1.material = Resources.Load<Material>("Matt");
        
        
        terrain = new VoxelTerrain(terrainSurface, width, height )
        {
            chunk = this
        };
        terrain.resetTerrain();
        terrain.generate();
        
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

        if (chunkPosition.z < dim )
        {
            int z = width;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MarchCube(new Vector3Int(x, y, z));
                }
            }    
        }
        
        if (chunkPosition.x < dim )
        {
            int x = width;
            for (int z = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MarchCube(new Vector3Int(x, y, z));
                }
            }    
        }
        

        Init();
    }
    
    
    void MarchCube (Vector3Int position) {
        
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            
            Vector3Int cornerPos = position + CubesData.CornerTable[i];
            if (cornerPos.z > width)
            {
                cornerPos = new Vector3Int(cornerPos.x, cornerPos.y, 0);
                Vector2Int chunkIndex = new Vector2Int(chunkPosition.x / width, (chunkPosition.z) / width + 1);
                
                VoxelTerrain nextTerrain = GameManager.Instance.chunks[chunkIndex.x, chunkIndex.y].terrain;
                cube[i] = nextTerrain.getValue(cornerPos);
            }
            else if (cornerPos.x > width)
            {
                cornerPos = new Vector3Int(0, cornerPos.y, cornerPos.z);
                
                VoxelTerrain nextTerrain = GameManager.Instance.chunks[(chunkPosition.x)/width + 1, (chunkPosition.z)/width].terrain;
                cube[i] = nextTerrain.getValue(cornerPos);
            }
            else
            {
                cube[i] = terrain.getValue(cornerPos);    
            }
            
        }
        int configIndex = getConfigIdx(cube);
        if (configIndex is 0 or 255)
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
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }



    public void switchFlatShading()
    {
        flatShaded = !flatShaded;
        CreateMeshData();
    }

    public void switchSmoothing()
    {
        smoothTerrain = !smoothTerrain;
        CreateMeshData();
    }

    public void resetTerrain()
    {
        terrain.resetTerrain();
        terrain.generate();
        CreateMeshData();
    }
    
}
