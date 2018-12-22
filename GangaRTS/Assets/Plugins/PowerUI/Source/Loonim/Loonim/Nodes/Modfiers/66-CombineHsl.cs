using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Combines h,s and l channels into one. Best sampled using GetColour of course!</summary>
	public class CombineHSL : TextureNode {
		
		public TextureNode HueModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode SaturationModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode LumModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public CombineHSL():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read H:
			float h=(float)HueModule.GetValue(x,y);
			
			// Read S:
			float s=(float)SaturationModule.GetValue(x,y);
			
			// Read L:
			float l=(float)LumModule.GetValue(x,y);
			
			// Convert to RGB:
			HslRgb.ToRgb(ref h,ref s,ref l);
			
			// Now RGB.
			return new Color(h,s,l,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Act like intensity:
			return LumModule.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Act like intensity:
			return LumModule.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x, double y){
			
			// Act like intensity:
			return LumModule.GetValue(x,y);
			
		}
		
		public override int TypeID{
			get{
				return 66;
			}
		}
		
	}
	
}
