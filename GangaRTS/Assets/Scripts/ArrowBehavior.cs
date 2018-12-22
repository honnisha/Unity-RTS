using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour {

    public GameObject target;
    public GameObject sender;
    public float damage;
    public float speed = 10.0f;
    public float distanceToDamage = 1.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target != null)
        {
            BaseBehavior targetBaseBehavior = target.transform.gameObject.GetComponent<BaseBehavior>();
            if (targetBaseBehavior.live)
            {
                transform.LookAt(target.transform.position);
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);
                if (dist <= distanceToDamage)
                {
                    targetBaseBehavior.TakeDamage(damage, sender);
                    Destroy(gameObject);
                }
            }
            else
                Destroy(gameObject);
        }
    }
}
