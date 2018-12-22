using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>Rounds Source upwards to nearest 'ToNearest'.</summary>
	public class Ceiling : Std2InputNode{
		
		public TextureNode ToNearestModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=ToNearestModule.GetColour(x,y);
			
			col1.r=col2.r * (float)System.Math.Ceiling(col1.r / col2.r);
			col1.g=col2.g * (float)System.Math.Ceiling(col1.g / col2.g);
			col1.b=col2.b * (float)System.Math.Ceiling(col1.b / col2.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double toNearest=ToNearestModule.GetWrapped(x,y,wrap);
			
			if(toNearest==0){
				return 0;
			}
			
			return toNearest * System.Math.Ceiling( SourceModule.GetWrapped(x,y,wrap) / toNearest);
		}
		
		public override double GetValue(double x, double y, double z){
			
			double toNearest=ToNearestModule.GetValue(x,y,z);
			
			if(toNearest==0){
				return 0;
			}
			
			return toNearest * System.Math.Ceiling( SourceModule.GetValue(x,y,z) / toNearest);
		}
		
		public override double GetValue(double x, double y){
			
			double toNearest=ToNearestModule.GetValue(x,y);
			
			if(toNearest==0){
				return 0;
			}
			
			return toNearest * System.Math.Ceiling( SourceModule.GetValue(x,y) / toNearest);
		}
		
		public override int TypeID{
			get{
				return 63;
			}
		}
		
	}
	
}
