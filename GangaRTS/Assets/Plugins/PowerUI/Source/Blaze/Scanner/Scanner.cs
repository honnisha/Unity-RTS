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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Blaze{

	/// <summary>
	/// Performs rasterisation scanning of glyphs/ vectors.
	/// </summary>
	
	public class Scanner:PointReceiver{
		
		/// <summary>The size of the SDF "blur" (distance field) in pixels around each vector.</summary>
		public int BlurSpread=10;
		/// <summary>Read only. Use Scale or ScaleX. A scalar value applied to all vectors rendered by this scanner.</summary>
		public float ScalarX=1f;
		/// <summary>Read only. Use Scale or ScaleX.  A scalar value applied to all vectors rendered by this scanner.</summary>
		public float ScalarY=1f;
		
		/// <summary>The distance between each row of pixels. Set automatically.</summary>
		internal float RawSampleDistance;
		/// <summary>The max pixel count in the pool.</summary>
		internal const int MaxPoolCount=50;
		/// <summary>The number of pixels in the pool.</summary>
		internal int PooledPixels;
		/// <summary>Shared pool of vector verts.</summary>
		
		/// <summary>Shared grid-cache. It's used for rapid distance checking. It's a 2D grid. x + (width*y).</summary>
		internal DistanceCacheSquare[] DistanceCache;
		/// <summary>The width of the distance cache.</summary>
		internal int DistanceCacheWidth;
		/// <summary>The height of the distance cache.</summary>
		internal int DistanceCacheHeight;
		
		/// <summary>True if the raster result is "blurred" (actually an SDF).</summary>
		private bool Blurred=true;
		/// <summary>Computed when the first vector is rasterised. BlurSpread * 2.</summary>
		private int DoubleBlurSpread;
		/// <summary>Computed when the first vector is rasterised. BlurSpread / 2.</summary>
		private int HalfBlurSpread;
		/// <summary>A vertical offset value applied to each point. Used for example on glyphs which extend below the baseline.</summary>
		public float VerticalOffset;
		/// <summary>A horizontal offset value applied to each point. Used for example on glyphs which are offset and would otherwise waste space.</summary>
		public float HorizontalOffset;
		/// <summary>Shared pool of scanner pixels.</summary>
		internal ScannerPixel FirstPooled;
		/// <summary>A globally shared scan line buffer.</summary>
		private ScannerScanLine[] ScanLineBuffer;
		/// <summary>True if this scanner needs some values to be setup. Set this true if you e.g. change the blur spread.</summary>
		public bool RequiresStart=true;
		/// <summary>Used in SDF rendering. The max distance a pixel can be from the outline. Same as BlurSpread, but as a float.</summary>
		private float MaxDistance;
		/// <summary>The square of the maximum distance. Used in SDF rendering.</summary>
		private float MaxDistanceSquared;
		/// <summary>Used in SDF rendering. Maps a distance value to being in 0-255 byte range.</summary>
		private float DistanceAdjuster;
		/// <summary>The raw spread of SDF rastered vectors. See SDFSize.</summary>
		private int RawSDFSize;
		/// <summary>Used whilst adding points to scanlines. Did we last go up or down.</summary>
		private bool WentUp;
		/// <summary>Used whilst adding points to scanlines. The Y line that was moved to.</summary>
		private int MoveToY;
		/// <summary>Used whilst adding points to scanlines. The previous visited line.</summary>
		private int PreviousY;
		/// <summary>The cached "line change" point's X value.</summary>
		private int LineChangeY=-1;
		/// <summary>The cached "line change" point's X value. The line change point is essentially the first point that is not on the moveTo line.</summary>
		private float LineChangeX;
		/// <summary>Did the line change point go up?</summary>
		private bool LineChangeWentUp;
		
		
		/// <summary>The distance between each row of pixels. Set automatically.</summary>
		public float SampleDistance{
			get{
				return RawSampleDistance;
			}
			set{
				RawSampleDistance=value;
			}
		}
		
		/// <summary>Changes the "spread" of SDF rastered vectors.</summary>
		public int SDFSize{
			get{
				return RawSDFSize;
			}
			set{
				
				if(value<0){
					value=0;
				}
				
				RawSDFSize=value;
				BlurSpread=value;
				RequiresStart=true;
				
				// Apply blurred right now incase SDF is queried:
				Blurred=(value!=0);
				
			}
		}
		
		
		/// <summary>Change this scanners draw mode. Defaults to SDF draw mode.</summary>
		public bool SDF{
			get{
				return Blurred;
			}
			set{
				if(value==Blurred){
					return;
				}
				
				RequiresStart=true;
				
				if(value){
					BlurSpread=RawSDFSize;
				}else{
					BlurSpread=0;
				}
				
			}
		}
		
		/// <summary>The scale applied to all vectors rendered with this scanner.</summary>
		public float ScaleY{
			get{
				return ScalarY;
			}
			set{
				ScalarY=value;
			}
		}
		
		/// <summary>The scale applied to all vectors rendered with this scanner.</summary>
		public float ScaleX{
			get{
				return ScalarX;
			}
			set{
				ScalarX=value;
			}
		}
		
		/// <summary>The scale applied to all vectors rendered with this scanner.</summary>
		public float Scale{
			get{
				return ScalarX;
			}
			set{
				ScalarX=value;
				ScalarY=value;
			}
		}
		
		/// <summary>Change the SDF default draw height of the scanner.</summary>
		public int DrawHeight{
			get{
				return (int)ScalarX;
			}
			set{
				if(value==DrawHeight){
					return;
				}
				
				Scale=(float)value;
				
				RequiresStart=true;
			}
		}
		
		/// <summary>Called automatically when the first vector is rasterized.</summary>
		public void Start(){
			
			RequiresStart=false;
			
			Blurred=(BlurSpread!=0);
			
			// Write sample distance:
			SampleDistance=1f/ScalarX;
			
			// Double the blur spread:
			DoubleBlurSpread=(BlurSpread*2);
			
			// And half it:
			HalfBlurSpread=BlurSpread/2;
			
			if(Blurred){
				
				// Cache sized such that by default it can handle upto and including super-wide (2W) characters.
				DistanceCacheHeight=(((int)ScalarX+DoubleBlurSpread)/BlurSpread);
				DistanceCacheWidth=DistanceCacheHeight*2;
				
				// Set the max distance:
				MaxDistance=(float)BlurSpread;
				
				// Figure out some additional distance values:
				MaxDistanceSquared=MaxDistance * MaxDistance;
				DistanceAdjuster=255f / MaxDistance;
				
				
				CreateDistanceCache();
			}else{
				DistanceCache=null;
			}
			
		}
		
		public void CreateDistanceCache(){
			
			// Create the caches:
			DistanceCache=new DistanceCacheSquare[DistanceCacheWidth * DistanceCacheHeight];
			
			// The cell square index:
			int index=0;
			
			// For each cell..
			for(int y=0;y<DistanceCacheHeight;y++){
				
				for(int x=0;x<DistanceCacheWidth;x++){
					
					// Create it:
					DistanceCache[index]=new DistanceCacheSquare(x,y,index);
					
					index++;
					
				}
				
			}
			
			index=0;
			
			// For each cell..
			for(int y=0;y<DistanceCacheHeight;y++){
				
				for(int x=0;x<DistanceCacheWidth;x++){
					
					// Set it up (done like this as it accesses other squares in cache):
					DistanceCache[index].Setup(this);
					
					index++;
					
				}
				
			}
			
		}
		
		public void MoveTo(float x,float y){
			
			// Cache spread:
			int blurOffset=HalfBlurSpread;
			
			y+=VerticalOffset;
			
			y*=ScalarY;
			
			// Figure out the scan-Y and distance-cache Y value:
			int curY=((int)y) + blurOffset;
			
			PreviousY=curY;
			MoveToY=curY;
		}
		
		/// <summary>Adds the given point as a point on our vector. Note that it's relative at this point.</summary>
		public void AddPoint(float x,float y){
			
			// Cache spread:
			int blurOffset=HalfBlurSpread;
			
			y+=VerticalOffset;
			
			y*=ScalarY;
			
			// Figure out the scan-Y and distance-cache Y value:
			int curY=((int)y) + blurOffset;
		
			// Make x non-relative (height is correct here! Don't distort):
			x+=HorizontalOffset;
			x*=ScalarX;
			
			if(curY!=PreviousY){
				
				// How many lines did we change by?
				int deviation=curY-PreviousY;
				bool goingUp=(deviation>0);
				
				if(MoveToY!=-1){
					
					// Make sure we don't place any pixels on the MoveTo line:
					WentUp=goingUp;
					
					// Cache the point this happened:
					// - We're essentially going to figure out if we need the point or not.
					// - We do this once the close node has been placed.
					LineChangeX=x;
					LineChangeY=MoveToY;
					LineChangeWentUp=goingUp;
					
					// Clear the block:
					MoveToY=-1;
					
				}
				
				int i;
				int max;
				
				// Are we going upwards?
				if(goingUp){
					
					// Did we last go upwards?
					if(WentUp){
						
						i=PreviousY+1;
						max=curY;
						
					}else{
						
						// Went down then back up. Add 2+ points.
						i=PreviousY;
						max=curY;
						
					}
					
				}else{
					
					if(WentUp){
						
						// Went up and back down. Add 2+ points.
						max=PreviousY;
						i=curY;
						
					}else{
						
						max=PreviousY-1;
						i=curY;
						
					}
					
				}
				
				if(i<0){
					i=0;
				}
				
				if(max>=ScanLineBuffer.Length){
					max=ScanLineBuffer.Length-1;
				}
				
				while(i<=max){
					
					AddPixel(x,i);
					
					i++;
					
				}
				
				// Update went up:
				WentUp=goingUp;
				
			}
			
			PreviousY=curY;
			
			if(DistanceCache==null){
				return;
			}
			
			// Add to distance caches too.
			int dX=(((int)x) + blurOffset) / BlurSpread;
			
			if(dX<0 || dX>=DistanceCacheWidth){
				return;
			}
			
			int dY=(curY/BlurSpread);
			
			if(dY<0 || dY>=DistanceCacheHeight){
				return;
			}
			
			// Get the square index:
			int cacheIndex=dX + (dY * DistanceCacheWidth);
			
			// Add:
			DistanceCache[cacheIndex].Add(x + blurOffset,y + blurOffset);
			
		}
		
		/// <summary>Adds a given pixel to the scanline buffer.</summary>
		private void AddPixel(float x,int y){
	
			ScannerPixel pixel;
			
			if(FirstPooled==null){
				pixel=new ScannerPixel();
			}else{
				pixel=FirstPooled;
				FirstPooled=pixel.Next;
				pixel.Next=null;
				PooledPixels--;
			}
			
			// Set it up:
			pixel.X=x + HalfBlurSpread;
			
			// Add it:
			ScanLineBuffer[y].Add(pixel);
			
		}
		
		/// <summary>Clears out all node entries in the distance cache.</summary>
		public void ReindexDistanceCache(){
			
			if(DistanceCache==null){
				
				// Not got one!
				return;
				
			}
			
			for(int i=0;i<DistanceCache.Length;i++){
				
				DistanceCache[i].RecalculateIndex(this);
				
			}
			
		}
		
		/// <summary>Clears out all node entries in the distance cache.</summary>
		public void ClearDistanceCache(){
			
			if(DistanceCache==null){
				
				// Not got one!
				return;
				
			}
			
			for(int i=0;i<DistanceCache.Length;i++){
				
				DistanceCache[i].Clear();
				
			}
			
		}
		
		/// <summary>Rasterises a generic vector.</summary>
		public bool Rasterise(VectorPath glyph,Color32[] atlasPixels,int atlasWidth,int baseIndex,int width,int height,float hOffset,float vOffset,Color32 fill,bool clear){
			
			if(glyph.FirstPathNode==null){
				return false;
			}
			
			if(RequiresStart){
				// Start now:
				Start();
			}
			
			PreviousY=-1;
			
			// Offset the rasteriser:
			VerticalOffset=vOffset;
			HorizontalOffset=hOffset;
			
			
			if(Blurred){
				
				// Add some space for the blur:
				width+=BlurSpread;
				height+=BlurSpread;
				
			}
			
			if(clear){
				int atlasIndex=baseIndex;
				
				for(int i=0;i<height;i++){
					
					// Clear the row:
					Array.Clear(atlasPixels,atlasIndex,width);
					
					atlasIndex+=atlasWidth;
					
				}
				
			}
			
			if(Blurred){
				
				if( width > (DistanceCacheWidth * BlurSpread) ){
					
					// Resize time!
					DistanceCacheWidth = ((width-1)/BlurSpread)+1;
					
					// Rebuild:
					CreateDistanceCache();
					
				}else if(height > (DistanceCacheHeight * BlurSpread)){
					
					// Resize time!
					DistanceCacheHeight = ((height-1)/BlurSpread)+1;
					
					// Rebuild:
					CreateDistanceCache();
					
				}else{
					
					// Clear the cache:
					ClearDistanceCache();
					
				}
				
			}
			
			if(ScanLineBuffer==null || ScanLineBuffer.Length<=height){
				
				// Create the shared buffer:
				ScanLineBuffer=new ScannerScanLine[height+1];
				
			}
			
			// Clear the buffer:
			for(int i=0;i<=height;i++){
				
				// Grab the line:
				ScannerScanLine line=ScanLineBuffer[i];
				
				if(line==null){
					
					// Create it now:
					ScanLineBuffer[i]=new ScannerScanLine(this);
					
					continue;
					
				}
				
				// Empty the line (into the pool):
				line.Clear();
				
			}
			
			// For each line in the glyph, resolve it into a series of points.
			// Each one is at most one pixel away from the previous point.
			
			// Get the next point (exists due to check above):
			VectorPoint point=glyph.FirstPathNode;
			
			while(point!=null){
				
				// Compute the points along the line which are fairly dense and regularly spaced (when possible):
				point.ComputeLinePoints(this);
				
				if((point.IsClose || point.Next==null) && LineChangeY!=-1 && MoveToY==-1){
					// Test if we need to add our special point:
					
					if(LineChangeWentUp!=WentUp){
						
						if(LineChangeY>=0 && LineChangeY<ScanLineBuffer.Length){
							AddPixel(LineChangeX,LineChangeY);
						}
						
					}
					
					// Clear Y:
					LineChangeY=-1;
				}
				
				// Go to the next one:
				point=point.Next;
				
			}
			
			int blurOffset=HalfBlurSpread;
			
			// For each scan line, skipping the ones we know won't contain anything at this point:
			int verticalMax=height-blurOffset;
			
			// Stop only one blurspread from the end - we know there's nothing beyond that.
			int lineMax=width-blurOffset;
			
			// The current pixel index - offset over the blur spread:
			int pixelIndex=baseIndex + (atlasWidth * blurOffset);
			
			if(Blurred){
				
				// SDF mode
				
				for(int i=blurOffset;i<verticalMax;i++){
					
					// Grab the line:
					ScannerScanLine line=ScanLineBuffer[i];
					
					if(line.First==null){
						
						// Advance to the next line:
						pixelIndex+=atlasWidth;
						
						continue;
						
					}
					
					// Grab the first pixel (each one represents an intersect):
					ScannerPixel inwardPixel=line.First;
					
					while(inwardPixel!=null){
						
						// Grab the next one:
						ScannerPixel outwardPixel=inwardPixel.Next;
						
						
						if(outwardPixel==null){
							break;
						}
						
						// Index of the last pixel (exclusive):
						int max=outwardPixel.PixelIndex;
						
						if(max>lineMax){
							// Clip it:
							max=lineMax;
						}
						
						// The section from inwardPixel to outwardPixel is filled.
						
						for(int p=inwardPixel.PixelIndex;p<max;p++){
							
							// Fill:
							atlasPixels[pixelIndex+p]=fill;
							
						}
						
						if(outwardPixel==null){
							
							// No more pairs:
							break;
							
						}
						
						// Go to the next intersect:
						inwardPixel=outwardPixel.Next;
						
					}
					
					// Advance to the next line:
					pixelIndex+=atlasWidth;
					
				}
				
				// "blur" time!
				
				int cacheSquareIndex=0;
				int blurSpread=BlurSpread;
				int atlasSize=atlasPixels.Length;
				
				// For each distance cache square..
				for(int dY=0;dY<DistanceCacheHeight;dY++){
					
					for(int dX=0;dX<DistanceCacheWidth;dX++){
						
						// Get the square:
						DistanceCacheSquare currentSquare=DistanceCache[cacheSquareIndex];
						
						pixelIndex=baseIndex+(currentSquare.PixelIndexY * atlasWidth)+currentSquare.PixelIndexX;
						
						if(currentSquare.InRange){
							
							// Get the FX/FY:
							float squareFX=currentSquare.XOffset;
							
							float fY=currentSquare.YOffset;
							float fX=squareFX;
							
							// Grab the squares search set:
							DistanceCacheSquare[] toSearch=currentSquare.SearchSet;
							
							// How many?
							int searchCount=toSearch.Length;
							
							// Reset pixel index:
							pixelIndex=baseIndex+(currentSquare.PixelIndexY * atlasWidth)+currentSquare.PixelIndexX;
							
							// For each pixel in this square..
							for(int y=0;y<blurSpread;y++){
								
								if(pixelIndex>=atlasSize){
									break;
								}
								
								for(int x=0;x<blurSpread;x++){
									
									// Where's the pixel?
									int fullIndex=pixelIndex+x;
									
									if(fullIndex>=atlasSize){
										break;
									}
									
									// Must be black otherwise we'll ignore it.
									if(atlasPixels[fullIndex].r!=0){
										
										// It has colour. Skip.
										
										// Move float x along:
										fX++;
										
										continue;
										
									}
									
									// Start doing distance tests - look in all the nearest squares:
									float bestDistance=float.MaxValue;
									
									for(int i=0;i<searchCount;i++){
										
										DistanceCacheSquare square=toSearch[i];
										
										if(square.Count==0){
											// Ignore empty square.
											continue;
										}
										
										// Temp grab the point set:
										List<DistanceCachePoint> points=square.Points;
										
										// How many points?
										int pointCount=points.Count;
										
										// For each node in the square, find the (relative) nearest.
										for(int p=0;p<pointCount;p++){
											
											// Get the point:
											DistanceCachePoint cachePoint=points[p];
											
											// Distance check:
											float deltaX=cachePoint.X - fX;
											
											float deltaY=cachePoint.Y - fY;
											
											// Full distance:
											float distance=(deltaX * deltaX) + (deltaY * deltaY);
											
											if(distance<bestDistance){
												
												// Closest distance found:
												bestDistance=distance;
												
											}
											
										}
										
									}
									
									// Result value:
									byte distancePixel;
									
									if(bestDistance>MaxDistanceSquared){
										
										// The pixel should be black:
										distancePixel=0;
										
									}else{
										
										// Map the distance to an accurate distance, and put it in range:
										distancePixel=(byte)(255f-(Math.Sqrt(bestDistance) * DistanceAdjuster));
										
									}
									
									// Write the result:
									atlasPixels[fullIndex]=new Color32(fill.r,fill.g,fill.b,distancePixel);
									
									// Move float x along:
									fX++;
									
								}
								
								// Move float y along:
								fY++;
								fX=squareFX;
								
								// Move pixel index up a row:
								pixelIndex+=atlasWidth;
								
							}
							
						}
						
						cacheSquareIndex++;
						
					}
					
				}
				
			}else{
				
				// Got alpha?
				bool fillAlpha=(fill.a!=255 && !clear);

				// "Invert" fill alpha:
				int alphaInvert=255-fill.a;
				int fillRA=fill.r * fill.a;
				int fillGA=fill.g * fill.a;
				int fillBA=fill.b * fill.a;
				
				
				for(int i=blurOffset;i<verticalMax;i++){
					
					// Grab the line:
					ScannerScanLine line=ScanLineBuffer[i];
					
					if(line.First==null){
						
						// Advance to the next line:
						pixelIndex+=atlasWidth;
						
						continue;
						
					}
					
					// Grab the first pixel (each one represents an intersect):
					ScannerPixel inwardPixel=line.First;
					
					while(inwardPixel!=null){
						
						// Grab the next one:
						ScannerPixel outwardPixel=inwardPixel.Next;
						
						
						if(outwardPixel==null){
							break;
						}
						
						// Index of the last pixel (exclusive):
						int max=outwardPixel.PixelIndex;
						
						if(max>lineMax){
							// Clip it:
							max=lineMax;
						}
						
						// The section from inwardPixel to outwardPixel is filled.
						
						if(fillAlpha){
							
							for(int p=inwardPixel.PixelIndex;p<max;p++){
								
								// Get the index:
								int index=pixelIndex+p;
								
								// Grab the colour:
								Color32 colour=atlasPixels[index];
								
								// Alpha blend:
								int dstA=(colour.a * alphaInvert) / 255;
								int cA=fill.a + dstA;
								
								colour.a=(byte)cA;
								colour.r=(byte) ((fillRA + colour.r * dstA)/cA);
								colour.g=(byte) ((fillGA + colour.g * dstA)/cA);
								colour.b=(byte) ((fillBA + colour.b * dstA)/cA);
								
								// Fill:
								atlasPixels[index]=colour;
								
							}
							
						}else{
						
							for(int p=inwardPixel.PixelIndex;p<max;p++){
								
								// Fill:
								atlasPixels[pixelIndex+p]=fill;
								
							}
							
						}
						
						if(outwardPixel==null){
							
							// No more pairs:
							break;
							
						}
						
						// Go to the next intersect:
						inwardPixel=outwardPixel.Next;
						
					}
					
					// Advance to the next line:
					pixelIndex+=atlasWidth;
					
				}
				
				
			}
			
			return true;
			
			/*
			// Create the output.
			SubScanPixel[][] intersects=new SubScanPixel[height][];
			
			for(int i=0;i<height;i++){
				
				// Flatten the scan line into the pixel buffer:
				intersects[i]=ScanLineBuffer[i].Flatten();
				
			}
			
			// Finally create the raster:
			RasterGlyph raster=new RasterGlyph();
			
			// Apply intersects:
			raster.Intersects=intersects;
			
			// Apply width:
			raster.Width=width;
			
			return raster;
			*/
			
		}
		
	}
	
}