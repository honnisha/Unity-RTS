using System;
using UnityEngine;

namespace Loonim{
	
	public class Root : Std2InputNode{
	
		public TextureNode DegreeModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=DegreeModule.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule.GetColour(x,y);
			
			if(col1.r!=0f){
				col1.r=(float)System.Math.Pow(col2.r,1.0 / col1.r);
			}
			
			if(col1.g!=0f){
				col1.g=(float)System.Math.Pow(col2.g,1.0 / col1.g);
			}
			
			if(col1.b!=0f){
				col1.b=(float)System.Math.Pow(col2.b,1.0 / col1.b);
			}
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double deg=DegreeModule.GetWrapped(x,y,wrap);
			
			if(deg==0){
				return 0;
			}
			
			return System.Math.Pow(SourceModule.GetWrapped(x,y,wrap),1.0 / deg);
		}
		
		public override double GetValue(double x, double y, double z){
			
			double deg=DegreeModule.GetValue(x, y, z);
			
			if(deg==0){
				return 0;
			}
			
			return System.Math.Pow(SourceModule.GetValue(x, y, z),1.0 / deg);
		}
		
		public override double GetValue(double x, double y){
			
			double deg=DegreeModule.GetValue(x, y);
			
			if(deg==0){
				return 0;
			}
			
			return System.Math.Pow(SourceModule.GetValue(x, y),1.0 / deg);
		}
		
		public override int TypeID{
			get{
				return 59;
			}
		}
		
	}
	
}
