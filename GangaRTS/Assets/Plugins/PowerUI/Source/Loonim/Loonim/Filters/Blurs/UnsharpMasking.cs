//--------------------------------------
//       Loonim Image Generator
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

// FFT Ported from Java by Kulestar.
// Original Java source copyright 2005 Huxtable.com. 
// All rights reserved.

using System;
using UnityEngine;

namespace Loonim{

	public class UnsharpFilter : GaussianFilter{
		
		public float Amount;
		public float Threshold;
		
		
		public UnsharpFilter(float amount,float threshold,float radius):base(radius){
			Amount=amount;
			Threshold=threshold;
		}
		
	   public override void Filter(int width,int height,Color[] src,Color[] target) {
			
			// Perform gaussian:
			base.Filter(width,height,src,target);
			
			// src.getRGB( 0, 0, width, height, outPixels, 0, width );
			
			float threshold=Threshold;
			float nThreshold=-threshold;
			float a = 4f * Amount;
			float aP1=a + 1f;
			int index = 0;
			
			for ( int y = 0; y < height; y++ ) {
				for ( int x = 0; x < width; x++ ) {
					
					Color pix1 = target[index];
					
					Color pix2 = src[index];
					float rDelta = pix1.r - pix2.r;
					float gDelta = pix1.g - pix2.g;
					float bDelta = pix1.b - pix2.b;
					
					if(rDelta<= nThreshold || rDelta >= threshold ){
						pix1.r = (aP1 * rDelta + pix2.r);
					}
					
					if(gDelta<= nThreshold || gDelta >= threshold ){
						pix1.g = (aP1 * gDelta + pix2.g);
					}
					
					if(bDelta<= nThreshold || bDelta >= threshold ){
						pix1.b = (aP1 * bDelta + pix2.b);
					}
					
					target[index] = pix1;
					index++;
					
				}
				
			}
			
		}
		
	}
	
}
