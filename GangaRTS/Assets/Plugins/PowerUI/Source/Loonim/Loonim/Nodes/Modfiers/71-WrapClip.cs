using System;
using UnityEngine;

namespace Loonim{

    public class WrapClip : TextureNode{
        
		/// <summary>The minimum x value.</summary>
		public TextureNode MinXModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The minimum y value.</summary>
		public TextureNode MinYModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The maximum x value minus the min x value.</summary>
		public TextureNode RangeXModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>The maximum y value minus the min y value.</summary>
		public TextureNode RangeYModule{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		
		
		public WrapClip():base(5){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Get the bounds:
			double minX=MinXModule.GetValue(x,y);
			double rangeX=RangeXModule.GetValue(x,y); // maxX = rangeX-minX
			double minY=MinYModule.GetValue(x,y);
			double rangeY=RangeYModule.GetValue(x,y); // maxY = rangeY-minY
			
			//Wrap and clip both:
			x=( (x-minX) % rangeX) + minX;
			y=( (y-minY) % rangeY) + minY;
			
			return SourceModule.GetColour(x,y);
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Get the bounds:
			double minX=MinXModule.GetValue(x,y);
			double rangeX=RangeXModule.GetValue(x,y); // maxX = rangeX-minX
			double minY=MinYModule.GetValue(x,y);
			double rangeY=RangeYModule.GetValue(x,y); // maxY = rangeY-minY
			
			//Wrap and clip both:
			x=( (x-minX) % rangeX) + minX;
			y=( (y-minY) % rangeY) + minY;
			
			return SourceModule.GetWrapped(x,y,wrap);
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Get the bounds:
			double minX=MinXModule.GetValue(x,y);
			double rangeX=RangeXModule.GetValue(x,y); // maxX = rangeX-minX
			double minY=MinYModule.GetValue(x,y);
			double rangeY=RangeYModule.GetValue(x,y); // maxY = rangeY-minY
			
			//Wrap and clip both:
			x=( (x-minX) % rangeX) + minX;
			y=( (y-minY) % rangeY) + minY;
			
			return SourceModule.GetValue(x,y,z);
		}
		
        public override double GetValue(double x, double y){
			
			// Get the bounds:
			double minX=MinXModule.GetValue(x,y);
			double rangeX=RangeXModule.GetValue(x,y); // maxX = rangeX-minX
			double minY=MinYModule.GetValue(x,y);
			double rangeY=RangeYModule.GetValue(x,y); // maxY = rangeY-minY
			
			//Wrap and clip both:
			x=( (x-minX) % rangeX) + minX;
			y=( (y-minY) % rangeY) + minY;
			
            return SourceModule.GetValue(x,y);
        }
		
		public override double GetValue(double t){
			
			// Read min and range:
			double min=MinXModule.GetValue(t);
			double range=RangeXModule.GetValue(t); // max = range-min
			
			//Wrap and clip:
			t=( (t-min) % range) + min;
			
			// Src value:
			return SourceModule.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 71;
			}
		}
		
    }
	
}
