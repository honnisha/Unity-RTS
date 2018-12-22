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
using System.Collections;
using System.Collections.Generic;


namespace Blaze{
	
	/// <summary>
	/// Each glyph is first broken down into nodes around it's edge (about 50 on average).
	/// These nodes are then added into a block of cells in a grid, aka distance cache squares, which they are "in range" of.
	/// Each output pixel then checks which of the nodes in the cell it's in is the nearest, and then bases its final colour from that.
	/// </summary>
	
	internal class DistanceCacheSquare{
		
		/// <summary>X coordinate of this cell in the distance grid.</summary>
		internal int X;
		/// <summary>Y coordinate of this cell in the distance grid.</summary>
		internal int Y;
		/// <summary>Array-level index of this cell in the grid.</summary>
		internal int Index;
		/// <summary>Points in this cache square.</summary>
		internal int Count;
		/// <summary>X as a float, multiplied by BlurSpread</summary>
		internal float XOffset;
		/// <summary>Y as a float, multiplied by BlurSpread</summary>
		internal float YOffset;
		/// <summary>True if this square contains points or is in range of one that does.</summary>
		internal bool InRange;
		/// <summary>The x index of the pixel in the bottom left corner of this square.</summary>
		internal int PixelIndexX;
		/// <summary>The y index of the pixel in the bottom left corner of this square.</summary>
		internal int PixelIndexY;
		/// <summary>A set of cache squares (including this one) which should be searched when running proximity tests from within this square.</summary>
		internal DistanceCacheSquare[] SearchSet;
		/// <summary>All the points in this square.</summary>
		internal List<DistanceCachePoint> Points=new List<DistanceCachePoint>();
	
		
		internal DistanceCacheSquare(int x,int y,int index){
			X=x;
			Y=y;
			Index=index;
		}
		
		/// <summary>Called just after the whole grid has been created.</summary>
		internal void Setup(Scanner scanner){
		
			// Create the search set.
			
			// Get bounds:
			int maxX=scanner.DistanceCacheWidth;
			int maxY=scanner.DistanceCacheHeight;
			
			// How many?
			int count=9;
			
			// Is this square at the max points?
			bool boundsX=(X==0 || X==(maxX-1));
			bool boundsY=(Y==0 || Y==(maxY-1));
			
			if(boundsX && boundsY){
				
				count=4;
				
			}else if(boundsX || boundsY){
				
				count=6;
				
			}
			
			// Create the set:
			SearchSet=new DistanceCacheSquare[count];
			
			int setIndex=0;
			
			// Add each one (in a 3x3 block):
			for(int y=Y-1;y<=(Y+1);y++){
				
				if(y<0 || y==maxY){
					// Out of range.
					continue;
				}
				
				for(int x=X-1;x<=(X+1);x++){
					
					if(x<0 || x==maxX){
						// Out of range.
						continue;
					}
					
					// Get the index:
					int squareIndex=x+(y * scanner.DistanceCacheWidth);
					
					// Get the square:
					SearchSet[setIndex]=scanner.DistanceCache[squareIndex];
					
					setIndex++;
					
				}
				
			}
			
			RecalculateIndex(scanner);
			
		}
		
		internal void RecalculateIndex(Scanner scanner){
			
			PixelIndexX=X * scanner.BlurSpread;
			PixelIndexY=Y * scanner.BlurSpread;
			
			// Apply x/y:
			XOffset=((float)PixelIndexX) + 0.5f;
			YOffset=((float)PixelIndexY) + 0.5f;
			
		}
		
		/// <summary>Clears this square.</summary>
		internal void Clear(){
			
			InRange=false;
			Count=0;
			Points.Clear();
			
		}
		
		/// <summary>Adds the given glyph outline point to this square.</summary>
		internal void Add(float x,float y){
			
			if(Count==0){
				// Set this and all in range as "InRange" (of something). Includes this square too.
				
				for(int i=0;i<SearchSet.Length;i++){
					SearchSet[i].InRange=true;
				}
				
			}
			
			Count++;
			Points.Add(new DistanceCachePoint(x,y));
			
		}
		
	}
	
}