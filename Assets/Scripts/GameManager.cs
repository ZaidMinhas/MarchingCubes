using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    
    public static GameManager Instance;
    
    
    public int width;
    public int height;
    public float terrainSurface = 0.5f;

    public static int dim => CubesData.dim;
    public Chunk[,] chunks;
    
    
    public void Awake()
    {
        Instance = this;
        LevelManager.setStrategy();
    }

    public void Start()
    {
        chunks = new Chunk[dim, dim];
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                
                Vector3Int chunkPos = new Vector3Int(i, 0, j)*width;
                
                chunks[i, j] = new Chunk(chunkPos);
            }
        }
        
        Init();
    }


    public void smoothTerrains()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.switchSmoothing();
            
        }
    }

    public void flatShadTerrains()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.switchFlatShading();
            
        }
    }

    public void resetTerrains()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.resetTerrain();
            chunk.ClearMeshData();
        }
    }

    public  void placeTerrain(Vector3 hitPos, int radius)
    {
        Vector3Int chunkPos = WorldToChunkPos(hitPos); 
        Chunk chunk = chunks[(int)chunkPos.x, (int)chunkPos.z];
        chunk.PlaceTerrain(WorldToTerrainPos(hitPos), radius+2);
    }
    
    public  void removeTerrain(Vector3 hitPos, int radius)
    {
        Vector3Int chunkPos = WorldToChunkPos(hitPos); 
        Chunk chunk = chunks[(int)chunkPos.x, (int)chunkPos.z];
        chunk.RemoveTerrain(WorldToTerrainPos(hitPos), radius+2);
    }

    Vector3Int WorldToChunkPos(Vector3 pos)
    {
        pos = pos / width;
        return new Vector3Int((int)pos.x, 0, (int)pos.z);
    }

    Vector3Int WorldToTerrainPos(Vector3 pos)
    {
        Vector3Int posInt = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        Vector3Int chunkPos = WorldToChunkPos(posInt);
        return posInt - chunkPos* width;
    }

    void Init()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.CreateMeshData();    
        }
        
    }
}
