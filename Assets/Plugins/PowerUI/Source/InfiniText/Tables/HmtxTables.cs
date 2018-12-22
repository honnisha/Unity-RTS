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

	public static class HmtxTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,Glyph[] glyphs,int numMetrics){
			
			// Seek there:
			parser.Position=offset;
			
			// Get the glyph length:
			int numGlyphs=glyphs.Length;
		
			ushort advanceWidth=0;
			short leftSideBearing=0;
			
			// For each one..
			for (int i=0;i<numGlyphs;i++){
				
				// Monospaced fonts only have one entry:
				if (i<numMetrics){
					advanceWidth=parser.ReadUInt16();
					leftSideBearing=parser.ReadInt16();
				}
				
				Glyph glyph=glyphs[i];
				
				glyph.AdvanceWidth=(float)advanceWidth/font.UnitsPerEmF;
				glyph.LeftSideBearing=(float)leftSideBearing/font.UnitsPerEmF;
			}
			
			
		}
		
	}

}