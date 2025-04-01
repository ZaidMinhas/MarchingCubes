using System.Collections.Generic;
using UnityEngine;

public interface TerrainStrategy
{
    public float generate(float x, float y, float z);

    public void init();
}

public class PerlinNoiseStrategy : TerrainStrategy
{
    private float height;
    private float val;
    
    public PerlinNoiseStrategy(float height =  8)
    {
        val = Random.value*100;
        this.height = height;
    }
    public float generate(float x, float y, float z)
    { 
        return y - height * Mathf.PerlinNoise( (x + val)/8f, (z + val)/8f);
    }

    public void init()
    {
        
        //val = Random.value*100;
    }
}


public class SphereStrategy : TerrainStrategy
{
    private float radius;
    private float width;
    private Vector3 centre;
    private float cx, cy, cz;
    public SphereStrategy(float width = 1, float radius = 1)
    {
        cx = cy = cz = width / 2;
        this.radius = radius;
    }
    
    public float generate(float x, float y, float z)
    {
        float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz));
        return dist - radius;

    }

    public void init()
    {
    }
}

public class ElipsoideStrategy : TerrainStrategy
{
    private float radius;
    private float width;
    private Vector3 centre;
    private float cx, cy, cz;
    public ElipsoideStrategy(float width = 1, float radius = 1)
    {
        cx = cy = cz = width / 2;
        this.radius = radius;
    }
    
    public float generate(float x, float y, float z)
    {
        float dist = Mathf.Sqrt(2*(x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz));
        return dist - radius;

    }

    public void init()
    {
    }
}

public class GaussianStrategy : TerrainStrategy
{
    private float amplitude;
    private float sigma;
    private float threshold;
    private float cx, cy, cz;

    public GaussianStrategy(float width = 1, float amplitude = 1, float sigma = 1, float threshold = 0.5f)
    {
        cx = cy = cz = width / 2; // Center the Gaussian
        this.amplitude = amplitude;
        this.sigma = sigma;
        this.threshold = threshold;
    }

    public float generate(float x, float y, float z)
    {
        float distSq = (x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz);
        float value = amplitude * Mathf.Exp(-distSq / (2 * sigma * sigma));
        return value - threshold;
    }

    public void init()
    {
    }
}

public class TorusStrategy : TerrainStrategy
{
    private float majorRadius;
    private float minorRadius;
    private float cx, cy, cz;

    public TorusStrategy(float width = 1, float majorRadius = 2, float minorRadius = 0.5f)
    {
        cx = cy = cz = width / 2;
        this.majorRadius = majorRadius;
        this.minorRadius = minorRadius;
    }

    public float generate(float x, float y, float z)
    {
        float dx = x - cx;
        float dy = y - cy;
        float dz = z - cz;
        
        float qx = Mathf.Sqrt(dx * dx + dz * dz) - majorRadius;
        float dist = Mathf.Sqrt(qx * qx + dy * dy) - minorRadius;
        
        return dist;
    }

    public void init()
    {
    }
}


public class ConeStrategy : TerrainStrategy
{
    private float height;
    private Vector2 c;
    private float cx, cy, cz;

    public ConeStrategy(float width = 1, float angle = 45, float height = 2)
    {
        cx = cy = cz = width / 2;
        this.height = height;
        float rad = angle * Mathf.Deg2Rad;
        c = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
    }

    public float generate(float x, float y, float z)
    {
        float dx = x - cx;
        float dy = y - cy;
        float dz = z - cz;

        Vector2 q = height * new Vector2(c.x / c.y, -1.0f);
        Vector2 w = new Vector2(Mathf.Sqrt(dx * dx + dz * dz), dy);

        Vector2 a = w - q * Mathf.Clamp(Vector2.Dot(w, q) / Vector2.Dot(q, q), 0.0f, 1.0f);
        Vector2 b = w - q * new Vector2(Mathf.Clamp(w.x / q.x, 0.0f, 1.0f), 1.0f);
        float k = Mathf.Sign(q.y);
        float d = Mathf.Min(Vector2.Dot(a, a), Vector2.Dot(b, b));
        float s = Mathf.Max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
        return Mathf.Sqrt(d) * Mathf.Sign(s);
    }

