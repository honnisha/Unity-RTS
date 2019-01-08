using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccidentalNoise;

public class TerrainGenerator : MonoBehaviour
{
    public FractalType fractalType = FractalType.MULTI;
    public BasisTypes basisType = BasisTypes.SIMPLEX;
    public InterpTypes interpType = InterpTypes.QUINTIC;

    public int octaves = 3;
    public double frequency = 2.0;
    public double lacunarity = 2.0;
    public uint seed = 1;

    public Texture2D mapTexture;

    public bool generate = false;

    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (generate)
        {
            generate = false;
            Generate();
        }
    }

    public ModuleBase GetFractal()
    {
        Fractal ground_shape_fractal = new Fractal(fractalType,
                                            basisType,
                                            interpType,
                                            octaves,
                                            frequency,
                                            seed);
        ground_shape_fractal.SetLacunarity(lacunarity);
        return ground_shape_fractal as ModuleBase;
    }

    private float KeepPositive(float value)
    {
        if (value < 0) return 0.0f;
        if (value > 1) return 1.0f;
        return value;
    }

    public void Generate()
    {
        Terrain t = Terrain.activeTerrain;
        int sizeX = t.terrainData.alphamapWidth;
        int sizeY = t.terrainData.alphamapHeight;
        if (t.terrainData.detailHeight != t.terrainData.alphamapWidth)
            Debug.Log("detailHeight and alphamapWidth must be equal");

        ModuleBase moduleBase = GetFractal();
        mapTexture = new Texture2D(sizeX, sizeY);
        SMappingRanges ranges = new SMappingRanges();

        float[,,] map = new float[sizeX, sizeY, 7];
        int[][,] grassLayers = new int[][,] { new int[sizeX, sizeY], new int[sizeX, sizeY], new int[sizeX, sizeY], new int[sizeX, sizeY] };
        for (int y = 0; y < t.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < t.terrainData.detailWidth; x++)
            {
                double p = (double)x / (double)sizeX;
                double q = (double)y / (double)sizeY;
                double nx, ny = 0.0;
                nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                float val = (float)moduleBase.Get(nx * scale, ny * scale);

                float textureScale = (val + 1.0f);
                if (textureScale > 0.9f)
                    mapTexture.SetPixel(x, y, new Color(0, 1, 0, 1));
                else
                    mapTexture.SetPixel(x, y, new Color(0, 0.4f, 0, 1));

                float grassValue = 0.0f;
                if(textureScale > 0.75)
                    grassValue = KeepPositive((textureScale - 0.75f) * 4.0f);
                map[x, y, 1] = grassValue;

                float stoneValue = 0.0f;
                if (textureScale < 0.2)
                {
                    stoneValue = 1.0f - KeepPositive(textureScale * 5.0f);
                }
                map[x, y, 3] = stoneValue;

                map[x, y, 0] = KeepPositive(textureScale + 0.2f) - grassValue - stoneValue / 2.0f;
                map[x, y, 2] = KeepPositive(1.0f - textureScale - 0.2f) - grassValue - stoneValue / 2.0f;

                if (textureScale > 0.75f)
                {
                    grassLayers[3][x, y] = 1;
                }
                if (textureScale > 0.6f)
                {
                    grassLayers[2][x, y] = 2;
                }
                else if (textureScale > 0.3f)
                {
                    grassLayers[2][x, y] = 1;
                    grassLayers[1][x, y] = 1;
                }
                else if (textureScale > 0.1f)
                    grassLayers[1][x, y] = 1;
            }
        }
        mapTexture.Apply();
        t.terrainData.SetAlphamaps(0, 0, map);
        for (int i = 0; i < grassLayers.Length; i++)
            t.terrainData.SetDetailLayer(0, 0, i, grassLayers[i]);
    }
    public double scale = 1.0;

    public static double DoubleLerp(double start, double end, double amount)
    {
        double difference = end - start;
        double adjusted = difference * amount;
        return start + adjusted;
    }

    public static Color ColorLerp(Color colour, Color to, double amount)
    {
        // start colours as lerp-able floats
        double sr = colour.r, sg = colour.g, sb = colour.b;

        // end colours as lerp-able floats
        double er = to.r, eg = to.g, eb = to.b;

        // lerp the colours to get the difference
        float r = (float)(DoubleLerp(sr, er, amount) / 255.0),
             g = (float)(DoubleLerp(sg, eg, amount) / 255.0),
             b = (float)(DoubleLerp(sb, eb, amount) / 255.0);

        // return the new colour
        return new Color(r, g, b);
    }
}