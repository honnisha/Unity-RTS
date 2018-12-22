using System;
using UnityEngine;

namespace Loonim{
	
	public class Saturation : Std2InputNode{
	
		public TextureNode SaturationModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float satChange=(float)SaturationModule.GetValue(x,y);
			
			// Read hsl:
			float h=col1.r;
			float s=col1.g;
			float l=col1.b;
			HslRgb.ToHsl(ref h,ref s,ref l);
			
			// Boost sat:
			s*=(1f+satChange);
			
			// Back to colour:
			HslRgb.ToRgb(ref h,ref s,ref l);
			
			// Now RGB:
			col1.r=h;
			col1.g=s;
			col1.b=l;
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetWrapped(x,y,wrap);
			double sat=1f+SaturationModule.GetWrapped(x,y,wrap);
			
			return baseValue * sat;
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y,z);
			double sat=1f+SaturationModule.GetValue(x,y,z);
			
			return baseValue * sat;
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y);
			double sat=1f+SaturationModule.GetValue(x,y);
			
			return baseValue * sat;
			
		}	  
		
		public override int TypeID{
			get{
				return 42;
			}
		}
		
	}
	
}
