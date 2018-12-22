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
	
	/// <summary>
	/// Loads glyph substitution tables (ligatures).
	/// Ligatures are language sensitive. Each language group defines a bunch of "features" a font supports.
	/// Features can be, for example, turning 1/2 into a fraction; the "frac" feature.
	/// </summary>
	
	public static class GsubTables{
		
		public static void Load(FontParser parser,int offset,FontFace font){
			
			// Seek there:
			parser.Position=offset;
			
			// Version:
			parser.ReadVersion();
			
			// Script list (directed by UI Language setting):
			int scriptList=parser.ReadUInt16();
			
			// Feature list:
			int featList=parser.ReadUInt16();
			
			// Lookup list:
			int lookList=parser.ReadUInt16();
			
			// Goto script list:
			int objectOffset=scriptList+offset;
			
			parser.Position=objectOffset;
			
			/*
			// How many language scripts?
			int scriptCount=parser.ReadUInt16();
			
			for(int i=0;i<scriptCount;i++){
				
				// Read the script code:
				string scrName=parser.ReadString(4);
				
				// And it's offset:
				int scriptOffset=parser.ReadUInt16()+objectOffset;
				
				int retPosition=parser.Position;
				
				// Seek and load it right away:
				parser.Position=scriptOffset;
				
				// What's the default lang?
				int defaultLangOffset=parser.ReadUInt16();
				
				// How many languages?
				int langCount=parser.ReadUInt16();
				
				for(int l=0;l<langCount;l++){
					
					// Read the lang code:
					string langName=parser.ReadString(4);
					
					// And it's offset - points to list of features:
					int langOffset=parser.ReadUInt16()+objectOffset;
					
				}
				
				parser.Position=retPosition;
				
			}
			*/
			
			// Create lookup set:
			LookupList lookups=new LookupList(parser,lookList+offset);
			
			// Goto feature list:
			objectOffset=featList+offset;
			
			parser.Position=objectOffset;
			
			// How many features?
			int featureCount=parser.ReadUInt16();
			
			for(int i=0;i<featureCount;i++){
				
				// Read the feature code:
				string featureName=parser.ReadString(4);
				
				// Table offset:
				int featTable=parser.ReadUInt16();
				
				// Create the feature ref:
				FontFeature feature=new FontFeature();
				feature.Name=featureName;
				feature.Offset=objectOffset + featTable;
				feature.Parser=parser;
				feature.Face=font;
				feature.List=lookups;
				
				// Add to font:
				font.Features[featureName]=feature;
				
			}
			
		}
		
	}

}