    public void init()
    {
    }
}

public class OctahedronStrategy : TerrainStrategy
{
    private float size;
    private float cx, cy, cz;

    public OctahedronStrategy(float width = 1, float size = 1)
    {
        cx = cy = cz = width / 2;
        this.size = size;
    }

    public float generate(float x, float y, float z)
    {
        float dx = Mathf.Abs(x - cx);
        float dy = Mathf.Abs(y - cy);
        float dz = Mathf.Abs(z - cz);
        
        return (dx + dy + dz) - size;
    }

    public void init()
    {
    }
}

public class PyramidStrategy : TerrainStrategy
{
    private float height;
    private float baseSize;
    private float cx, cy, cz;

    public PyramidStrategy(float width = 1, float baseSize = 1, float height = 1)
    {
        cx = cy = cz = width / 2;
        this.baseSize = baseSize;
        this.height = height;
    }

    public float generate(float x, float y, float z)
    {
        float dx = Mathf.Abs(x - cx);
        float dz = Mathf.Abs(z - cz);
        float py = height - y;
        
        float baseLimit = baseSize * (py / height);
        float dist = Mathf.Max(dx - baseLimit, dz - baseLimit, -py);
        
        return dist;
    }

    public void init()
    {
    }
}

public class IcosahedronStrategy : TerrainStrategy
{
    private float size;
    private float cx, cy, cz;
    private static readonly Vector3[] Normals = {
        new Vector3( 1,  1,  1), new Vector3( 1,  1, -1), new Vector3( 1, -1,  1), new Vector3( 1, -1, -1),
        new Vector3(-1,  1,  1), new Vector3(-1,  1, -1), new Vector3(-1, -1,  1), new Vector3(-1, -1, -1),
        new Vector3( 0,  1.618f,  0.618f), new Vector3( 0,  1.618f, -0.618f), new Vector3( 0, -1.618f,  0.618f), new Vector3( 0, -1.618f, -0.618f),
        new Vector3( 0.618f,  0,  1.618f), new Vector3(-0.618f,  0,  1.618f), new Vector3( 0.618f,  0, -1.618f), new Vector3(-0.618f,  0, -1.618f),
        new Vector3( 1.618f,  0.618f,  0), new Vector3(-1.618f,  0.618f,  0), new Vector3( 1.618f, -0.618f,  0), new Vector3(-1.618f, -0.618f,  0)
    };

    public IcosahedronStrategy(float width = 1, float size = 1)
    {
        cx = cy = cz = width / 2;
        this.size = size;
    }

    public float generate(float x, float y, float z)
    {
        Vector3 p = new Vector3(x - cx, y - cy, z - cz);
        float maxDist = float.MinValue;
        
        foreach (var normal in Normals)
        {
            float dist = Vector3.Dot(p, normal.normalized) - size;
            maxDist = Mathf.Max(maxDist, dist);
        }
        
        return maxDist;
    }

    public void init()
    {
    }
}
public class HeartStrategy : TerrainStrategy
{
    private float size;
    private Vector3 center;

    public HeartStrategy(float width = 1)
    {
        size = width / 2; // Scale appropriately
        center = new Vector3(width / 2, width / 2, width / 2);
    }

    public float generate(float x, float y, float z)
    {
        // Normalize coordinates relative to the heart's center
        float nx = (x - center.x) / size;
        float ny = (y - center.y) / size;
        float nz = (z - center.z) / size;

        // Heart implicit equation
        float heartEq = Mathf.Pow(nx * nx + 9f / 4f * ny * ny + nz * nz - 1, 3) 
                        - nx * nx * Mathf.Pow(nz, 3) 
                        - 9f / 80f * ny * ny * Mathf.Pow(nz, 3);

        return heartEq;
    }

    public void init()
    {
        
    }
}
