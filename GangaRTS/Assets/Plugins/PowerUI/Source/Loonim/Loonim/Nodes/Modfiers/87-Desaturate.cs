using System;
using UnityEngine;

/// <summary>
/// Turns something black + white by removing saturation.
/// </summary>

namespace Loonim{

	
	public enum DesaturateMethod:int{
		Greyscale=0, // RGB average.
		YChannel=1, // HSY 'Y' Channel.
		LChannel=2, // HSL 'L' Channel.
		VChannel=3 // HSV 'V' Channel.
	}
	
    public class Desaturate : Std1InputNode {
		
		public DesaturateMethod Method=DesaturateMethod.YChannel;
        
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public override int SubMaterialID{
			get{
				return (int)Method;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
            UnityEngine.Color col1=SourceModule.GetColour(x,y);
			float result;
			
			switch(Method){
				
				case DesaturateMethod.YChannel: // HSY model
					
					result=HsyRgb.Luminance(col1.r,col1.g,col1.b);
					
				break;
				
				case DesaturateMethod.LChannel: // HSL model
					
					result=HslRgb.Luminance(col1.r,col1.g,col1.b);
					
				break;
				
				case DesaturateMethod.VChannel: // HSV model
					
					result=HsvRgb.Luminance(col1.r,col1.g,col1.b);
					
				break;
				
				default:
				case DesaturateMethod.Greyscale: // Greyscale RGB average
					
					result=(col1.r + col1.g + col1.b ) /3f;
					
				break;
				
			}
			
			// Grey colour:
			return new Color(result,result,result,col1.a);
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
            double baseValue=SourceModule.GetWrapped(x,y,wrap);
			
			return baseValue;
			
		}
		
        public override double GetValue(double x, double y, double z){
			
            double baseValue=SourceModule.GetValue(x,y,z);
			
			return baseValue;
			
		}
		
        public override double GetValue(double x, double y)
        {
            
            double baseValue=SourceModule.GetValue(x,y);
			
			return baseValue;
			
        }      
		
		public override int TypeID{
			get{
				return 87;
			}
		}
		
    }
	
}
