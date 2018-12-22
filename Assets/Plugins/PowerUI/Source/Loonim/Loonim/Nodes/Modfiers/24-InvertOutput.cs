using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Note that this differs from InvertColour. This is simply -X.</summary>
	public class InvertOutput : Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			Color col1=SourceModule.GetColour(x,y);
			
			// Flipped:
			col1.r=- col1.r;
			col1.g=- col1.g;
			col1.b=- col1.b;
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return -SourceModule.GetWrapped(x,y,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return -SourceModule.GetValue(x, y, z);
		}
		
		public override double GetValue(double x, double y){
			return -SourceModule.GetValue(x, y);
		}
		
		public override int TypeID{
			get{
				return 24;
			}
		}
		
	}
	
}
