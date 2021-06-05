using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;

    public float scale = 5;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public float xMultiplier = 0.3f;
    public float yMultiplier = 0.3f;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    private void Update()
    {
        GetComponent<Renderer>().material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);

            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        float sample = CalculateHeight(x, y);

        return new Color(sample, sample, sample);
    }

    public float CalculateHeight(int x, int y)
    {
        float xPerlinCoord = (float)x * xMultiplier + offsetX;
        float yPerlinCoord = (float)y * yMultiplier + offsetY;

        return Mathf.PerlinNoise(xPerlinCoord, yPerlinCoord);
    }


}
