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

	public class Lookup{
		
		internal int Offset;
		public ushort MarkFilteringSet;
		public ushort Flags;
		/// <summary>The mapping. It marks the first glyph of a sequence to its set of one or more substitution matches.
		/// (A set of substitutions is itself a 'GlyphSubstitution' object - avoids the need to have a list here)</summary>
		public Dictionary<int,Substitution> Substitutions;
		
		
		public Lookup(int position){
			
			Offset=position;
			
		}
		
		public bool SetupRequired{
			get{
				return Substitutions==null;
			}
		}
		
		internal void Setup(FontParser parser){
			
			/*
			// Format:
			int format=parser.ReadUInt16();
			
			switch(format){
				
				case 1:
					
					
					
				break;
				
				case 2:
					
					
					
				break;
				
			}
			*/
			
			// Seek now:
			int start=parser.Position;
			parser.Position=Offset;
			
			// Lookup Type:
			ushort lookupType=parser.ReadUInt16();
			
			// Lookup Flags:
			Flags=parser.ReadUInt16();
			
			// Lookup sub-tables:
			int subtableCount=parser.ReadUInt16();
			
			Substitutions=new Dictionary<int,Substitution>();
			
			// For each one..
			for(int i=0;i<subtableCount;i++){
				
				// Get the offset:
				int offset=parser.ReadUInt16();
				
				// Get the current position:
				int position=parser.Position;
				
				// Hop there:
				parser.Position=Offset+offset;
				
				// Create the sub-table:
				LookupSubTable subTable=null;
				
				// Load it (depends on our type):
				switch(lookupType){
					
					case 1:
						subTable=new SingleSubTable();
					break;
					case 2:
						subTable=new MultiSubTable();
					break;
					case 3:
						subTable=new AlternateSubTable();
					break;
					case 4:
						subTable=new LigatureSubTable();
					break;
					case 5:
						subTable=new ContextualSubTable();
					break;
					case 6:
						subTable=new ChainingContextualSubTable();
					break;
					case 7:
						subTable=new ExtensionSubTable();
					break;
					case 8:
						subTable=new ReverseChainingContextualSubTable();
					break;
					
				}
				
				if(subTable==null){
					// Broken font!
					throw new Exception("Broken GPOS table in a font - font loader halted.");
				}
				
				// Load the table (we're already seeked to it):
				ushort format=parser.ReadUInt16();
				subTable.Load(parser,format,parser.Position-2);//,Substitutions);
				
				// Go back:
				parser.Position=position;
				
			}
			
			// Mark filtering set:
			MarkFilteringSet=((Flags & 0x10)==0x10) ? parser.ReadUInt16() : (ushort)0;
			
			// Return:
			parser.Position=start;
			
		}
		
	}
	
}