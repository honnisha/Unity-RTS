using System;
using UnityEngine;

namespace Loonim{
	
	public class ArcSine : Std1InputNode{
	
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			col1.r=(float)System.Math.Asin(col1.r);
			col1.g=(float)System.Math.Asin(col1.g);
			col1.b=(float)System.Math.Asin(col1.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return System.Math.Asin(SourceModule.GetWrapped(x,y,wrap));
		}
		
		public override double GetValue(double x, double y, double z){
			return System.Math.Asin(SourceModule.GetValue(x, y, z));
		}
		
		public override double GetValue(double x, double y){
			return System.Math.Asin(SourceModule.GetValue(x, y));
		}
		
		public override int TypeID{
			get{
				return 52;
			}
		}
		
	}
	
}
