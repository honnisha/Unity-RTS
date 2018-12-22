using UnityEngine;
using System.Collections;

public class LightBeamsControlScript : MonoBehaviour
{
    public GameObject SourceObject;
    public GameObject TargetObject;

    

    public GameObject RayPrefab;
    
	// Use this for initialization

    public Color RayColor;


    //public float AlphaCurve = 0;
    public Vector3 PositionRange = Vector3.zero;

    public float RadiusA;
    public float RadiusB;

    public float WidthA;
    public float WidthB;

    public float FadeSpeed = 1.0f;

    public int NumRays = 10;
    int Spawned = 0;
    float TimeToSpawnAll = 3.0f;
    float spawnInterval = 1.0f;
    float currentCountdown = 0f;


    RayBehavior[] rays;

    void setRayValues(RayBehavior ray)
    {
        ray.PositionRange = PositionRange;

        ray.BeginLocation = SourceObject;
        ray.EndLocation = TargetObject;

        ray.BeginColor = RayColor;
        ray.EndColor = RayColor;

        ray.WidthA = WidthA;
        ray.WidthB = WidthB;

        ray.RadiusA = RadiusA;
        ray.RadiusB = RadiusB;

        ray.FadeSpeed = FadeSpeed;

        ray.ResetRay();
    }

    
    
    void SpawnRay()
    {
        if (Spawned < NumRays)
        {
            rays[Spawned] = (GameObject.Instantiate(RayPrefab) as GameObject).GetComponent<RayBehavior>();
            setRayValues(rays[Spawned]);
        }

        Spawned += 1;

        currentCountdown = spawnInterval;
    }

	void Start () 
    {
        spawnInterval = TimeToSpawnAll / NumRays;

        rays = new RayBehavior[NumRays];

        SpawnRay();
	}

	
	// Update is called once per frame
	void Update () 
    {
        if (Spawned < NumRays)
        {
            if (currentCountdown <= 0)
            {
                SpawnRay();
            }

            currentCountdown -= Time.deltaTime;
        }
        
	}
}
