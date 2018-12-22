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

	public static class PostTables{
		
		public static void Load(FontParser parser,int offset,FontFace font,out int numberOfGlyphs){
			
			// Seek:
			parser.Position=offset;
			
			// version
			float version=parser.ReadVersion();
			
			// Italic angle. For some reason it's inverted in the spec - negative means a slope to the right.
			int frac;
			int dec=parser.ReadFixed(out frac);
			
			if(frac!=0){
				
				// Figure it out:
				float angle=(float)dec/(float)frac;
				
				// Apply it (un-inverted):
				font.SetItalicAngle(-angle);
				
			}
			
			// underlinePosition
			parser.ReadInt16();
			
			// underlineThickness
			parser.ReadInt16();
			
			// isFixedPitch
			parser.ReadUInt32();
			
			// minMemType42
			parser.ReadUInt32();
			
			// maxMemType42
			parser.ReadUInt32();
			
			// minMemType1
			parser.ReadUInt32();
			
			// maxMemType1
			parser.ReadUInt32();
			
			if(version==2f){
				
				numberOfGlyphs = parser.ReadUInt16();
				
				/*
				string[] glyphNames=new string[numberOfGlyphs];
				
				for (int i = 0; i < numberOfGlyphs; i++) {
					
					// Read the index:
					int index = parser.ReadUInt16();
					
					if(index >= StandardNames.Length){
						
						// Read the name:
						glyphNames[i]=parser.ReadString(parser.ReadByte());
						
					}else{
						
						// Grab the std name:
						glyphNames[i]=StandardNames[index];
						
					}
					
				}
				*/
				
			}else if(version==2.5f){
				numberOfGlyphs = parser.ReadUInt16();
				
				/*
				byte[] offsets = new byte[numberOfGlyphs];
				
				for (int i = 0; i < post.numberOfGlyphs; i++){
					
					offsets[i] = parser.ReadByte();
					
				}
				*/
			}else{
				numberOfGlyphs=-1;
			}
			
		}
		
	}

}