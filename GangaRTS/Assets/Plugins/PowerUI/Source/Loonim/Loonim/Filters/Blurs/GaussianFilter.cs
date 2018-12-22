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

	public class GaussianFilter : ConvolveFilter{
		
		public float Radius;
		
		
		public GaussianFilter():this(2f){}
		
		public GaussianFilter(float radius){
			Radius=radius;
			Kernel=MakeKernel(radius);
		}
		
		public override void Filter(int width,int height,Color[] srcAndTarget,Color[] buffer) {
			
			ConvolveAndTranspose(Kernel, srcAndTarget, buffer, width, height, Alpha, ConvolveWrapping.ClampEdges);
			ConvolveAndTranspose(Kernel, buffer, srcAndTarget, height, width, Alpha, ConvolveWrapping.ClampEdges);
			
		}
		
		public static void ConvolveAndTranspose(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, bool alpha, ConvolveWrapping edgeAction) {
			float[] matrix = kernel.Matrix;
			int cols = kernel.Columns;
			int cols2 = cols/2;

			for (int y = 0; y < height; y++) {
				
				int index = y;
				int ioffset = y*width;
				
				for (int x = 0; x < width; x++) {
					
					float r = 0f;
					float g = 0f;
					float b = 0f;
					float a = 0f;
					
					int moffset = cols2;
					
					for (int col = -cols2; col <= cols2; col++) {
						float f = matrix[moffset+col];

						if (f != 0) {
							int ix = x+col;
							
							if ( ix < 0 ) {
								if ( edgeAction == ConvolveWrapping.ClampEdges )
									ix = 0;
								else if ( edgeAction == ConvolveWrapping.WrapEdges )
									ix = (x+width) % width;
							} else if ( ix >= width) {
								if ( edgeAction == ConvolveWrapping.ClampEdges )
									ix = width-1;
								else if ( edgeAction == ConvolveWrapping.WrapEdges )
									ix = (x+width) % width;
							}
							
							Color pixel=inPixels[ioffset+ix];
							r += f * pixel.r;
							g += f * pixel.g;
							b += f * pixel.b;
							a += f * pixel.a;
							
						}
						
					}
					
					outPixels[index] = new Color(r,g,b,a);
					index += height;
					
				}
				
			}
			
		}

		/// <summary>Makes a Gaussian blur kernel.</summary>
		public static Kernel MakeKernel(float radius) {
			
			int r = (int)System.Math.Ceiling(radius);
			
			int rows = r*2+1;
			
			float[] matrix = new float[rows];
			
			float sigma = radius/3f;
			float sigma22 = 2f*sigma*sigma;
			float sigmaPi2 = 2f*(float)System.Math.PI*sigma;
			float sqrtSigmaPi2 = (float)System.Math.Sqrt(sigmaPi2);
			float radius2 = radius*radius;
			float total = 0;
			int index = 0;
			
			for (int row = -r; row <= r; row++) {
				float distance = row*row;
				
				if (distance > radius2){
					matrix[index] = 0;
				}else{
					matrix[index] = (float)System.Math.Exp(-(distance)/sigma22) / sqrtSigmaPi2;
				}
				
				total += matrix[index];
				index++;
			}
			
			for (int i = 0; i < rows; i++){
				matrix[i] /= total;
			}
			
			return new Kernel(rows, 1, matrix);
			
		}
		
	}

}