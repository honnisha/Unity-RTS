using System;
using UnityEngine;

namespace Loonim{
	
	public class LargerOutput : Std2InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule1.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule2.GetColour(x,y);
			
			// Pick biggest:
			if(col2.r>col1.r){
				col1.r=col2.r;
			}
			
			if(col2.g>col1.g){
				col1.g=col2.g;
			}
			
			if(col2.b>col1.b){
				col1.b=col2.b;
			}
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double a=SourceModule1.GetWrapped(x,y,wrap);
			double b=SourceModule2.GetWrapped(x,y,wrap);
			
			if(a>b){
				return a;
			}
			return b;
		}
		
		public override double GetValue(double x, double y, double z){
			double a=SourceModule1.GetValue(x, y, z);
			double b=SourceModule2.GetValue(x, y,z);
			
			if(a>b){
				return a;
			}
			return b;
		}
		
		public override double GetValue(double x, double y){
			double a=SourceModule1.GetValue(x, y);
			double b=SourceModule2.GetValue(x, y);
			
			if(a>b){
				return a;
			}
			return b;
		}
		
		public override double GetValue(double t){
			
			double a=SourceModule1.GetValue(t);
			double b=SourceModule2.GetValue(t);
			
			if(a>b){
				return a;
			}
			
			return b;
			
		}
		
		public override int TypeID{
			get{
				return 25;
			}
		}
		
	}
	
}
