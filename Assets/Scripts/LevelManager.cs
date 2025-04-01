

using UnityEngine;

public static class LevelManager
{
    public  static int idx;
    private static int width => GameManager.Instance.width;
    private static int height => GameManager.Instance.height;


    private static readonly TerrainStrategy[] terrainStrategies =  {
        new PerlinNoiseStrategy(),
        new SphereStrategy(width, width / 3),
        new TorusStrategy(width, width/4, 2),
        new ConeStrategy(width, 45, height),
        new OctahedronStrategy(width, 10),
        new PyramidStrategy(width, height/2, height/2),
        new IcosahedronStrategy(width, width*0.4f),
        
    };

    private static TerrainStrategy getStrategy()
    {
        return terrainStrategies[idx];
    }
    
    public static void setStrategy()
    {
        VoxelTerrain.terrainStrategy = getStrategy();

    }
    
    public static void setNextStrategy()
    {
        idx = (idx+1) % terrainStrategies.Length;
        VoxelTerrain.terrainStrategy = getStrategy();

    }

    public static float generate(float x, float y, float z)
    {
        return getStrategy().generate(x,y,z);
    }
    
    
}
