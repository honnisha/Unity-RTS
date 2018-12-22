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

	public class BoxBlurFilter{
		
		/// <summary>Set the radius of the kernel, and hence the amount of blur.</summary>
		public int HRadius=10;
		public int VRadius=10;
		public int Iterations = 1;
		
		
		public void Filter(int width,int height,Color[] srcAndTarget,Color[] buffer) {
			
			for (int i = 0; i < Iterations; i++ ) {
				BlurAndTranspose( srcAndTarget, buffer, width, height, HRadius );
				BlurAndTranspose( buffer, srcAndTarget, height, width, VRadius );
			}
			
		}
		
		/// <summary>Does a horizontal blur, writing the result in a transposed set.
		/// The transpose simply means the function can be used twice in order to get horizontal and vertical.</summary>
		public static void BlurAndTranspose( Color[] src,Color[] target, int width, int height, int radius ){
			
			int widthMinus1 = width-1;
			int tableSize = 2*radius+1;
			int widthMinusRadius=width-radius;
			float tableScale=1f / (float)tableSize;
			
			int inIndex = 0;
			
			
			for ( int y = 0; y < height; y++ ) {
				int outIndex = y;
				
				float tr = 0f;
				float tg = 0f;
				float tb = 0f;
				float ta = 0f;
				
				Color pix = src[inIndex];
				
				for ( int i = -radius; i < 0; i++ ) {
					tr+=pix.r;
					tg+=pix.g;
					tb+=pix.b;
					ta+=pix.a;
				}
				
				for ( int i = 0; i <= radius; i++ ) {
					
					int offset;
					
					if(i > widthMinus1){
						offset=widthMinus1;
					}else{
						offset=i;
					}
					
					pix = src[inIndex + offset];
					tr+=pix.r;
					tg+=pix.g;
					tb+=pix.b;
					ta+=pix.a;
					
				}

				for ( int x = -radius; x < widthMinusRadius; x++ ) {
					
					target[ outIndex ] = new Color( tr * tableScale , tg * tableScale , tb * tableScale, ta * tableScale);
					
					int i1 = x+tableSize;
					
					if ( i1 > widthMinus1 ){
						i1 = widthMinus1;
					}
					
					if ( x < 0 ){
						x = 0;
					}
					
					// Incoming pixel:
					Color rgb1 = src[inIndex+i1];
					
					// Outgoing pixel:
					Color rgb2 = src[inIndex+x];
					
					// Gain one pixel and loose the other.
					tr+=rgb1.r - rgb2.r;
					tg+=rgb1.g - rgb2.g;
					tb+=rgb1.b - rgb2.b;
					ta+=rgb1.a - rgb2.a;
					
					outIndex += height;
					
				}
				
				inIndex += width;
			}
			
		}
		
	}

}