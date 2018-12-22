using System;
using UnityEngine;

namespace Loonim{
	
	public class Cylinders: TextureNode{
		
		public Cylinders():base(1){}
		
		public TextureNode Frequency{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x *= frequency;
			y *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y);
			int distFromCenter0 = (distFromCenter > 0.0 ? (int)distFromCenter : (int)distFromCenter - 1);
			double distFromSmallerSphere = distFromCenter - distFromCenter0;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 2.0);
		}
		
		public override double GetValue(double x, double y, double z){
		
			double frequency=Frequency.GetValue(x,y,z);
			x *= frequency;
			y *= frequency;
			z *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y + z * z);
			int distFromCenter0 = (distFromCenter > 0.0 ? (int)distFromCenter : (int)distFromCenter - 1);
			double distFromSmallerSphere = distFromCenter - distFromCenter0;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 2.0);
		}
		
		public override double GetValue(double x, double y){
			
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y);
			int distFromCenter0 = (distFromCenter > 0.0 ? (int)distFromCenter : (int)distFromCenter - 1);
			double distFromSmallerSphere = distFromCenter - distFromCenter0;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 2.0);
		}
		
		public override int TypeID{
			get{
				return 4;
			}
		}
		
	}
}
