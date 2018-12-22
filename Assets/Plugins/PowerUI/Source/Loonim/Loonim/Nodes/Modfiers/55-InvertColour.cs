using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Similar to invert only this is 1-X. More traditional colour inversion.</summary>
	public class InvertColour : Std1InputNode{
	
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			Color col1=SourceModule.GetColour(x,y);
			
			col1.r=1f - col1.r;
			col1.g=1f - col1.g;
			col1.b=1f - col1.b;
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return 1.0-SourceModule.GetWrapped(x,y,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return 1.0-SourceModule.GetValue(x, y, z);
		}
		
		public override double GetValue(double x, double y){
			return 1.0-SourceModule.GetValue(x, y);
		}
		
		public override double GetValue(double t){
			
			return 1.0-SourceModule.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 55;
			}
		}
		
	}
	
}
