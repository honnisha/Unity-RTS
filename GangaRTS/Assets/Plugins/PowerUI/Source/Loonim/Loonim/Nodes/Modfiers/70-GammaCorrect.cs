using System;
using UnityEngine;

namespace Loonim{

	public class GammaCorrect : Std2InputNode{
		
		/// <summary>Note that -1 to +1 is range mapped to 0.1 to 10. 0 represents gamma=1 (no change).</summary>
		public TextureNode GammaModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Read gamma:
			double gamma=GammaModule.GetValue(x,y);
			
			// Map gamma into the 0.1 to 10 range, and invert it:
			gamma=1.0 / System.Math.Pow(10.0,gamma);
			
			col1.r=(float)(col1.r * gamma);
			col1.g=(float)(col1.g * gamma);
			col1.b=(float)(col1.b * gamma);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Read gamma:
			double gamma=GammaModule.GetValue(x,y);
			
			// Map gamma into the 0.1 to 10 range, and invert it:
			gamma=1.0 / System.Math.Pow(10.0,gamma);
			
			return SourceModule.GetWrapped(x,y,wrap) * gamma;
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Read gamma:
			double gamma=GammaModule.GetValue(x,y);
			
			// Map gamma into the 0.1 to 10 range, and invert it:
			gamma=1.0 / System.Math.Pow(10.0,gamma);
			
			return SourceModule.GetValue(x, y, z) * gamma;
		}
		
		public override double GetValue(double x, double y){
			
			// Read gamma:
			double gamma=GammaModule.GetValue(x,y);
			
			// Map gamma into the 0.1 to 10 range, and invert it:
			gamma=1.0 / System.Math.Pow(10.0,gamma);
			
			return SourceModule.GetValue(x, y) * gamma;
		}
		
		public override int TypeID{
			get{
				return 70;
			}
		}
		
	}
	
}
