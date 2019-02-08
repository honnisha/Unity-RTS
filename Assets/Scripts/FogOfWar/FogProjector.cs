    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogProjector : MonoBehaviour
{
    public RenderTexture fogTexture;
    Terrain terrain = null;

    void OnEnable()
    {
    }

    public void UpdateFog()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;

        Vector3 newPosition = terrain.terrainData.size / 2;
        transform.position = new Vector3(newPosition.x, 50.0f, newPosition.z);

        float newSize = terrain.terrainData.size.x / 2;
        // projector.orthographicSize = newSize;
        Light fogLight = GetComponentInChildren<Light>();
        fogLight.cookieSize = terrain.terrainData.size.x;

        GetComponentInChildren<Camera>().orthographicSize = newSize;
    }

    public void Update()
    {
        Terrain t = Terrain.activeTerrain;
        if (t != null && t.terrainData != null)
        {
            if (transform.position.x != (t.terrainData.size / 2).x)
                UpdateFog();
        }
    }
}