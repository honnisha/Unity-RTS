using System;
using UnityEngine;

namespace Loonim{
	
	public class Spheres : Std1InputNode {
		
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
				// 2D.
				return 2;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x *= frequency;
			y *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y);
			int xInt = (x > 0.0 ? (int)x : (int)x - 1);
			double distFromSmallerSphere = distFromCenter - xInt;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
		}
		
		public override double GetValue(double x, double y, double z){
			
			double frequency=Frequency.GetValue(x,y,z);
			x *= frequency;
			y *= frequency;
			z *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y + z * z);
			int xInt = (x > 0.0 ? (int)x : (int)x - 1);
			double distFromSmallerSphere = distFromCenter - xInt;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
		}
		
		public override double GetValue(double x, double y)
		{
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;

			double distFromCenter = System.Math.Sqrt(x * x + y * y);
			int xInt = (x > 0.0 ? (int)x : (int)x - 1);
			double distFromSmallerSphere = distFromCenter - xInt;
			double distFromLargerSphere = 1.0 - distFromSmallerSphere;
			double nearestDist = Math.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
			return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
		}
		
		public override int TypeID{
			get{
				return 11;
			}
		}
		
	}
	
}
