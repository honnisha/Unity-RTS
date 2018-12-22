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

	public static class GposTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,Glyph[] glyphs){
			
			// Seek:
			parser.Position=offset;
			
			//float scaleFactor=1f/font.UnitsPerEmF;
			
			// Look for kerning data.
			/*Glyph right=glyphs[rightIndex];
			Glyph left=glyphs[leftIndex];
			
			if(right==null || left==null){
				continue;
			}
			
			right.AddKerningPair(left,(float)value * scaleFactor);
			*/
			
		}
		
	}

}