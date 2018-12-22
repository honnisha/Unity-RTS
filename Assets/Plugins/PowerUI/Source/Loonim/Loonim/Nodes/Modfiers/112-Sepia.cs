using System;
using UnityEngine;

namespace Loonim{

	public class Sepia : Std2InputNode{
		
		public TextureNode IntensityModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public Sepia(){}
		
		public Sepia(TextureNode src){
			SourceModule=src;
			IntensityModule = new Property(1f);
		}
		
		public Sepia(TextureNode src,TextureNode intensity){
			SourceModule=src;
			IntensityModule = intensity;
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			float weight=(float)IntensityModule.GetValue(x,y);
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Compute full sepia version:
			float sepiaR=(col1.r * .393f) + (col1.g *.769f) + (col1.b * .189f);
			float sepiaG=(col1.r * .349f) + (col1.g *.686f) + (col1.b * .168f);
			float sepiaB=(col1.r * .272f) + (col1.g *.534f) + (col1.b * .131f);
			
			// Blended:
			return new Color(
				col1.r + (sepiaR - col1.r) * weight,
				col1.g + (sepiaG - col1.g) * weight,
				col1.b + (sepiaB - col1.b) * weight,
				col1.a
			);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			if(SourceModule == null){
				return 0;
			}
			
			return SourceModule.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			return SourceModule.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			return SourceModule.GetValue(x,y);
			
		}	  
		
		public override int TypeID{
			get{
				return 112;
			}
		}
		
	}
	
}
