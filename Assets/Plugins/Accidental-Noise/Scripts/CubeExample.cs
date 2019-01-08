using UnityEngine;
using System.Collections;
using AccidentalNoise;

public class CubeExample : MonoBehaviour {

	public GameObject duplicateObj;
	public int height = 256;
	public int width = 256;
	public float threshold = 0.5f;
	public PresetType preset;

	void Start () {

		// METHOD A
		// Note: Very CPU heavy
		// GenerateInOneFrame(); 

		// or 

		// METHOD B
		StartCoroutine(GenerateStream());
	}

	IEnumerator GenerateStream(){

		float scale = duplicateObj.transform.localScale.x;

		ModuleBase combinedTerrain = TerrainPresets.GetPreset(preset);
		SMappingRanges ranges = new SMappingRanges();


		Camera.main.transform.position = new Vector3(width / 2.0f * scale, height / 2.0f * scale, -15.0f);


		yield return null;

		for (int x = 0; x < width; x++){
			yield return null;

			for(int y = 0; y < height; y++){
				double p = (double)x / (double)width;
				double q = (double)y / (double)height;
				double nx, ny = 0.0;
				nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
				ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

				double val = combinedTerrain.Get(nx * 3, ny * 3);
				
				if(val > threshold){
					Vector3 pos = new Vector3(x,height - y,0) * scale;
					GameObject g = GameObject.Instantiate(duplicateObj, pos , Quaternion.identity) as GameObject;
					g.name = "cube-"+x+"-"+y;
				}
			}
		}		

	}

	
	void GenerateInOneFrame(){

		Texture2D texture = new Texture2D(width,height);
		GetComponent<Renderer>().material.mainTexture = texture;

		ModuleBase combinedTerrain = TerrainPresets.GetPreset(preset);
		SMappingRanges ranges = new SMappingRanges();


		for (int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				double p = (double)x / (double)width;
				double q = (double)y / (double)height;
				double nx, ny = 0.0;
				nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
				ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

				double val = combinedTerrain.Get(nx * 3, ny * 3);
				if(val == 0) texture.SetPixel(x,y,Color.black);
				else texture.SetPixel(x,y,Color.white);

			}
		}

		texture.Apply();
	}


}
