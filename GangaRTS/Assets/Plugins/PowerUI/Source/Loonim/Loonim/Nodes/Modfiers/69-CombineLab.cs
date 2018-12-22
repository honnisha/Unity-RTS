using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Combines l,a and b channels into one. Best sampled using GetColour of course!</summary>
	public class CombineLAB : TextureNode {
		
		public TextureNode AModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode BModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode LightModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public CombineLAB():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read L:
			float l=(float)LightModule.GetValue(x,y);
			
			// Read A:
			float a=(float)AModule.GetValue(x,y);
			
			// Read B:
			float b=(float)BModule.GetValue(x,y);
			
			// Convert to RGB:
			LabRgb.ToRgb(ref l,ref a,ref b);
			
			// Now RGB.
			return new Color(l,a,b,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Act like intensity:
			return LightModule.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Act like intensity:
			return LightModule.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x, double y){
			
			// Act like intensity:
			return LightModule.GetValue(x,y);
			
		}
		
		public override int TypeID{
			get{
				return 69;
			}
		}
		
	}
	
}
