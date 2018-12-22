//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;

namespace Loonim{
	
	public class Checkerboard: TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			x*=2;
			y*=2;
			
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);

			return (double)((x0 & 1 ^ y0 & 1));
		}
		
		public override double GetValue(double x, double y, double z){
			x*=2;
			y*=2;
			z*=2;
			
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);
			int z0 = (z > 0.0 ? (int)z : (int)z - 1);
			
			return (double)((x0 & 1 ^ y0 & 1 ^ z0 & 1));
			
		}
		
		public override double GetValue(double x, double y){
			x*=2;
			y*=2;
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);
			
			return (double)(x0 & 1 ^ y0 & 1);
			
		}
		
		public override int TypeID{
			get{
				return 2;
			}
		}
		
	}
}
