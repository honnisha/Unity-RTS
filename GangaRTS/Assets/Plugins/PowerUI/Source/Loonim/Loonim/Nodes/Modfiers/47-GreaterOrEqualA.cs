using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Greater or equal. Applies to alpha channel too.</summary>
	public class GreaterOrEqualA : StdLogicNode {
	
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule1.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule2.GetColour(x,y);
			
			// T:
			UnityEngine.Color t=IfTrue.GetColour(x,y);
			
			// False:
			UnityEngine.Color f=IfFalse.GetColour(x,y);
			
			// Pick:
			if(col1.r>=col2.r){
				col1.r=t.r;
			}else{
				col1.r=f.r;
			}
			
			if(col1.g>=col2.g){
				col1.g=t.g;
			}else{
				col1.g=f.g;
			}
			
			if(col1.b>=col2.b){
				col1.b=t.b;
			}else{
				col1.b=f.b;
			}
			
			if(col1.a>=col2.a){
				col1.a=t.a;
			}else{
				col1.a=f.a;
			}
			
			return col1;
			
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double a=SourceModule1.GetWrapped(x,y,wrap);
			double b=SourceModule2.GetWrapped(x,y,wrap);
			
			if(a>=b){
				return IfTrue.GetWrapped(x,y,wrap);
			}
			
			return IfFalse.GetWrapped(x,y,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			double a=SourceModule1.GetValue(x, y, z);
			double b=SourceModule2.GetValue(x, y,z);
			
			if(a>=b){
				return IfTrue.GetValue(x,y,z);
			}
			
			return IfFalse.GetValue(x,y,z);
		}
		
		public override double GetValue(double x, double y){
			double a=SourceModule1.GetValue(x, y);
			double b=SourceModule2.GetValue(x, y);
			
			if(a>=b){
				return IfTrue.GetValue(x,y);
			}
			
			return IfFalse.GetValue(x,y);
		}
		
		public override int TypeID{
			get{
				return 47;
			}
		}
		
	}
	
}