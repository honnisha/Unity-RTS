using System;
using UnityEngine;

namespace Loonim{
	
	public class InvertInput : Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			return SourceModule.GetColour(-x,-y);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return SourceModule.GetWrapped(-x, -y, wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return SourceModule.GetValue(-x, -y, -z);
		}
		
		public override double GetValue(double x, double y){
			return SourceModule.GetValue(-x, -y);
		}
		
		public override int TypeID{
			get{
				return 23;
			}
		}
		
	}
	
}
