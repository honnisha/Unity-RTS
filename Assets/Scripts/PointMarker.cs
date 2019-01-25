using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GangaGame
{
    public class PointMarker : MonoBehaviour {

        public enum MarkerType { Point, Flag, Arrow };
        public GameObject flag;
        public GameObject arrow;
    
	    // Use this for initialization

        private void Update()
        {
            if (arrow.activeSelf)
                arrow.transform.LookAt(new Vector3(Camera.main.transform.position.x, arrow.transform.position.y, Camera.main.transform.position.z));
        }

        public void SetMarker(Color color, MarkerType markerType, float timer = 0.0f)
        {
            GetComponentInChildren<Projector>().material.color = color;

            if (markerType == MarkerType.Arrow)
            {
                arrow.GetComponentInChildren<Renderer>().material.color = color;
                arrow.transform.LookAt(new Vector3(Camera.main.transform.position.x, arrow.transform.position.y, Camera.main.transform.position.z));
                arrow.SetActive(true);
            }
            else
                arrow.SetActive(false);

            if (markerType == MarkerType.Flag)
                flag.SetActive(true);
            else
                flag.SetActive(false);

            if (timer > 0)
                Destroy(gameObject, timer);
        }
    }
}

