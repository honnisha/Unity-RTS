using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Combines h,s and y channels into one. Best sampled using GetColour of course!</summary>
	public class CombineHSY : TextureNode {
		
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
		
		public TextureNode LightModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public CombineHSY():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read H:
			float h=(float)HueModule.GetValue(x,y);
			
			// Read S:
			float s=(float)SaturationModule.GetValue(x,y);
			
			// Read Y:
			float ly=(float)LightModule.GetValue(x,y);
			
			// Convert to RGB:
			HsyRgb.ToRgb(ref h,ref s,ref ly);
			
			// Now RGB.
			return new Color(h,s,ly,1f);
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
				return 68;
			}
		}
		
	}
	
}
