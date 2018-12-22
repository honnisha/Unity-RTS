//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;
using Blaze;


namespace InfiniText{

	public static class GlyfTables{
		
		public static Glyph[] Load(FontParser parser,int start,uint[] locations,FontFace font){
			
			// Get the vertical range of the font - it's the em size.
			float range=font.UnitsPerEmF;
			
			// The number of glyphs (last location is just used for computation purposes).
			int glyphCount=locations.Length-1;
			
			Glyph[] glyphs=new Glyph[glyphCount];
			font.ParserGlyphs=glyphs;
			
			// For each glyph..
			for(int i=0;i<glyphCount;i++){
				
				uint offset=locations[i];
				uint nextOffset=locations[i+1];
				
				if(offset!=nextOffset){
					
					// Seek there now:
					parser.Position=start+(int)offset;
					
					// Load it:
					glyphs[i]=ParseGlyph(parser,font,range);
					
				}else{
					
					glyphs[i]=new Glyph(font);
					
				}
				
			}
			
			if(Fonts.Preload){
				
				// Composite glyphs next.
				for(int i=0;i<glyphCount;i++){
					
					Glyph glyph=glyphs[i];
					
					if(glyph!=null){
						glyph.LoadFully(glyphs);
					}
					
				}
				
			}
			
			return glyphs;
			
		}
		
		/// <summary>Used when delaying the loading of a glyph. This results in rapid startup.</summary>
		public static void LoadFully(Glyph glyph,FontParser parser,LoadMetaPoint meta){
			
			parser.Position=meta.Start;
			
			LoadGlyph(glyph,meta.Length,parser,glyph.Font.UnitsPerEmF);
			
		}
		
