using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonBehavior : MonoBehaviour
{
    public GameObject holderObject;
    private float oldRotationY;
    private float maxAngle = 80.0f;
    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (holderObject != null)
        {
            float rotateChange = (holderObject.transform.rotation.eulerAngles.y - oldRotationY) * -1.5f;
            if (rotateChange > 20.0f || rotateChange < -20.0f)
                rotateChange = 0.0f;

            float newRotate = transform.localRotation.eulerAngles.y + rotateChange;

            if (newRotate < 0)
                newRotate = 360 + newRotate;

            if (newRotate > maxAngle && newRotate < 180.0f)
                newRotate = maxAngle;
            else if (newRotate < 360 - maxAngle && newRotate > 180.0f)
                newRotate = 360 - maxAngle;

            Vector3 curMove = holderObject.transform.position - previousPosition;
            float curSpeed = curMove.magnitude / Time.deltaTime;
            previousPosition = holderObject.transform.position;
            if (newRotate < 180.0f)
            {
                newRotate -= curSpeed;
                if (newRotate < 0) newRotate = 0.0f;
            }
            else if (newRotate > 180.0f)
            {
                newRotate += curSpeed;
                if (newRotate > 360.0f) newRotate = 360.0f;
            }

            transform.localRotation = Quaternion.Euler(new Vector3(0, newRotate, 0));
            oldRotationY = holderObject.transform.rotation.eulerAngles.y;
        }
    }
}
