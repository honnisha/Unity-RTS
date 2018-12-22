using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Percentile filters essentially average a region of the image.
	/// The average weighting used is the percentile.
	/// </summary>
	
	public class PercentileFilter{
		
		public static void Filter(Color[] inPixels, Color[] outPixels, int width, int height,int hRadius,int vRadius,TextureNode percentile){
			
			// Histogram buffer (essentially a horizontal "slice" of the image):
			ushort[] histograms=new ushort[256 * width];
			
			// Red
			PercentileFilter.Filter( inPixels, outPixels, width, height, hRadius, vRadius,0,histograms,percentile );
			
			// Green
			PercentileFilter.Filter( inPixels, outPixels, width, height, hRadius, vRadius,1,histograms,percentile );
			
			// Blue
			PercentileFilter.Filter( inPixels, outPixels, width, height, hRadius, vRadius,2,histograms,percentile );
			
			// Alpha
			PercentileFilter.Filter( inPixels, outPixels, width, height, hRadius, vRadius,3,histograms,percentile );
			
		}
		
		public static void Filter(Color[] inPixels, Color[] outPixels, int width, 
			int height,int hRadius,int vRadius,int ch,ushort[] histograms,TextureNode percentile){
			
			// We're gonna slide the histograms upwards.
			// Essentially each column has a histogram. A [256] with each entry a counter of how many px have that value.
			
			// int verticalSize=(vRadius * 2) + 1;
			// int horizontalSize=(hRadius * 2) + 1;
			
			ushort vPlus1=(ushort)(vRadius+1);
			
			int row=0;
			
			// Histogram init: Add the bottom row of pixels vRadius+1 times.
			for(int i=0;i<width;i++){
				
				Color pixel=inPixels[i];
				
				int value=(int)(pixel[ch] * 255f);
				
				if(value<0){
					value=0;
				}else if(value>255){
					value=255;
				}
				
				histograms[row+value]+=vPlus1;
				
				row+=256;
				
			}
			
			// Histogram init: Next, add vRadius rows.
			int index=width;
			
			for(int y=1;y<=vRadius;y++){
				
				row=0;
				
				for(int x=0;x<width;x++){
					
					Color pixel=inPixels[index];
					
					int value=(int)(pixel[ch] * 255f);
					
					if(value<0){
						value=0;
					}else if(value>255){
						value=255;
					}
					
					histograms[row+value]+=vPlus1;
					
					
					index++;
					row+=256;
					
				}
				
			}
			
			// Ok - Histograms are up and ready to go!
			
			// -> For each pixel, find the percentile value from horizontalSize # of histograms (act like they are virtually merged together).
			// -> Add the next row up to the current pixels histogram, and remove the lower one.
			// This has a kind of "zipper" effect.
			
			/*
			index=0;
			
			for(int y=0;y<height;y++){
				
				row=0;
				
				for(int x=0;x<width;x++){
					
					// - Select horizontalSize histograms.
					// - Find the percentile we want from them.
					
					// Color pixel=inPixels[index]; <- don't actually need this pixel at all.
					// Need the one which is vRadius rows down (to remove), and vRadius rows up (to add).
					
					//histograms[row+value]+=vPlus1;
					
					
					index++;
					row+=256;
					
				}
				
			}
			*/
			
		}
		
	}
	
}