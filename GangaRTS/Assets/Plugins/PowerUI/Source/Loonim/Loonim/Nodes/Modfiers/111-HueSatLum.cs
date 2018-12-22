using System;
using UnityEngine;

namespace Loonim{

	public class HueSatLum : TextureNode{
		
		public HueSatLum():base(4){}
		
		public HueSatLum(TextureNode src,TextureNode hueChange,TextureNode satChange,TextureNode lumChange):base(4){
			Sources[0]=src;
			HueModule=hueChange;
			SatModule=satChange;
			LumModule=lumChange;
		}
		
		public TextureNode HueModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode SatModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode LumModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float hueChange=(float)HueModule.GetValue(x,y);
			float satChange=(float)SatModule.GetValue(x,y);
			float lumChange=(float)LumModule.GetValue(x,y);
			
			// Read hsl:
			float h=col1.r;
			float s=col1.g;
			float l=col1.b;
			HslRgb.ToHsl(ref h,ref s,ref l);
			
			// Boost all 3:
			h=(h+hueChange)%1f;
			s*=1f+satChange;
			l*=1f+lumChange;
			
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
			double lum=1f+LumModule.GetWrapped(x,y,wrap);
			
			return baseValue * lum;
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y,z);
			double lum=1f+LumModule.GetValue(x,y,z);
			
			return baseValue * lum;
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y);
			double lum=1f+LumModule.GetValue(x,y);
			
			return baseValue * lum;
			
		}	  
		
		public override int TypeID{
			get{
				return 111;
			}
		}
		
	}
	
}
