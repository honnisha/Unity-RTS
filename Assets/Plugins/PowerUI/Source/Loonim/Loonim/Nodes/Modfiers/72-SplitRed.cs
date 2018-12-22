using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Reads the red channel of an image.</summary>
	public class SplitRed : Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read:
			float channel=SourceModule.GetColour(x,y).r;
			
			return new Color(channel,channel,channel,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			return SourceModule.GetColour(x,y).r;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			return SourceModule.GetColour(x,y).r;
			
		}
		
		public override double GetValue(double x, double y){
			
			return SourceModule.GetColour(x,y).r;
			
		}
		
		public override int TypeID{
			get{
				return 72;
			}
		}
		
	}
	
}
