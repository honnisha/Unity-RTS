using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Samples at 1-x,1-y.</summary>
	public class InvertColourInput : Std1InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			return SourceModule.GetColour(1-x,1-y);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return SourceModule.GetWrapped(1-x, 1-y, wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return SourceModule.GetValue(1-x, 1-y, 1-z);
		}
		
		public override double GetValue(double x, double y){
			return SourceModule.GetValue(1-x, 1-y);
		}
		
		public override double GetValue(double t){
			
			return SourceModule.GetValue(1.0 - t);
			
		}
		
		public override int TypeID{
			get{
				return 22;
			}
		}
		
	}
	
}
