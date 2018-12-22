using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Reads the alpha channel of an image.</summary>
	public class SplitAlpha : Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read:
			float a=SourceModule.GetColour(x,y).a;
			
			return new Color(a,a,a,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Read:
			return SourceModule.GetColour(x,y).a;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Read:
			return SourceModule.GetColour(x,y).a;
			
		}
		
		public override double GetValue(double x, double y){
			
			// Read:
			return SourceModule.GetColour(x,y).a;
			
		}
		
		public override int TypeID{
			get{
				return 80;
			}
		}
		
	}
	
}
