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
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{

	public static class CffTables{
		
		private static Glyph[] LoadIndex(FontParser parser,CffGlyphParser cffParser){
			
			// Read the index which contains a bunch of char strings.
			// Each charstring is a postscript glyph definition.
			
			// How many are in here?
			int count=parser.ReadUInt16();
			
			if(count==0){
				
				return null;
				
			}
			
			// Create the offset set:
			int[] offsets=new int[count+1];
			
			// Read the offset size:
			int offsetSize=parser.ReadByte();
			
			// Read each offset:
			for(int i=0;i<=count;i++){
				
				// Read the current offset:
				offsets[i]=parser.ReadOffset(offsetSize);
				
			}
		
			// Grab the object offset, minus one as their not zero based:
			int objectOffset=parser.Position-1;
			
			// Create the glyph set:
			Glyph[] glyphs=new Glyph[offsets.Length-1];
			
			// For each one..
			for(int i=0;i<glyphs.Length;i++){
				
				// Get the (relative) indices:
				int startIndex=offsets[i];
				int length=offsets[i+1]-startIndex;
				
				// Load the glyph now, which starts at startIndex+objectOffset:
				Glyph glyph=cffParser.LoadGlyph(startIndex+objectOffset,length);
				
				// Add to the set:
				glyphs[i]=glyph;
				
			}
			
			// Seek over the table:
			parser.Position=objectOffset+offsets[count];
			
			return glyphs;
			
		}
		
		private static CffSubPosition[] LoadSubIndex(FontParser parser){
			
			// How many are in here?
			int count=parser.ReadUInt16();
			
			if(count==0){
				
				return null;
				
			}
			
			// Create the offset set:
			int[] offsets=new int[count+1];
			
			// Read the offset size:
			int offsetSize=parser.ReadByte();
			
			// Read each offset:
			for(int i=0;i<=count;i++){
				
				// Read the current offset:
				offsets[i]=parser.ReadOffset(offsetSize);
				
			}
			
			// Minus one as their not zero based:
			int objectOffset=parser.Position-1;
			
			// Seek over the table:
			parser.Position=objectOffset+offsets[count];
		
			// Create the result set:
			CffSubPosition[] results=new CffSubPosition[offsets.Length-1];
			
			// For each one..
			for(int i=0;i<results.Length;i++){
				
				// Get the (relative) indices:
				int startIndex=offsets[i];
				int length=offsets[i+1]-startIndex;
				
				// Load the glyph now, which starts at startIndex+objectOffset:
				results[i]=new CffSubPosition(startIndex+objectOffset,length);
				
			}
		
			return results;
			
		}
		
		private static void SkipIndex(FontParser parser){
			
			// How many are in here?
			int count=parser.ReadUInt16();
			
			if(count>0){
				
				// Read the offset size:
				int offsetSize=parser.ReadByte();
				
				// Skip count offsets:
				parser.Position+=offsetSize*count;
				
				// Read the last offset:
				int lastOffset=parser.ReadOffset(offsetSize);
				
				// Seek there, minus one as their not zero based:
				parser.Position+=lastOffset-1;
				
			}
			
		}
		
		public static int GetBias(CffSubPosition[] set){
			
			if(set==null || set.Length<1240){
				return 107;
			}else if(set.Length<33900){
				return 1131;
			}
			
			return 32768;
			
		}
		
		private static Dictionary<int,List<int>> LoadDict(FontParser parser){
			
			// How many are in here?
			int count=parser.ReadUInt16();
			
			if(count==0){
				
				return null;
				
			}
			
			// Read the offset size:
			int offsetSize=parser.ReadByte();
			
			// Grab the position:
			int position=parser.Position;
			
			// Find where the data starts:
			int objectOffset=position+((count+1)*offsetSize)-1;
			
			// Read two only:
			int firstOffset=parser.ReadOffset(offsetSize);
			int secondOffset=parser.ReadOffset(offsetSize);
			
			// Seek to the location:
			parser.Position=firstOffset+objectOffset;
			
			// Parse the dictionary now:
			Dictionary<int,List<int>> set=ParseCFFDict(parser,secondOffset-firstOffset);
			
			// Return:
			parser.Position=position;
			
			// Skip the rest:
			parser.Position+=offsetSize*count;
			
			// Read the last offset:
			secondOffset=parser.ReadOffset(offsetSize);
			
			// Seek there, minus one as their not zero based:
			parser.Position+=secondOffset-1;
			
			return set;
			
		}
		
		public static int ParseOperand(FontParser parser,int b0){
			
			if(b0==28){
				return parser.ReadInt16();
			}
			
			if(b0==29){
				return parser.ReadInt32();
			}
			
			if(b0==30){
				// A floating point value which we really don't need - skipping!
				
				while(true){
					byte b=parser.ReadByte();
					int n1 = b >> 4;
					int n2 = b & 15;

					if (n1 ==15 || n2==15) {
						break;
					}
					
				}
				
				return 0;
			}
			
			if (b0 >= 32 && b0 <= 246) {
				return b0 - 139;
			}
			
			if (b0 >= 247 && b0 <= 250) {
				return (b0 - 247) * 256 + parser.ReadByte() + 108;
			}
			
			if (b0 >= 251 && b0 <= 254) {
				return -(b0 - 251) * 256 - parser.ReadByte() - 108;
			}
			
			return 0;
		}
		
		public static Dictionary<int,List<int>> ParseCFFDict(FontParser parser,int length){
			
			// Create the results set:
			Dictionary<int,List<int>> results=new Dictionary<int,List<int>>();
			
			// Our first values set:
			List<int> values=new List<int>();
			
			// Note that rather awkwardly the key comes *after* the set of values.
			
			int max=parser.Position+length;
			
			// While there's more data..
			while(parser.Position<max){
				
				// Read the state byte:
				int state=parser.ReadByte();
				
				if(state<22){
					
					if(state==12){
						// 2 byte key code.
						state=1200+parser.ReadByte();
					}
					
					// Push values:
					results[state]=values;
					
					// Clear:
					values=new List<int>();
				
				}else{
					
					// Read the operand:
					int operand=ParseOperand(parser,state);
					
					// Push:
					values.Add(operand);
					
				}
				
			}
			
			return results;
			
		}
		
		public static int ReadDict(Dictionary<int,List<int>> dict,int index){
			
			List<int> values;
			if(dict.TryGetValue(index,out values)){
				return values[0];
			}
			
			return 0;
			
		}
		
		public static Glyph[] Load(FontParser parser,int offset,FontFace font){
			
			// Create the parser:
			CffGlyphParser cffParser=new CffGlyphParser(parser,font);
			cffParser.FullLoad=Fonts.Preload;
			font.CffParser=cffParser;
			
			// Seek, skipping the 4 byte header:
			parser.Position=offset+4;
			
			// Skip the name index:
			SkipIndex(parser);
			
			// Top dict index next (one entry only):
			Dictionary<int,List<int>> topDict=LoadDict(parser);
			
			// String index:
			SkipIndex(parser);
			
			// GSubr index:
			cffParser.GSubrs=LoadSubIndex(parser);
			
			// Figure out the bias:
			cffParser.GsubrsBias=GetBias(cffParser.GSubrs);
			
			// Read the private dict next - grab the info about where it is:
			List<int> privateDictInfo=topDict[18];
			
			// Get it's offset:
			int privateDictOffset=offset+privateDictInfo[1];
			
			// Seek there:
			parser.Position=privateDictOffset;
			
			// Load:
			Dictionary<int,List<int>> privateDict=ParseCFFDict(parser,privateDictInfo[0]);
			
			// Grab the default values:
			cffParser.DefaultWidthX=(float)ReadDict(privateDict,20);
			cffParser.NominalWidthX=(float)ReadDict(privateDict,21);
			
			// Grab the subrs offset. May be zero (or non-existant):
			int privateSubrs=ReadDict(privateDict,19);
			
			if(privateSubrs!=0){
				
				// We have a "subroutine" set. Get it's full offset and hop there:
				parser.Position=privateDictOffset+privateSubrs;
				
				// Load the set:
				cffParser.Subrs=LoadSubIndex(parser);
				
				// Figure out the bias/offset:
				cffParser.SubrsBias=GetBias(cffParser.Subrs);
				
			}
			
			// Seek to the char string table:
			parser.Position=offset+ReadDict(topDict,17);
			
			// Time to actually load the actual glyphs, wohoo! O.O
			return LoadIndex(parser,cffParser);
			
		}
		
	}

}