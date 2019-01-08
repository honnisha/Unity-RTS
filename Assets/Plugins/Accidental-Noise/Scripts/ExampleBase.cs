using UnityEngine;
using System.Collections;
using AccidentalNoise;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class ExampleBase : MonoBehaviour {

	public int height = 256;
	public int width = 256;
	public double scale = 1.0;
	
	protected void GenerateInOneFrame(ModuleBase moduleBase){

		Texture2D texture = new Texture2D(width,height);
		GetComponent<Renderer>().material.mainTexture = texture;
		SMappingRanges ranges = new SMappingRanges();


		for (int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				double p = (double)x / (double)width;
				double q = (double)y / (double)height;
				double nx, ny = 0.0;
				nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
				ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

				float val = (float) moduleBase.Get(nx * scale, ny * scale);
				texture.SetPixel(x,y,new Color(val,val,val));
			}
		}

		texture.Apply();
	}



	public static double DoubleLerp(double start, double end, double amount)
	{
		double difference = end - start;
		double adjusted = difference * amount;
		return start + adjusted;
	}

	public static Color ColorLerp(Color colour, Color to, double amount)
	{
		// start colours as lerp-able floats
		double sr = colour.r, sg = colour.g, sb = colour.b;

		// end colours as lerp-able floats
		double er = to.r, eg = to.g, eb = to.b;

		// lerp the colours to get the difference
		float r = (float) (DoubleLerp(sr, er, amount) / 255.0) ,
			 g = (float) (DoubleLerp(sg, eg, amount) / 255.0) ,
			 b = (float) (DoubleLerp(sb, eb, amount) / 255.0) ;

		// return the new colour
		return new Color(r, g, b);
	}

}
