using UnityEngine;
using System.Collections;

public class RayBehavior : MonoBehaviour 
{
    public GameObject BeginLocation;
    public GameObject EndLocation;

    public Color BeginColor = Color.white;
    public Color EndColor = Color.white;

    public Vector3 PositionRange;


    public float WidthA = 1.0f;
    public float WidthB = 1.0f;

    public float RadiusA = 1.0f;
    public float RadiusB = 1.0f;

    //public float Offset = 1.0f;

    private LineRenderer Line;
    private Animation Anim;

    private bool changed = true;
    private Vector3 Offset;


    public float AlphaCurve;

    public float FadeSpeed = 1.0f;


	// Use this for initialization
    public void ResetRay()
    {
        Offset = new Vector3( Random.Range(-PositionRange.x, PositionRange.x), 
            Random.Range(-PositionRange.y, PositionRange.y),
            Random.Range(-PositionRange.z, PositionRange.z)
            );

        


        changed = true;
    }

    public void UpdateLineData()
    {
        Line.SetPosition(0, BeginLocation.transform.position + (Offset * RadiusA));
        Line.SetPosition(1, EndLocation.transform.position + (Offset * RadiusB));
        
        Line.SetWidth(WidthA, WidthB);
    }


	void Start () 
    {
        Line = GetComponent<LineRenderer>();
        Anim = GetComponent<Animation>();


        Anim["RayAlphaCurve"].speed = FadeSpeed;        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (changed)
        {
            changed = false;
            UpdateLineData();
        }

        
        Line.SetColors(new Color(BeginColor.r, BeginColor.g, BeginColor.b, AlphaCurve),
            new Color(EndColor.r, EndColor.g, EndColor.b, AlphaCurve));
        

        
        //Line.renderer.material.color = new Color(1, 1, 1, AlphaCurve);
	
	}
}
