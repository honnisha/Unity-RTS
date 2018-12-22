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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Compression;


namespace InfiniText{
	
	public static class WoffLoader{
		
		/// <summary>Woff2 uses this lookup rather than table names.</summary>
		public static readonly string[] TagLookup=new string[]{
		  "cmap",
			"head",
			"hhea",
			"hmtx",
			"maxp",
			"name",
			"OS/2",
			"post",
			"cvt ",
		  "fpgm",
			"glyf",
			"loca",
			"prep",
			"CFF ",
			"VORG",
			"EBDT",
			"EBLC",
			"gasp",
		  "hdmx",
			"kern",
			"LTSH",
			"PCLT",
			"VDMX",
			"vhea",
			"vmtx",
			"BASE",
			"GDEF",
		  "GPOS",
			"GSUB",
			"EBSC",
			"JSTF",
			"MATH",
			"CBDT",
			"CBLC",
			"COLR",
			"CPAL",
		  "SVG ",
			"sbix",
			"acnt",
			"avar",
			"bdat",
			"bloc",
			"bsln",
			"cvar",
			"fdsc",
		  "feat",
			"fmtx",
			"fvar",
			"gvar",
			"hsty",
			"just",
			"lcar",
			"mort",
			"morx",
		  "opbd",
			"prop",
			"trak",
			"Zapf",
			"Silf",
			"Glat",
			"Gloc",
			"Feat",
			"Sill"
		};
		
		/// <summary>Loads the WOFF header. Only useful thing from this for us is the number of tables.</summary>
		public static void LoadHeader(int version,FontParser parser,out ushort numTables){
			
			// - Woff header -

			parser.Position+=4; // Flavour (uint)
			parser.Position+=4; // Length (uint)
			numTables=parser.ReadUInt16();
			parser.Position+=2; // Reserved (ushort)
			parser.Position+=4; // Total size (uint)
			
			if(version==2){ // It's a shame this was put above the actual declared version
				parser.Position+=4; // Total compressed size (uint)
			}
			
			parser.Position+=2; // Version major (ushort)
			parser.Position+=2; // Version minor (ushort)
			parser.Position+=4; // meta Offset (uint)
			parser.Position+=4; // meta Length (uint)
			parser.Position+=4; // meta Orig Length (uint)
			parser.Position+=4; // priv offset (uint)
			parser.Position+=4; // priv Length (uint)
			
		}
		
		public static bool Load(int version,FontParser parser,FontFace font){
			
			// Load the V1/V2 header:
			ushort numTables;
			LoadHeader(version,parser,out numTables);
			
			if(version==1){
				
				MemoryStream ms=new MemoryStream(parser.Data);
				
				// Get the ZLIB compression helper:
				Compressor zLib=Compression.Get("zlib");
				
				// Read each table next.
				for(int i=0;i<numTables;i++){
					
					string tag=parser.ReadTag();
					uint offset=parser.ReadUInt32();
					uint compLength=parser.ReadUInt32();
					uint origLength=parser.ReadUInt32();
					parser.ReadUInt32(); // origChecksum
					
					// Cache position:
					int pos=parser.Position;
					
					if(compLength!=origLength){
						// Decompress the table now (zlib)
						
						// Seek to table:
						ms.Position=(int)offset;
						
						// Decompressed data:
						byte[] decompressedTable=new byte[(int)origLength];
						
						// Decompress now into our target bytes:
						zLib.Decompress(ms,decompressedTable);
						
					}else{
						// Ordinary table.
						parser.HandleTable(tag,(int)offset,font);
					}
					
					// Restore position:
					parser.Position=pos;
					
				}
				
			}else if(version==2){
				
				// Read each table entry next. The data here is compressed as one single block after the table meta.
				Woff2Table[] tableHeaders=new Woff2Table[numTables];
				int offset=0;
				
				for(int i=0;i<numTables;i++){
					
					// Read the table entry:
					byte flags=parser.ReadByte();
					
					string tag;
					
					int tagFlag=(flags & 63);
					
					if (tagFlag == 63) {
						
						tag=parser.ReadTag();
						
					}else{
						
						tag=TagLookup[tagFlag];
						
					}
					
					ulong origLength=parser.ReadBase128();
					ulong transformLength=0;
					
					int transformVersion=(flags >> 6); //0-3
					
					if (tag == "glyf" || tag == "loca") {
						
						// transform length:
						transformLength=parser.ReadBase128();
						
					}
					
					offset+=(int)origLength;
					
					tableHeaders[i]=new Woff2Table(tag,offset,(int)transformLength,transformVersion);
					
				}
				
			}
			
			// All ok:
			return true;
			
		}
		
	}
	
	public struct Woff2Table{
		
		public string Tag;
		public int Offset;
		public int TransformLength;
		public int TransformVersion;
		
		public Woff2Table(string tag,int offset,int tLength,int tVersion){
			Tag=tag;
			Offset=offset;
			TransformLength=tLength;
			TransformVersion=tVersion;
		}
		
	}
	
}