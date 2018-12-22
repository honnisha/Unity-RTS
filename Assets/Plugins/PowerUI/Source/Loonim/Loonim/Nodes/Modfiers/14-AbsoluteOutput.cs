using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Module that returns the absolute value of the output of a source module.	
	/// </summary>
	public class AbsoluteOutput: Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col=SourceModule.GetColour(x,y);
			
			// ABS the rgb channels:
			if(col.r<0f){
				col.r=-col.r;
			}
			
			if(col.g<0f){
				col.g=-col.g;
			}
			
			if(col.b<0f){
				col.b=-col.b;
			}
			
			return col;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return System.Math.Abs(SourceModule.GetWrapped(x,y,wrap));
		}
		
		public override double GetValue(double x, double y, double z){
			return System.Math.Abs(SourceModule.GetValue(x, y,z));
		}
		
		/// <summary>
		/// Returns the absolute value of noise from the source module at the given coordinates.
		/// </summary>
		public override double GetValue(double x, double y){
			return System.Math.Abs(SourceModule.GetValue(x, y));
		}
		
		public override int TypeID{
			get{
				return 14;
			}
		}
		
	}
	
}
