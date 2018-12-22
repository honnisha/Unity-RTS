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


namespace InfiniText{

	public static class KerningTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,Glyph[] glyphs){
			
			// Seek:
			parser.Position=offset;
			
			// Check table version - 0 is current:
			if(parser.ReadUInt16()!=0){
				return;
			}
			
			// Skip ntables:
			parser.Position+=2;
			
			// Sub-table version - 0 is current:
			if(parser.ReadUInt16()!=0){
				return;
			}
			
			// Skip subTableLength, subTableCoverage:
			parser.Position+=4;
			
			// How many pairs?
			int pairCount=parser.ReadUInt16();
			
			// Skip searchRange, entrySelector, rangeShift.
			parser.Position+=6;
			
			float scaleFactor=1f/font.UnitsPerEmF;
			
			for(int i=0;i<pairCount;i++){
				
				// Get the glyph indices:
				int leftIndex=parser.ReadUInt16();
				int rightIndex=parser.ReadUInt16();
				
				// Get the kerning value:
				short value=parser.ReadInt16();
				
				// Push:
				Glyph right=glyphs[rightIndex];
				Glyph left=glyphs[leftIndex];
				
				if(right==null || left==null){
					continue;
				}
				
				right.AddKerningPair(left,(float)value * scaleFactor);
				
			}
			
		}
		
	}

}