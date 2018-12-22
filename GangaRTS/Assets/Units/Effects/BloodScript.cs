using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScript : MonoBehaviour
{
    public float destroyAfter = 2.0f;
    public float disableEmissionAfter = 1.0f;

	// Use this for initialization
	void Start ()
    {
        transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Destroy(gameObject, destroyAfter);
    }

    // Update is called once per frame
    void Update()
    {
        disableEmissionAfter -= Time.deltaTime;
        if (disableEmissionAfter <= 0.0f)
        {
            ParticleSystem bloodParticle = transform.gameObject.GetComponent<ParticleSystem>();
            bloodParticle.enableEmission = false;
        }
    }
}
