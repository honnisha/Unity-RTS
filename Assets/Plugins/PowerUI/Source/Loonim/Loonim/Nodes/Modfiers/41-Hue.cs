using System;
using UnityEngine;

namespace Loonim{

	public class Hue : Std2InputNode{
		
		public Hue(){}
		
		public Hue(TextureNode src,TextureNode hueChange){
			Sources[0]=src;
			Sources[1]=hueChange;
		}
		
		public TextureNode HueModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float hueChange=(float)HueModule.GetValue(x,y);
			
			// Read hsl:
			float h=col1.r;
			float s=col1.g;
			float l=col1.b;
			HslRgb.ToHsl(ref h,ref s,ref l);
			
			// Cycle the hue:
			h=(h+hueChange)%1f;
			
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
			double hue=HueModule.GetWrapped(x,y,wrap);
			
			// Cycle the hue:
			return (baseValue+hue)%1f;
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y,z);
			double hue=HueModule.GetValue(x,y,z);
			
			// Cycle the hue:
			return (baseValue+hue)%1f;
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y);
			double hue=HueModule.GetValue(x,y);
			
			// Cycle the hue:
			return (baseValue+hue)%1f;
		}	  
		
		public override int TypeID{
			get{
				return 41;
			}
		}
		
	}
	
}
