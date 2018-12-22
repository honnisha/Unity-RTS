using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Checks if the difference between two textures is above a threshold (channelwise).
	/// If it is, src2 is used. The bigger the absolute difference, the more apparent src2 is.
	/// </summary>
	
	public class AbsThreshold: TextureNode{
		
		/// <summary>The threshold.</summary>
		public TextureNode Threshold{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>Gain applied when above the threshold.</summary>
		public TextureNode Gain{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public AbsThreshold():base(4){}
		
		public AbsThreshold(TextureNode src1,TextureNode src2,TextureNode threshold,TextureNode gain):base(4){
			SourceModule1=src1;
			SourceModule2=src2;
			Threshold=threshold;
			Gain=gain;
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			float threshold=(float)Threshold.GetValue(x,y);
			float nThreshold=-threshold;
			float a = 4f * (float)Gain.GetValue(x,y);
			a+=1f;
			
			// Read the two colours:
			UnityEngine.Color pix2 = SourceModule1.GetColour(x,y);
			UnityEngine.Color pix1 = SourceModule2.GetColour(x,y);
			
			
			// Get deltas:
			float rDelta = pix1.r - pix2.r;
			float gDelta = pix1.g - pix2.g;
			float bDelta = pix1.b - pix2.b;
			
			// Threshold test:
			if(rDelta<= nThreshold || rDelta >= threshold ){
				pix1.r = (a * rDelta + pix2.r);
			}
			
			if(gDelta<= nThreshold || gDelta >= threshold ){
				pix1.g = (a * gDelta + pix2.g);
			}
			
			if(bDelta<= nThreshold || bDelta >= threshold ){
				pix1.b = (a * bDelta + pix2.b);
			}
			
			return pix1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override double GetValue(double x, double y){
			
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override int TypeID{
			get{
				return 91;
			}
		}
		
	}
	
}