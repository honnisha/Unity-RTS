using System;
using UnityEngine;

namespace Loonim
{
	
	/// <summary>
	/// Generates a "full colour" spectrum.
	/// </summary>
	
    public class Spectrum: Std2InputNode{
		
		public TextureNode Saturation{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode Lightness{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			float r=(float)x;
			float g=(float)Saturation.GetValue(x,y);
			float b=(float)Lightness.GetValue(x,y);
			
			HsvRgb.ToRgb(ref r,ref g,ref b);
			
			return new UnityEngine.Color(r,g,b,1f);
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			return GetValue(x,y);
		}
		
        public override double GetValue(double x, double y, double z){
			return GetValue(x,y);
		}
		
        public override double GetValue(double x, double y){
			UnityEngine.Color col=GetColour(x,y);
			return (col.r + col.g + col.b)/3.0;
        }
		
		public override int TypeID{
			get{
				return 95;
			}
		}
		
    }
	
}
