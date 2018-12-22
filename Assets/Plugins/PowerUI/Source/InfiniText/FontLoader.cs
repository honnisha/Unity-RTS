//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;


namespace InfiniText{

	public static class FontLoader{
		
		/// <summary>Creates a fontface from the given data. Only actually loads it on first glyph request.</summary>
		public static FontFace DeferredLoad(byte[] data){
			
			// Create the parser:
			FontParser parser=new FontParser(data);
			
			// Create the font:
			FontFace font=new FontFace();
			
			if(Load(parser,font,false)){
				return font;
			}else{
				return null;
			}
			
		}
		
		public static FontFace Load(byte[] data){
			
			FontParser parser=new FontParser(data);
			
			return Load(parser);
			
		}
		
		public static FontFace Load(FontParser parser){
			
			// Create the font:
			FontFace font=new FontFace();
			
			if(Load(parser,font,true)){
				return font;	
			}else{
				return null;
			}
			
		}
		
		public static bool Load(FontParser parser,FontFace font,bool full){
			
			font.RequiresLoad=!full;
			font.Parser=parser;
			
			// Read the magic number:
			uint magic=parser.ReadUInt32();
			
			if(magic == 0x00010000){
				
				// TTF outline format.
				
			}else if(magic == 0x774F4646){
				
				// WOFF 1
				return WoffLoader.Load(1,parser,font);
				
			}else if(magic == 0x774F4632){
				
				// WOFF 2
				return WoffLoader.Load(2,parser,font);
				
			}else{
				
				// OpenType (probably) 0x4F54544F
				
				// Reset to start:
				parser.Position=0;
				
				// OpenType. Read the tag (right at the start):
				string openTypeVersion=parser.ReadTag();
				
				if (openTypeVersion == "OTTO"){
					
					// CFF outline format.
					
				}else{
					
					// Unsupported format.
					return false;
					
				}
				
			}
			
			// Table count:
			int numTables=parser.ReadUInt16();
			
			// Move to p12:
			parser.Position=12;
			
			for (int i=0;i<numTables;i++){
				
				// Read the tables tag (e.g. GPOS):
				string tag=parser.ReadTag();
				
				// Move parser along:
				parser.Position+=4;
				
				// Read the offset:
				int offset=(int)parser.ReadUInt32();
				
				// Grab the position - this allows the tables to mess it up:
				int basePosition=parser.Position;
				
				// Handle the table now:
				if(!parser.HandleTable(tag,offset,font)){
					return false;
				}
				
				// Skip meta:
				parser.Position=basePosition+4;
			}
			
			if(full){
				return ReadTables(parser,font);
			}
			
		return true;
		
	}
	
	public static bool ReadTables(FontParser parser,FontFace font){
			
			int hmMetricCount=0;
			
			int tableOffset=parser.TableOffset("OS/2");
			
			if(tableOffset!=0){
				
				// OS/2 table:
				OS2Tables.Load(parser,tableOffset,font);
				
			}
			
			tableOffset=parser.TableOffset("hhea");
			
			if(tableOffset!=0){
				
				// Hhea table (always after OS/2):
				HheaTables.Load(parser,tableOffset,font,out hmMetricCount);
				
			}
			
			// Handle meta if it's not been loaded yet:
			if(font.Ascender==0f){
				// Requires info from both HHEA and OS/2:
				parser.ApplyWindowsMetrics(font);
			}
			
			
			// General metadata - the name table:
			tableOffset=parser.TableOffset("name");
			
			if(tableOffset!=0){
				
				NameTables.Load(parser,tableOffset,font);
				
			}
			
			// Glyph data next!
			
			Glyph[] glyphs=null;
			
			int locaOffset=parser.TableOffset("loca");
			tableOffset=parser.TableOffset("glyf");
			
			if(tableOffset!=0 && locaOffset!=0){
				
				bool shortVersion=(parser.IndexToLocFormat==0);
				
				// Load a loca table (temporary):
				uint[] locaTable=LocaTables.Load(parser,locaOffset,parser.GlyphCount,shortVersion);
				
				// Load the glyph set:
				glyphs=GlyfTables.Load(parser,tableOffset,locaTable,font);
				
				HmtxTables.Load(parser,parser.TableOffset("hmtx"),font,glyphs,hmMetricCount);
				
			}else{
				
				tableOffset=parser.TableOffset("CFF ");
				
				if(tableOffset!=0){
					
					int postOffset=parser.TableOffset("post");
					
					if(postOffset!=0){
						
						// Post table:
						int postGlyphCount;
						PostTables.Load(parser,postOffset,font,out postGlyphCount);
						
						if(parser.GlyphCount==0 && postGlyphCount!=-1){
							
							parser.GlyphCount=postGlyphCount;
							
						}
						
					}
					
					// Load the CFF (PostScript glyph) table:
					glyphs=CffTables.Load(parser,tableOffset,font);
					
				}else{
					
					// Unrecognised/ bad font.
					return false;
					
				}
				
			}
			
			if(glyphs!=null && glyphs.Length>0){
				
				font.NotDefined=glyphs[0];
				
			}
			
			// Kerning table next:
			tableOffset=parser.TableOffset("kern");
			
			if(tableOffset!=0){
				
				// Kerning table:
				KerningTables.Load(parser,tableOffset,font,glyphs);
				
			}else{
				
				// Try GPOS instead:
				tableOffset=parser.TableOffset("GPOS");
				
				if(tableOffset!=0){
				
					// GPOS table (also kerning data):
					GposTables.Load(parser,tableOffset,font,glyphs);
					
				}
				
			}
			
			// GSUB(stitution):
			tableOffset=parser.TableOffset("GSUB");
			
			if(tableOffset!=0){
				GsubTables.Load(parser,tableOffset,font);
				
			}
			
			// Finally, the character map:
			tableOffset=parser.TableOffset("cmap");
			
			if(tableOffset!=0){
				
				// Load the charmap:
				CharMapTables.Load(parser,tableOffset,font,glyphs);
				
			}
			
			if(Fonts.Preload){
				font.AllGlyphsLoaded();
			}
			
			return true;
		}
		
	}

}