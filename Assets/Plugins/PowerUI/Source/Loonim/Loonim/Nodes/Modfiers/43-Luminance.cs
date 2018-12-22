using System;
using UnityEngine;

namespace Loonim{
	
	public class Luminance : Std2InputNode{
	
		public TextureNode LuminanceModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float ctrast=1f + (float)LuminanceModule.GetValue(x,y);
			
			// Boost:
			col1.r*=ctrast;
			col1.g*=ctrast;
			col1.b*=ctrast;
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetWrapped(x,y,wrap);
			double luminance=1.0 + LuminanceModule.GetWrapped(x,y,wrap);
			
			return baseValue * luminance;
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y,z);
			double luminance=1.0 + LuminanceModule.GetValue(x,y,z);
			
			return baseValue * luminance;
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y);
			double luminance=1.0 + LuminanceModule.GetValue(x,y);
			
			return baseValue * luminance;
			
		}	  

		public override int TypeID{
			get{
				return 43;
			}
		}
		
	}
	
}