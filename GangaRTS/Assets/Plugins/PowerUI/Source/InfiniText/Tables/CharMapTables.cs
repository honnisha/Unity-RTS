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

	public static class CharMapTables{
		
		public static bool Load(FontParser parser,int start,FontFace font,Glyph[] glyphs){
			
			// Seek there:
			parser.Position=start;
			
			// Read the version (and check if it's zero):
			if(parser.ReadUInt16()!=0){
				return false;
			}
			
			// Strangely the cmap table can have lots of tables inside it. For now we're looking for the common type 3 table.
			
			// -> Got to grab all tables for a particular "platform".
			// -> Prefer platform 3 (Microsoft, most common and best documented), then 0, then anything else.
			
			// So, first let's figure out which platform we'll be using.
			
			// Favourite platform so far:
			int selectedPlatform=-1;
			
			// Number of tables:
			int tableCount=parser.ReadUInt16();
			
			for(int i = 0; i < tableCount; i += 1) {
				
				// Grab the platform ID:
				int platformId=parser.ReadUInt16();
				
				if(platformId==0){
					
					// Great, got platform 0.
					// Just halt there because we know this fonts got 0 so we'll use that.
					selectedPlatform=0;
					break;
					
				}else if(platformId==3){
					
					// Prefer 3 over others:
					selectedPlatform=3;
					
				}else if(selectedPlatform==-1){
					
					// Anything else (last resort):
					selectedPlatform=platformId;
					
				}
				
				// Skip encoding ID and offset:
				parser.Position+=6;
				
			}
			
			if(selectedPlatform==-1){
				// Empty table! Return.
				return false;
			}
			
			// Round 2. This time, select all subtables of the favourite platform.
			// Then collect the offset and load it up.
			
			// Reset parser, skipping count and version:
			parser.Position=start+4;
			
			// For each table..
			for(int i = 0; i < tableCount; i += 1) {
			
				// Grab the platform ID:
				int platformId=parser.ReadUInt16();
				
				#if INFINITEXT_DEBUG
				
				// And the encoding ID:
				int encodingId=parser.ReadUInt16();
				
				Fonts.OnLogMessage("Font "+font.FamilyName+" cmap subtable. Platform ID: "+platformId+", encoding ID: "+encodingId);
				
				#else
				
				// And the encoding ID:
				parser.ReadUInt16();
				
				#endif
				
				if(platformId==selectedPlatform){
					
					// Read offset:
					int offset=(int)parser.ReadUInt32();
					
					// Get position:
					int position=parser.Position;
					
					// Load the subtable now:
					font.CharacterCount+=LoadSubTable(parser,start+offset,font,glyphs);
					
					// Reset parser:
					parser.Position=position;
					
				}else{
					
					// Skip offset:
					parser.Position+=4;
					
				}
				
			}
			
			return true;
			
		}
		
		private static int LoadSubTable(FontParser parser,int start,FontFace font,Glyph[] glyphs){
			
			// Total characters in subtable:
			int characterCount=0;
			
			// Seek to the cmap now:
			parser.Position=start;
			
			// Check it's format 4:
			int format=parser.ReadUInt16();
			
			#if INFINITEXT_DEBUG
			
			Fonts.OnLogMessage("Cmap subtable format: "+format);
			
			#endif
			
			if(format>13){
				
				// We now have e.g. 14.0 - ulong here ("Length"):
				parser.Position+=4;
				
			}else if(format>6){
				// We now have e.g. 12.0 - another short here (reserved):
				parser.Position+=2;
				
				// Length and language are both 4 byte ints now:
				parser.Position+=8;
				
			}else{
			
				// Size of the sub-table (map length, u16):
				parser.Position+=2;
				
				// Structure of the sub-table (map language, u16):
				parser.Position+=2;
				
			}
			
			switch(format){
				
				case 0:
				
				// Byte encoding table:
				
				for(int i=0;i<256;i++){
					
					int rByte=parser.ReadByte();
					
					Glyph glyph=glyphs[rByte];
					
					if(glyph!=null){
						characterCount++;
					
						glyph.AddCharcode(i);
					}
					
				}
				
				break;
				case 2:
				
				// The offset to the headers:
				int subOffset=parser.Position + (256 * 2);
				
				// For each high byte:
				for(int i=0;i<256;i++){
					
					// Read the index to the header and zero out the bottom 3 bits:
					int headerPosition=subOffset + (parser.ReadUInt16() & (~7));
					
					// Read the header:
					int firstCode=parser.ReadUInt16(ref headerPosition);
					int entryCount=parser.ReadUInt16(ref headerPosition);
					short idDelta=parser.ReadInt16(ref headerPosition);
					
					// Grab the current position:
					int pos=headerPosition;
					
					// Read the idRangeOffset - the last part of the header:
					pos+=parser.ReadUInt16(ref headerPosition);
					
					int maxCode=firstCode+entryCount;
					
					// Get the high byte value:
					int highByte=(i<<8);
					
					// For each low byte:
					for (int j=firstCode;j<maxCode;j++){
						
						// Get the full charcode (which might not actually exist yet):
						int charCode=highByte+j;
						
						// Read the base of the glyphIndex:
						int p=parser.ReadUInt16(ref pos);
						
						if(p==0){
							continue;
						}
						
						p=(p+idDelta) & 0xFFFF;
						
						if(p==0){
							continue;
						}
						
						Glyph glyph=glyphs[p];
						
						if(glyph!=null){
							
							characterCount++;
							
							glyph.AddCharcode(charCode);
							
						}
						
					}
				}
				
				break;
				case 4:
				
				// Segment count. It's doubled.
				int segCount=(parser.ReadUInt16() >> 1);
				
				// Search range, entry selector and range shift (don't need any):
				parser.Position+=6;
				
				int baseIndex=parser.Position;
				
				int endCountIndex=baseIndex;
				
				baseIndex+=2;
				
				int startCountIndex = baseIndex + segCount * 2;
				int idDeltaIndex = baseIndex + segCount * 4;
				int idRangeOffsetIndex = baseIndex + segCount * 6;
				
				for(int i = 0; i < segCount - 1; i ++){
					
					int endCount = parser.ReadUInt16(ref endCountIndex);
					int startCount = parser.ReadUInt16(ref startCountIndex);
					int idDelta = parser.ReadInt16(ref idDeltaIndex);
					int idRangeOffset = parser.ReadUInt16(ref idRangeOffsetIndex);
					
					for(int c = startCount; c <= endCount;c++){
						
						int glyphIndex;
						
						if(idRangeOffset != 0){
							
							// The idRangeOffset is relative to the current position in the idRangeOffset array.
							// Take the current offset in the idRangeOffset array.
							int glyphIndexOffset = (idRangeOffsetIndex - 2);
							
							// Add the value of the idRangeOffset, which will move us into the glyphIndex array.
							glyphIndexOffset += idRangeOffset;
							
							// Then add the character index of the current segment, multiplied by 2 for USHORTs.
							glyphIndexOffset += (c - startCount) * 2;
							
							glyphIndex=parser.ReadUInt16(ref glyphIndexOffset);
							
							if(glyphIndex!=0){
								glyphIndex = (glyphIndex + idDelta) & 0xFFFF;
							}
							
						}else{
							glyphIndex = (c + idDelta) & 0xFFFF;
						}
						
						// Add a charcode to the glyph now:
						Glyph glyph=glyphs[glyphIndex];
						
						if(glyph!=null){
							characterCount++;
						
							glyph.AddCharcode(c);
						}
					}
					
				}
				
				break;
				
				case 6:
				
				int firstCCode=parser.ReadUInt16();
				int entryCCount=parser.ReadUInt16();
				
				for(int i=0;i<entryCCount;i++){
					
					Glyph glyphC=glyphs[parser.ReadUInt16()];
					
					if(glyphC!=null){
					
						characterCount++;
						
						glyphC.AddCharcode(firstCCode+i);
					
					}
					
				}
				
				break;
				
				case 10:
				
				// Trimmed array. Similar to format 6.
				
				int startCharCode=parser.ReadUInt16();
				int numChars=parser.ReadUInt16();
				
				for(int i=0;i<numChars;i++){
					
					Glyph glyphC=glyphs[parser.ReadUInt16()];
					
					if(glyphC!=null){
					
						characterCount++;
						
						glyphC.AddCharcode(startCharCode+i);
					
					}
					
				}
				
				break;
				
				case 12:
				
				// Segmented coverage.
				// Mapping of 1 charcode to 1 glyph. "Segmented" because it can come in blocks called groups.
				
				int groups=(int)parser.ReadUInt32();
				
				// For each group of glyphs..
				for(int i=0;i<groups;i++){
					
					// Start/end charcode:
					int startCode=(int)parser.ReadUInt32();
					int endCode=(int)parser.ReadUInt32();
					
					// Start glyph ID:
					int startGlyph=(int)parser.ReadUInt32();
					
					int count=(endCode - startCode);
					
					// For each glyph/charcode pair..
					for(int j=0;j<=count;j++){
						
						// Get the glyph:
						int glyphIndex=(startGlyph+j);
						
						Glyph glyph=glyphs[glyphIndex];
						
						if(glyph!=null){
							
							characterCount++;
							
							// Charcode is..
							glyph.AddCharcode(startCode+j);
							
						}
						
					}
				}
				
				break;
				
				case 13:
				
				// Many to one. Same format as #12 but the meaning is a bit different.
				
				// How many groups?
				int glyphCount=(int)parser.ReadUInt32();
				
				for(int i=0;i<glyphCount;i++){
					int startCode=(int)parser.ReadUInt32();
					int endCode=(int)parser.ReadUInt32();
					int glyphID=(int)parser.ReadUInt32();
					
					// Get the glyph:
					Glyph glyph=glyphs[glyphID];
					
					if(glyph!=null){
						
						int count=(endCode - startCode);
						
						// For each charcode..
						for(int j=0;j<=count;j++){
							
							characterCount++;
							
							// Hook up glyph to this charcode:
							glyph.AddCharcode(startCode+j);
							
						}
						
					}
					
				}
				
				break;
				
				case 14:
				
					Fonts.OnLogMessage("InfiniText partially supports part of the font '"+font.Family.Name+"' - this is harmless. Search for this message for more.");
					
					// This font contains a format 14 CMAP Table.
					// Format 14 is "Unicode variation selectors" - essentially different versions of the same character.
					// E.g. a text Emoji character and a graphical one.
					// In a text system like InfiniText, that just means we must map a bunch of different charcodes
					// to the same glyph.
					
					// .. I Think! As usual, the OpenType spec doesn't make too much sense.
					// However, it appears to be entirely optional.
					
					// So, approx implementation is below, however getting the utf32 code point from the variation + base character
					// is completely undocumented - my best guess unfortunately threw errors.
					
					// See the commented out block below!
					
				break;
				
				/*
				
				case 14:
				
				// How many var selector records?
				int records=(int)parser.ReadUInt32();
				
				for(int i=0;i<records;i++){
					
					// variation selector:
					int varSelector=(int)parser.ReadUInt24();
					
					// Offsets:
					int defaultUVSOffset=(int)parser.ReadUInt32();
					int nonDefaultUVSOffset=(int)parser.ReadUInt32();
					
					// Grab parser position:
					int position=parser.Position;
					
					// Got a ref to a default style table?
					if(defaultUVSOffset!=0){
						
						// Yep! The UVS is simply a list of "base" characters, each with ranges of available charcodes.
						// [BaseCharCode][The extended part. Each of these comes from the range.]
						// The actual glyph is the one that we get by directly looking up each of the base characters.
						
						// Seek to the table:
						parser.Position=start+defaultUVSOffset;
						
						// Read the unicode value ranges count:
						int numUniRangesCount=(int)parser.ReadUInt32();
						
						// For each one..
						for(int m=0;m<numUniRangesCount;m++){
							
							// Read the base charcode:
							int baseCharcode=(int)parser.ReadUInt24();
							
							// Read the size of the range:
							byte rangeSize=parser.ReadByte();
							
							for(int c=0;c<=rangeSize;c++){
								
								// Fetch the base glyph:
								Glyph glyph=font.GetGlyphDirect(baseCharcode);
								
								if(glyph!=null){
									
									// Combine baseCharcode with varSelector next to form the variation (of "glyph").
									
									// Get the full charcode (this is incorrect!):
									// int charcode=char.ConvertToUtf32((char)baseCharcode,(char)varSelector);
									
									// Add:
									//glyph.AddCharcode(charcode);
									
								}
								
								// Move baseCharcode along:
								baseCharcode++;
								
							}
							
						}
						
						// Restore parser:
						parser.Position=position;
						
					}
					
					// Got a ref to a non-default style table?
					if(nonDefaultUVSOffset!=0){
						
						// Yep! The UVS is simply a list of "base" characters, each with ranges of available charcodes.
						// [BaseCharCode][The extended part. Each of these comes from the range.]
						// This time though, the glyph to use is directly specified 
						// (that's what gives it the "non-default" property).
						
						// Seek to the table:
						parser.Position=start+nonDefaultUVSOffset;
						
						// Read the number of mappings:
						int numMappings=(int)parser.ReadUInt32();
						
						// For each one..
						for(int m=0;m<numMappings;m++){
							
							// Read the base charcode:
							int baseCharcode=(int)parser.ReadUInt24();
							
							// Read glyph ID:
							int glyphID=(int)parser.ReadUInt16();
							
							// Get the glyph:
							Glyph glyph=glyphs[glyphID];
							
							if(glyph!=null){
							
								// Combine baseCharcode with varSelector next to form the variation (of "glyph").
								
								// Get the full charcode (this is incorrect!):
								// int charcode=char.ConvertToUtf32((char)baseCharcode,(char)varSelector);
								
								// Add:
								//glyph.AddCharcode(charcode);
								
							}
							
						}
						
						// Restore parser:
						parser.Position=position;
						
					}
					
				}
				
				break;
				
				*/
				
				default:
					Fonts.OnLogMessage("InfiniText does not currently support part of this font. If you need it, please contact us with this: Format: "+format);
				break;
			}
			
			return characterCount;
			
		}
		
	}

}