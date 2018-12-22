//--------------------------------------
//          Blaze Rasteriser
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace Blaze{
	
	/// <summary>
	/// Represents a scan line across a glyph. Used during the raster process.
	/// Note that some lines may be empty.
	/// </summary>
	
	public class ScannerScanLine{
		
		/// <summary>Pixels on this scan line.</summary>
		public int Count;
		/// <summary>The scanner that this is a line within.</summary>
		public Scanner Scanner;
		/// <summary>The last scan pixel on this line.</summary>
		public ScannerPixel Last;
		/// <summary>The first pixel on this line.</summary>
		public ScannerPixel First;
		
		
		public ScannerScanLine(Scanner scanner){
			
			Scanner=scanner;
			
		}
		
		/// <summary>Clears this scan line.</summary>
		public void Clear(){
			
			if(First==null){
				return;
			}
			
			if(Scanner.PooledPixels<Scanner.MaxPoolCount){
				
				// We're ok to add to the pool.
				// This will likely go over max, but that's fine - these things are tiny.
				
				Scanner.PooledPixels+=Count;
				Last.Next=Scanner.FirstPooled;
				Scanner.FirstPooled=First;
			
			}
			
			First=null;
			Last=null;
			Count=0;
			
		}
		
		/// <summary>Flattens this scan lines data into our base set of pixels.</summary>
		public SubScanPixel[] Flatten(){
			
			// Create the set:
			SubScanPixel[] set=new SubScanPixel[Count];
			
			// Copy each:
			ScannerPixel current=First;
			
			int index=0;
			
			while(current!=null){
				
				// Create and add in our pixel:
				set[index]=new SubScanPixel((ushort)current.X,current.Fill);
				
				// Increase the index:
				index++;
				
				// Hop to the next one:
				current=current.Next;
				
			}
			
			return set;
			
		}
		
		/// <summary>Adds the given intersect pixel to this scan line.</summary>
		public void Add(ScannerPixel pixel){
			
			// Clear prev/next:
			pixel.Previous=null;
			pixel.Next=null;
			
			// Increase the count:
			Count++;
			
			if(First==null){
				
				// One and only:
				First=Last=pixel;
				
				return;
			}
			
			if(pixel.X>Last.X){
				
				// It's the furthest over. Stick it on the end:
				
				pixel.Previous=Last;
				Last=Last.Next=pixel;
			
			}else{
				
				// Find the place it needs to go between:
				ScannerPixel current=Last;
				
				while(current!=null){
					
					if(pixel.X<=current.X){
						
						if(current.Previous!=null){
							
							if(pixel.X<=current.Previous.X){
								
								current=current.Previous;
								continue;
								
							}
							
							// Ensure next is connected too.
							pixel.Previous=current.Previous;
							
							pixel.Previous.Next=pixel;
							
						}else{
							
							// It goes at the start:
							First=pixel;
							
						}
						
						// It goes before current.
						pixel.Next=current;
						
						current.Previous=pixel;
						
						return;
						
					}
					
					current=current.Previous;
				}
				
			}
			
		}
		
	}
	
}