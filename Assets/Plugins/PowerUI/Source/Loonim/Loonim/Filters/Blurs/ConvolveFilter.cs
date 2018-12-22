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

	public enum ConvolveWrapping:int{
		ZeroEdges=0,
		ClampEdges=1,
		WrapEdges=2
	}

	public class ConvolveFilter{
		
		/// <summary>The kernel to use.</summary>
		public Kernel Kernel;
		/// <summary>True if alpha channel is in use.</summary>
		public bool Alpha=true;
		/// <summary>The wrapping to use.</summary>
		public ConvolveWrapping EdgeAction=ConvolveWrapping.ClampEdges;
		
		
		/// <summary>Construct a filter with a null kernel. 
		/// This is only useful if you're going to change the kernel later on.</summary>
		public ConvolveFilter(){
			Kernel=new Kernel(3, 3,new float[9]);
		}

		/// <summary>Construct a filter with the given 3x3 kernel.</summary>
		/// <param name='matrix'>an array of 9 floats containing the kernel</param>
		public ConvolveFilter(float[] matrix){
			Kernel=new Kernel(3, 3, matrix);
		}
		
		/// <summary>Construct a filter with the given 3x3 kernel.</summary>
		/// <param name='rows'>the number of rows in the kernel</param>
		/// <param name='cols'>the number of columns in the kernel</param>
		/// <param name='matrix'>an array of rows*cols floats containing the kernel</param>
		public ConvolveFilter(int rows, int cols, float[] matrix) {
			Kernel=new Kernel(cols, rows, matrix);
		}
		
		/// <summary>Construct a filter with the given 3x3 kernel.</summary>
		/// <param name='kernel'>The kernel.</param>
		public ConvolveFilter(Kernel kernel) {
			Kernel=kernel;
		}
		
		public virtual void Filter(int width,int height,Color[] src,Color[] target) {
			
			Convolve(Kernel,src,target,width,height,Alpha,EdgeAction);
			
		}
		
		/// <summary>Convolve with a 2D kernel</summary>
		public static void Convolve(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, ConvolveWrapping edgeAction) {
			Convolve(kernel, inPixels, outPixels, width, height, true, edgeAction);
		}
		
		/// <summary>Convolve with a 2D kernel</summary>
		public static void Convolve(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, bool alpha, ConvolveWrapping edgeAction) {
			
			if (kernel.Rows == 1){
				ConvolveH(kernel, inPixels, outPixels, width, height, alpha, edgeAction);
			}else if (kernel.Columns == 1){
				ConvolveV(kernel, inPixels, outPixels, width, height, alpha, edgeAction);
			}else{
				ConvolveHV(kernel, inPixels, outPixels, width, height, alpha, edgeAction);
			}
			
		}
		
		/// <summary>Convolve with a 2D kernel</summary>
		public static void ConvolveHV(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, bool alpha, ConvolveWrapping edgeAction) {
			int index = 0;
			float[] matrix = kernel.Matrix;
			int rows = kernel.Rows;
			int cols = kernel.Columns;
			int rows2 = rows/2;
			int cols2 = cols/2;

			for (int y = 0; y < height; y++) {
				
				for (int x = 0; x < width; x++) {
					
					float r = 0f;
					float g = 0f;
					float b = 0f;
					float a = 0f;
					
					for (int row = -rows2; row <= rows2; row++) {
						
						int iy = y+row;
						int ioffset;
						
						if (0 <= iy && iy < height){
							ioffset = iy*width;
						}else if ( edgeAction == ConvolveWrapping.ClampEdges ){
							ioffset = y*width;
						}else if ( edgeAction == ConvolveWrapping.WrapEdges ){
							ioffset = ((iy+height) % height) * width;
						}else{
							continue;
						}
						
						int moffset = cols*(row+rows2)+cols2;
						
						for (int col = -cols2; col <= cols2; col++) {
							float f = matrix[moffset+col];

							if (f != 0) {
								int ix = x+col;
								if (!(0 <= ix && ix < width)) {
									
									if ( edgeAction == ConvolveWrapping.ClampEdges ){
										ix = x;
									}else if ( edgeAction == ConvolveWrapping.WrapEdges ){
										ix = (x+width) % width;
									}else{
										continue;
									}
									
								}
								
								Color pixel=inPixels[ioffset+ix];
								r += f * pixel.r;
								g += f * pixel.g;
								b += f * pixel.b;
								a += f * pixel.a;
								
							}
							
						}
						
					}
					
					outPixels[index++] = new Color(r,g,b,a);
					
				}
				
			}
			
		}

		/**
		 * Convolve with a kernel consisting of one row
		 */
		public static void ConvolveH(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, bool alpha, ConvolveWrapping edgeAction) {
			int index = 0;
			float[] matrix = kernel.Matrix;
			int cols = kernel.Columns;
			int cols2 = cols/2;

			for (int y = 0; y < height; y++) {
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
								
								if ( edgeAction == ConvolveWrapping.ClampEdges ){
									ix = 0;
								}else if ( edgeAction == ConvolveWrapping.WrapEdges ){
									ix = (x+width) % width;
								}
								
							} else if ( ix >= width) {
								
								if ( edgeAction == ConvolveWrapping.ClampEdges ){
									ix = width-1;
								}else if ( edgeAction == ConvolveWrapping.WrapEdges ){
									ix = (x+width) % width;
								}
								
							}
							
							Color pixel=inPixels[ioffset+ix];
							r += f * pixel.r;
							g += f * pixel.g;
							b += f * pixel.b;
							a += f * pixel.a;
							
						}
						
					}
					
					outPixels[index++] = new Color(r,g,b,a);
					
				}
				
			}
			
		}

		/**
		 * Convolve with a kernel consisting of one column
		 */
		public static void ConvolveV(Kernel kernel, Color[] inPixels, Color[] outPixels, int width, int height, bool alpha, ConvolveWrapping edgeAction) {
			int index = 0;
			float[] matrix = kernel.Matrix;
			int rows = kernel.Rows;
			int rows2 = rows/2;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					
					float r = 0f;
					float g = 0f;
					float b = 0f;
					float a = 0f;
					

					for (int row = -rows2; row <= rows2; row++) {
						int iy = y+row;
						int ioffset;
						
						if ( iy < 0 ) {
							
							switch(edgeAction){
								case ConvolveWrapping.ClampEdges:
									ioffset = 0;
								break;
								case ConvolveWrapping.WrapEdges:
									ioffset = ((y+height) % height)*width;
								break;
								default:
									ioffset = iy*width;	
								break;
							}
							
						} else if ( iy >= height) {
							
							switch(edgeAction){
								case ConvolveWrapping.ClampEdges:
									ioffset = (height-1)*width;
								break;
								case ConvolveWrapping.WrapEdges:
									ioffset = ((y+height) % height)*width;
								break;
								default:
									ioffset = iy*width;
								break;
							}
							
						} else{
							ioffset = iy*width;
						}
						
						float f = matrix[row+rows2];

						if (f != 0) {
							
							Color pixel=inPixels[ioffset+x];
							r += f * pixel.r;
							g += f * pixel.g;
							b += f * pixel.b;
							a += f * pixel.a;
							
						}
					}
					
					outPixels[index++] = new Color(r,g,b,a);
					
				}
				
			}
			
		}

	}

}