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

	public static class HheaTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,out int hmMetricCount){
			
			// Seek there:
			parser.Position=offset;
			
			// Version:
			parser.ReadVersion();
			
			// Ascender:
			float ascender=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Descender:
			float descender=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Line gap:
			float lineGap=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			if(lineGap<0f){
				// Negative line gaps are treated as meaning 0.
				lineGap=0f;
			}
			
			parser.HheaAscender=ascender;
			parser.HheaDescender=descender;
			parser.HheaLineGap=lineGap;
			
			// Advance width max:
			font.MaxAdvanceWidth=(float)parser.ReadUInt16()/font.UnitsPerEmF;
			
			// Min left side bearing:
			font.MinLeftSideBearing=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Min right side bearing:
			font.MinRightSideBearing=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Max x extent:
			font.MaxXExtent=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Caret slope rise:
			float caretRise=(float)parser.ReadInt16();
			
			// Caret slope run:
			float caretRun=(float)parser.ReadInt16();
			
			font.CaretAngle=(float)Math.Atan2(caretRise,caretRun);
			
			// Caret offset:
			font.CaretOffset=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// Skip:
			parser.Position += 8;
			
			// Metric format:
			parser.ReadInt16();
			
			// Metric count:
			hmMetricCount = parser.ReadUInt16();
			
		}
		
	}

}