		private static void LoadGlyph(Glyph glyph,int contourCount,FontParser parser,float range){
			
			// The contour count (tiny set):
			ushort[] endPointIndices=new ushort[contourCount];
			
			// Load each endpoint:
			for(int i=0;i<contourCount;i++){
				
				endPointIndices[i]=parser.ReadUInt16();
				
			}
			
			// How big is the instruction block?
			int instructionLength=parser.ReadUInt16();
			
			// And skip it!
			parser.Position+=instructionLength;
			
			// How many coordinates?
			int numberOfCoordinates=endPointIndices[endPointIndices.Length-1]+1;
			
			// Create the flag set:
			byte[] flags=new byte[numberOfCoordinates];
			
			// For each one..
			for (int i = 0; i < numberOfCoordinates;i++) {
				
				byte flag=parser.ReadByte();
				
				flags[i]=flag;
				
				// If bit 3 is set, we repeat this flag n times, where n is the next byte.
				if((flag&8)>0){
					int repeatCount = parser.ReadByte();
					
					for (int j = 0; j < repeatCount; j += 1) {
						i++;
						flags[i]=flag;
					}
				}
				
			}
			
			if (endPointIndices.Length > 0) {
				
				// X/Y coordinates are relative to the previous point, except for the first point which is relative to 0,0.
				if (numberOfCoordinates > 0){
					
					// Current coord:
					int coord=0;
					
					// Coord index:
					int coordIndex=0;
					
					// The coord set:
					float[] coords=new float[numberOfCoordinates*2];
					
					// Load X coords:
					for (int i = 0; i < numberOfCoordinates; i++) {
						byte flag = flags[i];
						
						coord = LoadGlyphCoordinate(parser,flag,coord,2,16);
						
						coords[coordIndex]=(float)coord/range;
						
						coordIndex+=2;
						
					}
					
					// Reset shared vars:
					coord=0;
					coordIndex=1;
					
					// Load Y coords:
					for (int i = 0; i < numberOfCoordinates; i++) {
						byte flag = flags[i];
						
						coord = LoadGlyphCoordinate(parser,flag,coord,4,32);
						
						coords[coordIndex]=(float)coord/range;
						
						coordIndex+=2;
						
					}
					
					int[] orderedEnds=new int[endPointIndices.Length];
					int currentEnd=0;
					
					for (int i = 0; i < numberOfCoordinates; i++) {
						
						// Grab the flag:
						byte flag=flags[i];
						
						// On curve flag - Control point otherwise:
						flag=(byte)(flag&1);
						
						// Last point of the current contour?
						
						// For each end point index (tiny set - better than hash):
						for(int e=endPointIndices.Length-1;e>=0;e--){
							
							if(endPointIndices[e]==i){
								
								orderedEnds[currentEnd]=i;
								
								currentEnd++;
								
								break;
							}
							
						}
						
						// Update the flag - it's now just a 1 or 0:
						flags[i]=flag;
						
					}
					
					// Reset shared index again:
					coordIndex=0;
					
					// Create our temp holders of point info:
					GlyphPoint firstPointRaw=new GlyphPoint(flags,coords);
					GlyphPoint lastPointRaw=new GlyphPoint(flags,coords);
					GlyphPoint prevPointRaw=new GlyphPoint(flags,coords);
					GlyphPoint currentPointRaw=new GlyphPoint(flags,coords);
					GlyphPoint controlPoint=new GlyphPoint(flags,coords);
					
					// For each contour..
					for(int i=0;i<contourCount;i++){
						
						int pointOffset=0;
						
						// Get the indices of the first/last points on this contour.
						int firstIndex=0;
						int lastIndex=orderedEnds[i];
						
						if(i!=0){
							firstIndex=orderedEnds[i-1]+1;
						}
						
						GlyphPoint firstPoint=firstPointRaw;
						firstPoint.Set(firstIndex);
						
						GlyphPoint lastPoint=lastPointRaw;
						lastPoint.Set(lastIndex);
						
						if(firstPoint.OnCurve){
							
							// No control point:
							controlPoint.Active=false;
							
							// The first point will be consumed by the moveTo command so skip it:
							pointOffset=1;
							
						}else{
							
							if(lastPoint.OnCurve){
								
								// If the first point is off-curve and the last point is on-curve,
								// start at the last point.
								firstPoint=lastPoint;
								
							}else{
								// If both first and last points are off-curve, start at their middle.
								
								firstPoint.X=(firstPoint.X+lastPoint.X)/2f;
								firstPoint.Y=(firstPoint.Y+lastPoint.Y)/2f;
								
							}
							
							controlPoint.Set(firstPoint);
							
						}
						
						glyph.MoveTo(firstPoint.X,firstPoint.Y);
						
						int contourStart=firstIndex+pointOffset;
						
						for(int j=contourStart;j<=lastIndex;j++){
							
							// Setup the previous point:
							GlyphPoint prevPoint;
							
							if(j==firstIndex){
								prevPoint=firstPoint;
							}else{
								prevPoint=prevPointRaw;
								prevPoint.Set(j-1);
							}
							
							// Setup the current point:
							GlyphPoint pt=currentPointRaw;
							pt.Set(j);
							
							if(prevPoint.OnCurve && pt.OnCurve) {
								
								// Just a line here:
								glyph.LineTo(pt.X,pt.Y);
								
							}else if (prevPoint.OnCurve && !pt.OnCurve){
								
								controlPoint.Set(pt);
								
							}else if(!prevPoint.OnCurve && !pt.OnCurve){
								
								float midPointX=(prevPoint.X+pt.X)/2f;
								float midPointY=(prevPoint.Y+pt.Y)/2f;
								
								glyph.QuadraticCurveTo(prevPoint.X,prevPoint.Y,midPointX,midPointY);
								controlPoint.Set(pt);
								
							}else if(!prevPoint.OnCurve && pt.OnCurve){
								
								// Previous point off-curve, this point on-curve.
								glyph.QuadraticCurveTo(controlPoint.X,controlPoint.Y,pt.X,pt.Y);
								
								controlPoint.Active=false;
								
							}
							
						}
						
						if(firstPoint!=lastPoint){
							// Close the path.
							
							if(controlPoint.Active){
								
								// Still got a spare control point:
								glyph.QuadraticCurveTo(controlPoint.X,controlPoint.Y,firstPoint.X,firstPoint.Y);
								
							}
							
							// Just a normal close:
							glyph.ClosePath();
							
						}
						
					}
					
				}
				
			}
			
			if(glyph.Font.WindingUnknown){
				// Find the winding now:
				glyph.Font.FindWinding(glyph);
			}
			
		}
		
