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

	public static class LocaTables{
		
		public static uint[] Load(FontParser parser,int offset,int numGlyphs,bool shortVersion){
			
			// Seek there now:
			parser.Position=offset;
			
			// Extra entry is used for computing the length of the last glyph, thus +1.
			int length=numGlyphs+1;
			
			// Create the output set:
			uint[] locations=new uint[length];
			
			// For each one..
			for(int i=0;i<length;i++){
				
				if(shortVersion){
					locations[i]=(uint)(parser.ReadUInt16()*2);
				}else{
					locations[i]=parser.ReadUInt32();
				}
				
			}
			
			return locations;
			
		}
		
	}

}