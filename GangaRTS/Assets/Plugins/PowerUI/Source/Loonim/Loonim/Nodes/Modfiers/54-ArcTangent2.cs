using System;
using UnityEngine;

namespace Loonim{
	
	public class ArcTangent2 : Std2InputNode{
	
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule1.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule2.GetColour(x,y);
			
			col1.r=(float)System.Math.Atan2(col1.r,col2.r);
			col1.g=(float)System.Math.Atan2(col1.g,col2.g);
			col1.b=(float)System.Math.Atan2(col1.b,col2.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return System.Math.Atan2(SourceModule1.GetWrapped(x,y,wrap),SourceModule2.GetWrapped(x,y,wrap));
		}
		
		public override double GetValue(double x, double y, double z){
			return System.Math.Atan2(SourceModule1.GetValue(x, y, z),SourceModule2.GetValue(x, y, z));
		}
		
		public override double GetValue(double x, double y){
			return System.Math.Atan2(SourceModule1.GetValue(x, y),SourceModule2.GetValue(x, y));
		}
		
		public override int TypeID{
			get{
				return 54;
			}
		}
		
	}
	
}
