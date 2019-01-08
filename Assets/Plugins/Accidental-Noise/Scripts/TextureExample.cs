using UnityEngine;
using System.Collections;
using AccidentalNoise;

public class TextureExample : ExampleBase {

	public PresetType preset;

	void Start () {		
		// METHOD A
		// Note: Very CPU heavy
		// GenerateInOneFrame(TerrainPresets.GetPreset(preset)); 

		// or 

		// METHOD B
		StartCoroutine(GenerateStream());		
	}

	IEnumerator GenerateStream(){
		Texture2D texture = new Texture2D(width,height);
		GetComponent<Renderer>().material.mainTexture = texture;

		ModuleBase combinedTerrain = TerrainPresets.GetPreset(preset);
		SMappingRanges ranges = new SMappingRanges();

		yield return null;

		for (int x = 0; x < width; x++){
			yield return null;

			for(int y = 0; y < height; y++){
				double p = (double)x / (double)width;
				double q = (double)y / (double)height;
				double nx, ny = 0.0;
				nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
				ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

				float val = (float) combinedTerrain.Get(nx * scale, ny * scale);
				texture.SetPixel(x,y,new Color(val,val,val));
			}
			texture.Apply();
		}		

	}

}
