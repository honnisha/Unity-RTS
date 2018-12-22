using System;
using UnityEngine;
using Blaze;

namespace Loonim{
	
	/// <summary>
	/// Generates a box with rounded edges. Can produce circles and elipses too.
	/// </summary>
	
    public class RoundedBox: TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
		/// <summary>The extra width of the box. Radius is added on top.</summary>
		// height/width gives elipseness. width is dx offset. height is dy offset.
		public TextureNode WidthModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		/// <summary>The smoothness of the edges.</summary>
		public TextureNode BlurModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The corner radius.</summary>
		public TextureNode RadiusModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>Optional function to apply to radius. This is used to create polygons and stars.
		/// The "angle" about the origin is used as the x value, ranging from 0-1. 
		/// For polygons, this edge funtion repeats itself n edges times. For stars, the function dips in the middle.</summary>
		public TextureNode EdgeFunction{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public RoundedBox():base(4){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Base value is..
			float value=(float)GetValue(x,y);
			
			// Black + white:
			return new Color(value,value,value,1f);
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			return GetValue(x,y);
		}
		
        public override double GetValue(double x, double y, double z){
			return GetValue(x,y);
		}
		
        public override double GetValue(double x, double y){
			
			// Radius:
			double radius=RadiusModule.GetValue(x,y);
			
			// Get width (and height):
			double size=WidthModule.GetValue(x,y);
			
			// Blur factor:
			double blur=BlurModule.GetValue(x,y);
			
			// Get delta from origin:
			double dx=x-0.5;
			double dy=y-0.5;
			
			double angle=0.0;
			
			if(EdgeFunction!=null){
				
				// Compute the angle in 0-1 range:
				angle=System.Math.Atan2(dy,dx) / ( System.Math.PI * 2.0 );
				angle+=0.5;
				
			}
			
			// Offset dx and dy:
			if(size>0){
				
				if(dx<0){
					dx=-dx;
				}
				
				dx-=size;
				
				if(dx<0){
					dx=0;
				}
				
				if(dy<0){
					dy=-dy;
				}
				
				dy-=size;
				
				if(dy<0){
					dy=0;
				}
				
			}
			
			// Compute distance:
			double distance=System.Math.Sqrt(dx * dx + dy * dy);
			
			if(EdgeFunction!=null){
				
				// Chord adjustment:
				// - Imagine a right angle triangle segment fitted perfectly into the circle.
				//   The right angle is sitting at the center of the circle, and one of its edges
				//   forms a chord of the circle.
				//   The chord adjustment is where we compute the distance to the chord
				//   and update the radius value. This results in n-polygon edges in the output.
				
				// Read the graph at the angle in the 0-1 range and apply to radius:
				radius*=1.0-EdgeFunction.GetValue(angle);
				
			}
			
			if(distance>radius){
				
				// Beyond the outside of the shape.
				return 0.0;
				
			}
			
			// Inside the shape. Inside the blur?
			distance-=(radius-blur);
			
			if(distance<0.0){
				return 1.0;
			}
			
			// Interpolate:
			return 1.0-distance/blur;
			
        }
		
		public override int TypeID{
			get{
				return 98;
			}
		}
		
    }
	
}
