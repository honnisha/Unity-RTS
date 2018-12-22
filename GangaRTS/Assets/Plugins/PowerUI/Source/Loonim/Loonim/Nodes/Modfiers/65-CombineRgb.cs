using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Combines r,g and b channels into one. Best sampled using GetColour of course!</summary>
	public class CombineRGB : TextureNode {
		
		public TextureNode RedModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode GreenModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode BlueModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public CombineRGB():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read R:
			float r=(float)RedModule.GetValue(x,y);
			
			// Read G:
			float g=(float)GreenModule.GetValue(x,y);
			
			// Read B:
			float b=(float)BlueModule.GetValue(x,y);
			
			return new Color(r,g,b,1f);
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Act like intensity:
			double sum=RedModule.GetWrapped(x,y,wrap);
			sum+=GreenModule.GetWrapped(x,y,wrap);
			sum+=BlueModule.GetWrapped(x,y,wrap);
			
			return sum/3.0;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Act like intensity:
			double sum=RedModule.GetValue(x,y,z);
			sum+=GreenModule.GetValue(x,y,z);
			sum+=BlueModule.GetValue(x,y,z);
			
			return sum/3.0;
			
		}
		
		public override double GetValue(double x, double y){
			
			// Act like intensity:
			double sum=RedModule.GetValue(x,y);
			sum+=GreenModule.GetValue(x,y);
			sum+=BlueModule.GetValue(x,y);
			
			return sum/3.0;
			
		}
		
		public override int TypeID{
			get{
				return 65;
			}
		}
		
	}
	
}
