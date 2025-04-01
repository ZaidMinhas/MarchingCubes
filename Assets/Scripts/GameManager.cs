using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public Marching marching;
    public static GameManager Instance;
    public TerrainStrategy[] terrainStrategies;
    
    public int width;
    public int height;
    private int idx = 0;
    public void Awake()
    {
        Instance = this;

        terrainStrategies = new TerrainStrategy[]
        {
            new SphereStrategy(width, width / 3),
            new TorusStrategy(width, width/4, 2),
            new ConeStrategy(width, 45, height),
            new OctahedronStrategy(width, 10),
            new PyramidStrategy(width, height/2, height/2),
            new IcosahedronStrategy(width, width*0.4f),
            new PerlinNoiseStrategy()
        };

    }

    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            marching.smoothTerrain = !marching.smoothTerrain;
            marching.CreateMeshData();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            marching.flatShaded = !marching.flatShaded;
            marching.CreateMeshData();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            VoxelTerrain terrain = marching.terrain;
            terrain.resetTerrain();
            terrain.generate();
            marching.CreateMeshData();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            VoxelTerrain terrain = marching.terrain;
            terrain.terrainStrategy = terrainStrategies[idx];
            idx = (idx+1) % terrainStrategies.Length;
            terrain.generate();
            marching.CreateMeshData();
        }
    }
    
    
}
