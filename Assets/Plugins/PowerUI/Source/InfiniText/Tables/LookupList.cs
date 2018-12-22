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

	public class LookupList{
		
		internal FontParser Parser;
		internal int Offset;
		
		
		public LookupList(FontParser parser,int position){
			
			Parser=parser;
			Offset=position;
			
		}
		
		private Lookup[] Entries;
		
		private void Setup(){
			
			// Cache position:
			int pos=Parser.Position;
			
			// Seek now:
			Parser.Position=Offset;
			
			// Load each one:
			int lookCount=Parser.ReadUInt16();
			
			Entries=new Lookup[lookCount];
			
			for(int i=0;i<lookCount;i++){
				
				// Load it:
				int entryOffset=Parser.ReadUInt16();
				
				// Add to set:
				Entries[i]=new Lookup(Offset+entryOffset);
				
			}
			
			// Restore position:
			Parser.Position=pos;
			
		}
		
		/// <summary>Gets a lookup entry at the given index.</summary>
		public Lookup this[int index]{
			get{
				
				if(Entries==null){
					Setup();
				}
				
				// Get and setup if needed:
				Lookup entry=Entries[index];
				
				if(entry.SetupRequired){
					entry.Setup(Parser);
				}
				
				return entry;
				
			}
		}
		
	}
	
}