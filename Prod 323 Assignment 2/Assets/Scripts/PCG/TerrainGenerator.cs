using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //[SerializeField] GameObject tree;
    //[SerializeField] GameObject center;
    public int depth = 20; // influence of the vertical y on the terrain
    public int width = 256;
    public int height = 256;   
    public int mapwidth = 500;
    public int mapheight = 500;
    
    float[,] heights;

    public float xScale = 0.3f;
    public float yScale = 0.3f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        //PlaceTree(terrain);
    }

   void PlaceTree(Terrain terrainData)
    {
        float x = mapwidth / 2;
        float z = mapheight / 2;

        Vector3 pos = new Vector3(x, 0, z);
        float y = terrainData.SampleHeight(pos);

        //tree.transform.position = new Vector3(x, y, z);
        //center.transform.position = new Vector3(x, y, z);

    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(mapwidth, depth, mapheight);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    public float CalculateHeight(int x, int y)
    {
        float xPerlinCoord = (float)x * xScale + offsetX;
        float yPerlinCoord = (float)y * yScale + offsetY;
        return Mathf.PerlinNoise(xPerlinCoord, yPerlinCoord);
    }
}