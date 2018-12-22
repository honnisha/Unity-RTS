using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Aka HSV. Combines h,s and b channels into one. Best sampled using GetColour of course!</summary>
	public class CombineHSB : TextureNode {
		
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
		
		public TextureNode BrightModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public CombineHSB():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read H:
			float h=(float)HueModule.GetValue(x,y);
			
			// Read S:
			float s=(float)SaturationModule.GetValue(x,y);
			
			// Read V:
			float v=(float)BrightModule.GetValue(x,y);
			
			// Convert to RGB:
			HsvRgb.ToRgb(ref h,ref s,ref v);
			
			// Now RGB.
			return new Color(h,s,v,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Act like intensity:
			return BrightModule.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Act like intensity:
			return BrightModule.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x, double y){
			
			// Act like intensity:
			return BrightModule.GetValue(x,y);
			
		}
		
		public override int TypeID{
			get{
				return 67;
			}
		}
		
	}
	
}
