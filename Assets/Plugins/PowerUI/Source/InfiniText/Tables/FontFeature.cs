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
	/// A font feature in a particular font face, such as 'smcp' (small caps).
	/// </summary>
	
	public class FontFeature{
		
		public string Name;
		public FontParser Parser;
		public int Offset;
		public FontFace Face;
		/// <summary>The list of lookup entries.</summary>
		public LookupList List;
		/// <summary>This features lookup sets.</summary>
		public Lookup[] Lookups;
		
		
		/// <summary>Fully sets up this feature now.</summary>
		public void Setup(){
			
			int position=Parser.Position;
			
			// Skip unused params (ushort):
			Parser.Position=Offset+2;
			
			// Index count:
			int count=Parser.ReadUInt16();
			
			// For each one..
			Lookups=new Lookup[count];
			
			for(int i=0;i<count;i++){
				
				// Grab the index:
				int index=Parser.ReadUInt16();
				
				// Grab the lookup entry:
				Lookups[i]=List[index];
				
			}
			
			Parser.Position=position;
			
		}
		
		/// <summary>True if this feature has been fully loaded.</summary>
		public bool SetupRequired{
			get{
				return (Lookups==null);
			}
		}
		
	}
	
}