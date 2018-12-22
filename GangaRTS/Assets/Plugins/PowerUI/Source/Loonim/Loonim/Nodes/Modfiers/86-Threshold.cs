using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Threshold. Selects high if above and low if below. May blend depending on smoothing range.</summary>
    public class Threshold : TextureNode{
		
		/// <summary>If above threshold, use this.</summary>
		public TextureNode HighModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>If below threshold, use this.</summary>
		public TextureNode LowModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The actual threshold.</summary>
		public TextureNode ThresholdModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>Rather than a sudden change from low->high, defines a blending range.</summary>
		public TextureNode SmoothingRange{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		public Threshold():base(5){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Threshold:
			double threshold=ThresholdModule.GetValue(x,y);
			
			// Smoothing range (+- half this):
			double smooth=SmoothingRange.GetValue(x,y);
			double hSmooth=smooth * 0.5;
			
			// Get input colours intensity:
			double intensity=(col1.r + col1.g + col1.b) / 3.0;
			
			// High:
			UnityEngine.Color high;
			
			// Low:
			UnityEngine.Color low;
			
			// How does it compare to the minimum?
			double min=threshold-hSmooth;
			
			// Note: Equals here avoids a division by 0 when computing blending factor.
			if(intensity<=min){
				
				low=LowModule.GetColour(x,y);
				low.a=col1.a;
				return low;
				
			}
			
			// And the max?
			if(intensity>=threshold+hSmooth){
				
				high=HighModule.GetColour(x,y);
				high.a=col1.a;
				return high;
				
			}
			
			// Smooth. Blend between high and low.
			high=HighModule.GetColour(x,y);
			low=LowModule.GetColour(x,y);
			
			// Blending factor is.. (NB: won't div by 0)
			float blend=(float)( (intensity - min) / smooth);
			
			col1.r=low.r + (high.r - low.r) * blend;
			col1.g=low.g + (high.g - low.g) * blend;
			col1.b=low.b + (high.b - low.b) * blend;
			
			return col1;
			
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
		
		public override double GetValue(double t){
			return GetValue(t,0);
		}
		
		public override int TypeID{
			get{
				return 86;
			}
		}
		
    }
	
}