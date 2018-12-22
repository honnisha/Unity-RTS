//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace Spa{
	
	/// <summary>
	/// Use one of these when creating a bitmap font.
	/// </summary>
	
	public class SPAFontMeta{
		
		/// <summary>The ascender. Gap between the baseline and the top of the line.</summary>
		public int Ascender;
		/// <summary>The descender. Gap between the bottom of the line and the baseline.</summary>
		public int Descender;
		/// <summary>Italic font?</summary>
		public bool Italic;
		/// <summary>Oblique font?</summary>
		public bool Oblique;
		/// <summary>The regular font?</summary>
		public bool Regular;
		/// <summary>Font weight. Can change Bold instead. (they both mean the same thing, but one is more flexible)
		/// Traditionally, 400 is "normal", 700 is bold.</summary>
		public int Weight=400;
		/// <summary>The family this font face belongs to.</summary>
		public string FamilyName;
		
		
		/// <summary>Loads this font meta.</summary>
		public SPAFontMeta(SPAReader reader){
			
			// Flags:
			int flags=reader.ReadByte();
			
			// Properties:
			Regular=((flags&1)==1);
			Italic=((flags&2)==2);
			Oblique=((flags&4)==4);
			
			// Name:
			FamilyName=reader.ReadString();
			
			// Font weight:
			Weight=(int)reader.ReadCompressed();
			
			// Ascender:
			Ascender=(int)reader.ReadCompressed();
			
			// Descender:
			Descender=(int)reader.ReadCompressed();
			
		}
		
	}

}