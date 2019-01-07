using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {
        Terrain t = Terrain.activeTerrain;
        int sizeX = t.terrainData.alphamapWidth;
        int sizeY = t.terrainData.alphamapHeight;
        float[,,] map = new float[sizeX, sizeY, 8];
        int[][,] grassLayers = new int[][,] { new int[sizeX, sizeY], new int[sizeX, sizeY], new int[sizeX, sizeY] };
        for (int y = 0; y < t.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < t.terrainData.detailWidth; x++)
            {
                map[x, y, 0] = 1;
                grassLayers[0][x, y] = 2;
                grassLayers[1][x, y] = 1;
                grassLayers[2][x, y] = 0;
            }
        }
        t.terrainData.SetAlphamaps(0, 0, map);
        for (int i = 0; i <= grassLayers.Length; i++)
            t.terrainData.SetDetailLayer(0, 0, i, grassLayers[i]);
    }
}
