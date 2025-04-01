using UnityEngine;

public class VoxelTerrain
{
    float[,,] terrain;
    
    public Chunk chunk;
    private float terrainSurface;
    private readonly int width;
    private readonly int height;
    public static TerrainStrategy terrainStrategy;
    
    public VoxelTerrain(float terrainSurface, int width, int height)
    {
        this.terrainSurface = terrainSurface;
        this.width = width;
        this.height = height;
    }

    public void resetTerrain()
    {
        terrain = new float[width + 1, height + 1, width + 1];
    }
    
    public void generate () {
        for (int x = 0; x < width + 1; x++) {
            for (int z = 0; z < width + 1; z++) {
                for (int y = 0; y < height + 1; y++)
                {
                    if (LevelManager.idx == 0)
                    {
                        terrain[x, y, z] = terrainStrategy.generate(x+chunk.chunkPosition.x,y, z+chunk.chunkPosition.z);    
                    }
                    else
                    {
                        terrain[x, y, z] = terrainStrategy.generate(x, y, z);    
                    }
                    
                }
            }
        }
    }

    public void setValue(Vector3Int pos, float value)
    {
        terrain[pos.x, pos.y, pos.z] = value;
    }

    public float getValue(Vector3Int pos)
    {
        return terrain[pos.x, pos.y, pos.z];
    }


    public void setCloudValues(Vector3Int pos, float targetValue, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3Int point = pos + new Vector3Int(x, y, z);
                    if (point.x < 0 || point.x > width || point.y < 0 || point.y > height || point.z < 0 || point.z > width)
                        continue;

                    float distance = Vector3.Distance(point, pos);
                    if (distance <= radius)
                    {
                        float falloff = Mathf.Clamp01(1 - (distance / radius));
                        float newValue = Mathf.Lerp(getValue(point), targetValue, falloff);
                        setValue(point, newValue);
                    }
                }
            }
        }
    }
}