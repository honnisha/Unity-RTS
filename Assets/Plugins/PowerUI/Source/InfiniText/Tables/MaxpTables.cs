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

	public static class MaxpTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,out int glyphCount){
			
			// Seek:
			parser.Position=offset;
			
			// Table version:
			float version=parser.ReadVersion();
			
			// Glyph count:
			glyphCount=parser.ReadUInt16();
			
			if (version == 1f){
				
				// Max points:
				parser.ReadUInt16();
				
				// Max contours:
				parser.ReadUInt16();
				
				// Max composite points:
				parser.ReadUInt16();
				
				// Max composite contours:
				parser.ReadUInt16();
				
				// Max zones:
				parser.ReadUInt16();
				
				// Max twilight points:
				parser.ReadUInt16();
				
				// Max storage:
				parser.ReadUInt16();
				
				// Max function defs:
				parser.ReadUInt16();
				
				// Max instruction defs:
				parser.ReadUInt16();
				
				// Max stack elements:
				parser.ReadUInt16();
				
				// Max instruction size:
				parser.ReadUInt16();
				
				// Max component elements:
				parser.ReadUInt16();
				
				// Max component depth:
				parser.ReadUInt16();
				
			}
			
		}
		
	}

}