		public static Glyph ParseGlyph(FontParser parser,FontFace font,float range){
			
			// How many contours has it got?
			int contourCount=parser.ReadInt16();
			
			// Skip bounds - we don't trust these too much, so we'll figure them out ourselves:
			parser.Position+=8;
			
			if(contourCount>0){
				// This glyph is not a composite.
				
				// Create the glyph:
				Glyph glyph=new Glyph(font);
				
				if(Fonts.Preload){
					
					LoadGlyph(glyph,contourCount,parser,range);
					
				}else{
					
					// Increase unloaded count:
					font.UnloadedGlyphs++;
					
					// Add position info:
					glyph.AddPathNode(new LoadMetaPoint(parser.Position,contourCount));
					
				}
				
				return glyph;
				
			}else if(contourCount==0){
				
				// Empty glyph e.g. space. Create the glyph:
				Glyph glyph=new Glyph(font);
				
				return glyph;
				
			}
			
			CompositeGlyph compGlyph=new CompositeGlyph(font);
			
			bool moreComponents=true;
			
			while(moreComponents){
			
				ushort cFlags=parser.ReadUInt16();
				ushort glyphIndex=parser.ReadUInt16();
				
				VectorTransform component=new VectorTransform(glyphIndex);
				
				if ((cFlags & 1) > 0) {
					// The arguments are words
					component.Dx = (float)parser.ReadInt16() / range;
					component.Dy = (float)parser.ReadInt16() / range;
				} else {
					// The arguments are bytes
					component.Dx = (float)parser.ReadByte()  / range;
					component.Dy = (float)parser.ReadByte()  / range;
				}
				
				if ((cFlags & 8) > 0) {
					// We have one scale
					component.XScale = component.YScale = parser.ReadF2Dot14();
				} else if ((cFlags & 64) > 0) {
					// We have an X / Y scale
					component.XScale = parser.ReadF2Dot14();
					component.YScale = parser.ReadF2Dot14();
				} else if ((cFlags & 128) > 0) {
					// We have a 2x2 transformation
					component.XScale = parser.ReadF2Dot14();
					component.Scale01 = parser.ReadF2Dot14();
					component.Scale10 = parser.ReadF2Dot14();
					component.YScale = parser.ReadF2Dot14();
				}
				
				// Push the component to the end:
				compGlyph.AddComponent(component);
				
				moreComponents = ((cFlags & 32)==32);
			}
			
			return compGlyph;
			
		}
		
		public static int LoadGlyphCoordinate(FontParser parser,byte flag,int previousValue,int shortVectorBitMask,int sameBitMask){
			
			int v;
			
			if((flag&shortVectorBitMask)>0){
				
				// The coordinate is 1 byte long.
				v = parser.ReadByte();
				
				// The "same" bit is re-used for short values to signify the sign of the value.
				if ((flag & sameBitMask) == 0) {
					v = -v;
				}
				
				v = previousValue + v;
			} else {
				
				//  The coordinate is 2 bytes long.
				// If the same bit is set, the coordinate is the same as the previous coordinate.
				
				if ((flag & sameBitMask) > 0) {
					v = previousValue;
				} else {
					// Parse the coordinate as a signed 16-bit delta value.
					v = previousValue + parser.ReadInt16();
				}
				
			}
			
			return v;
			
		}
		
	}

}