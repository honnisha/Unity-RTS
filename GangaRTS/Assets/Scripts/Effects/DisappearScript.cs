using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearScript : MonoBehaviour {

    private float disappearTimer;
    private float timerToDisappear = 0.2f;

    private float opacity = 1.0f;

    // Use this for initialization
    void Start () {
        disappearTimer = 30.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0.0f)
        {
            timerToDisappear -= Time.deltaTime;
            if (timerToDisappear <= 0.0f)
            {
                timerToDisappear = 0.2f;
                foreach (var material in GetComponent<Renderer>().materials)
                {
                    if (opacity <= 0)
                        Destroy(gameObject);

                    opacity -= 0.05f;
                    Color newColor = new Color(1.0f, 1.0f, 1.0f, opacity);
                    material.SetFloat("_Mode", 2);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    material.SetColor("_Color", newColor);
                }
            }
        }

    }
}
