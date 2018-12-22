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

	public class LensBlurFilter{
		
		/// <summary>Set the radius of the kernel, and hence the amount of blur.</summary>
		public float Radius = 10;
		/// <summary>The bloom factor.</summary>
		public float Bloom = 2;
		/// <summary>The threshold beyond which bloom occurs.</summary>
		public float BloomThreshold = 192f / 255f;
		/// <summary>The lens angle.</summary>
		public float Angle = 0;
		/// <summary>Set the number of sides (e.g. 6 for hexagon).</summary>
		public int Sides = 5;
		
		
		public void Filter(int width,int height,Color[] pixels) {
			
			int rows = 1;
			int cols = 1;
			int log2rows = 0;
			int log2cols = 0;
			int iradius = (int)System.Math.Ceiling(Radius);
			int tileWidth = 128;
			int tileHeight = tileWidth;
			
			// int adjustedWidth = (int)(width + iradius*2);
			// int adjustedHeight = (int)(height + iradius*2);
			
			tileWidth = iradius < 32 ? System.Math.Min(128, width+2*iradius) : System.Math.Min(256, width+2*iradius);
			tileHeight = iradius < 32 ? System.Math.Min(128, height+2*iradius) : System.Math.Min(256, height+2*iradius);
			
			while (rows < tileHeight) {
				rows *= 2;
				log2rows++;
			}
			while (cols < tileWidth) {
				cols *= 2;
				log2cols++;
			}
			int w = cols;
			int h = rows;

			tileWidth = w;
			tileHeight = h;//FIXME-tileWidth, w, and cols are always all the same

			FFT fft = new FFT( System.Math.Max(log2rows, log2cols) );
			
			int totalSize=w*h;
			// int totalSize2=totalSize*2;
			
			float[][] mask = new float[2][]{new float[totalSize],new float[totalSize]};
			float[][] gb = new float[2][]{new float[totalSize],new float[totalSize]};
			float[][] ar = new float[2][]{new float[totalSize],new float[totalSize]};

			// Create the kernel
			double polyAngle = System.Math.PI/Sides;
			double polyScale = 1.0f / System.Math.Cos(polyAngle);
			double r2 = Radius*Radius;
			double rangle = Angle * Mathf.Deg2Rad;
			float total = 0;
			int i = 0;
			
			for ( int y = 0; y < h; y++ ) {
				for ( int x = 0; x < w; x++ ) {
					double dx = x-w/2f;
					double dy = y-h/2f;
					double r = dx*dx+dy*dy;
					double f = r < r2 ? 1 : 0;
					
					if (f != 0) {
						
						r = System.Math.Sqrt(r);
						
						if ( Sides != 0 ) {
							double a = System.Math.Atan2(dy, dx)+rangle;
							a = Mod(a, polyAngle*2)-polyAngle;
							f = System.Math.Cos(a) * polyScale;
						}else{
							f = 1;
						}
						
						f = f*r < Radius ? 1 : 0;
					}
					
					total += (float)f;

					mask[0][i] = (float)f;
					mask[1][i] = 0;
					i++;
				}
			}
			
			// Normalize the kernel
			i = 0;
			
			for ( int y = 0; y < h; y++ ) {
				for ( int x = 0; x < w; x++ ) {
					mask[0][i] /= total;
					i++;
				}
			}

			fft.Transform2D( mask[0], mask[1], w, h, true );
			
			int deltaTileY=tileHeight-2*iradius;
			int deltaTileX=tileWidth-2*iradius;
			
			for ( int tileY = -iradius; tileY < height; tileY += deltaTileY ) {
				
				for ( int tileX = -iradius; tileX < width; tileX += deltaTileX ) {
					
					// Debug.Log("Tile: "+tileX+" "+tileY+" "+tileWidth+" "+tileHeight);
					
					// Create a float array from the pixels. Any pixels off the edge of the source image get duplicated from the edge.
					i = 0;
					for ( int y = 0; y < h; y++ ) {
						
						int inY=(y+tileY+iradius);
						
						if(inY<0){
							inY=0;
						}else if(inY>=height){
							inY=height-1;
						}
						
						inY *= width;
						
						for ( int x = 0; x < w; x++ ) {
							
							int inX=(x+tileX+iradius);
							
							if(inX<0){
								inX=0;
							}else if(inX>=width){
								inX=width-1;
							}
							
							// Grab source pixel:
							Color pixel=pixels[inY + inX];
							
							ar[0][i] = pixel.a;
							float r = pixel.r;
							float g = pixel.g;
							float b = pixel.b;
							
							// Bloom...
							if ( r > BloomThreshold )
								r *= Bloom;
	//							r = BloomThreshold + (r-BloomThreshold) * Bloom;
							if ( g > BloomThreshold )
								g *= Bloom;
	//							g = BloomThreshold + (g-BloomThreshold) * Bloom;
							if ( b > BloomThreshold )
								b *= Bloom;
	//							b = BloomThreshold + (b-BloomThreshold) * Bloom;

							ar[1][i] = r;
							gb[0][i] = g;
							gb[1][i] = b;

							i++;
						}
					}
					
					// Transform into frequency space
					fft.Transform2D( ar[0], ar[1], cols, rows, true );
					fft.Transform2D( gb[0], gb[1], cols, rows, true );

					// Multiply the transformed pixels by the transformed kernel
					i = 0;
					for ( int y = 0; y < h; y++ ) {
						for ( int x = 0; x < w; x++ ) {
							float re = ar[0][i];
							float im = ar[1][i];
							float rem = mask[0][i];
							float imm = mask[1][i];
							ar[0][i] = re*rem-im*imm;
							ar[1][i] = re*imm+im*rem;
							
							re = gb[0][i];
							im = gb[1][i];
							gb[0][i] = re*rem-im*imm;
							gb[1][i] = re*imm+im*rem;
							i++;
						}
					}

					// Transform back
					fft.Transform2D( ar[0], ar[1], cols, rows, false );
					fft.Transform2D( gb[0], gb[1], cols, rows, false );

					// Convert back to RGB pixels, with quadrant remapping
					int row_flip = w >> 1;
					int col_flip = h >> 1;
					
					for ( int y = 0; y < w; y++ ) {
					
						int imageY=y+tileY+iradius;
						
						if(imageY<0 || imageY>=height){
							continue;
						}
						
						// Adjust to row:
						imageY*=width;
						
						int ym = y ^ row_flip;
						int yi = ym*cols;
						
						for ( int x = 0; x < w; x++ ) {
							
							int imageX=x+tileX+iradius;
							
							if(imageX<0 || imageX>=width){
								continue;
							}
							
							int xm = yi + (x ^ col_flip);
							
							// Output:
							pixels[imageY + imageX] = new Color(
								ar[1][xm], // r
								gb[0][xm], // g
								gb[1][xm], // b
								ar[0][xm] // a
							);
							
						}
					}
					
				}
			}
			
		}
		
		private double Mod(double a,double b){
			int n = (int)(a/b);
			
			a -= n*b;
			
			if (a < 0){
				return a + b;
			}
			
			return a;
		}
		
	}
	
}
