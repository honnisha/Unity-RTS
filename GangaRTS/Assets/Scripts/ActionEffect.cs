using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffect : MonoBehaviour {

    public GameObject actionObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void activate(GameObject sender, GameObject target, float damage)
    {
        GameObject createdActionObject = Instantiate(actionObject, gameObject.transform.position, Quaternion.identity);
        //arrow.transform.parent = gameObject.transform;
        ArrowBehavior unitArrowBehavior = createdActionObject.transform.gameObject.GetComponent<ArrowBehavior>();
        unitArrowBehavior.sender = sender;
        unitArrowBehavior.target = target;
        unitArrowBehavior.damage = damage;
    }
}
