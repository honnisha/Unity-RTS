using System;
using UnityEngine;

namespace Loonim{

	public class Contrast : Std2InputNode{
	
		public TextureNode ContrastModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float contrast=1f + (float)ContrastModule.GetValue(x,y);
			
			// Boost:
			col1.r= ( (col1.r-0.5f ) * contrast ) + 0.5f;
			col1.g= ( (col1.g-0.5f ) * contrast ) + 0.5f;
			col1.b= ( (col1.b-0.5f ) * contrast ) + 0.5f;
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetWrapped(x,y,wrap);
			double contrast=1.0 + ContrastModule.GetWrapped(x,y,wrap);
			
			return ( (baseValue-0.5) * contrast ) + 0.5;
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y,z);
			double contrast=1.0 + ContrastModule.GetValue(x,y,z);
			
			return ( (baseValue-0.5) * contrast ) + 0.5;
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double baseValue=SourceModule.GetValue(x,y);
			double contrast=1.0 + ContrastModule.GetValue(x,y);
			
			return ( (baseValue-0.5) * contrast ) + 0.5;
			
		}	  

		public override int TypeID{
			get{
				return 40;
			}
		}
		
	}
	
}
