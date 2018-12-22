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

	public static class HeaderTables{
		
		public static bool Load(FontParser parser,int offset,FontFace font,out int locFormatIndex){
			
			// Seek there now:
			parser.Position=offset;
			
			// Version:
			parser.ReadVersion();
			
			// Revision:
			parser.ReadRevision();
			
			// Checksum adjustment:
			parser.ReadUInt32();
			
			// Magic number:
			if(parser.ReadUInt32()!=0x5F0F3CF5){
				
				locFormatIndex=0;
				
				return false;
				
			}
			
			// Flags:
			parser.ReadUInt16();
			
			// Units per em:
			font.UnitsPerEm = parser.ReadUInt16();
			font.UnitsPerEmF = (float)font.UnitsPerEm;
			
			// Skip created and modified:
			parser.ReadTime();
			parser.ReadTime();
			
			// X min:
			parser.ReadInt16();
			
			// Y min:
			parser.ReadInt16();
			
			// X max:
			parser.ReadInt16();
			
			// Y max:
			parser.ReadInt16();
			
			// Mac style:
			parser.ReadUInt16();
			
			// Lowest Rec PPEM:
			parser.ReadUInt16();
			
			// Font direction hint:
			parser.ReadInt16();
			
			// Index for the loc format:
			locFormatIndex = parser.ReadInt16(); // 50
			
			// Glyph data format:
			parser.ReadInt16();
			
			return true;
			
		}
		
	}

}