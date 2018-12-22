using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Contrast around a particular origin.
	/// </summary>
	public class ContrastOrigin : GraphNode{
	
		public TextureNode Contrast{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode Origin{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public ContrastOrigin():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float contrast=1f + (float)Contrast.GetValue(x,y);
			float origin=(float)Origin.GetValue(x,y);
			
			// Boost:
			col1.r= ( (col1.r-origin ) * contrast ) + origin;
			col1.g= ( (col1.g-origin ) * contrast ) + origin;
			col1.b= ( (col1.b-origin ) * contrast ) + origin;
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			if(SourceModule == null){
				return 0;
			}
			
			double origin=Origin.GetWrapped(x,y,wrap);
			double baseValue=SourceModule.GetWrapped(x,y,wrap);
			double contrast=1.0 + Contrast.GetWrapped(x,y,wrap);
			
			return ( (baseValue-origin) * contrast ) + origin;
			
		}
		
		public override double GetValue(double x, double y, double z){
			if(SourceModule == null){
				return 0;
			}
			
			double origin=Origin.GetValue(x,y,z);
			double baseValue=SourceModule.GetValue(x,y,z);
			double contrast=1.0 + Contrast.GetValue(x,y,z);
			
			return ( (baseValue-origin) * contrast ) + origin;
			
		}
		
		public override double GetValue(double x, double y)
		{
			if(SourceModule == null){
				return 0;
			}
			
			double origin=Origin.GetValue(x,y);
			double baseValue=SourceModule.GetValue(x,y);
			double contrast=1.0 + Contrast.GetValue(x,y);
			
			return ( (baseValue-origin) * contrast ) + origin;
			
		}	  
		
		public override double GetValue(double t){
			
			// Read amplitude:
			double amp=Contrast.GetValue(t);
			
			// Origin:
			double origin=Origin.GetValue(t);
			
			// Sample at that point:
			return ( (SourceModule1.GetValue(t)-origin) * amp ) + origin;
			
		}
		
		public override int TypeID{
			get{
				return 38;
			}
		}
		
	}
	
}
