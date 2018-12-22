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
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace Spa{
	
	/// <summary>
	/// Used when an SPA is acting as a bitmap font.
	/// Holds info for a particular character. In this case, ID is charcode.
	/// </summary>
	
	public partial class SPACharacter : SPAMapEntry{
		
		/// <summary>The characters x offset.</summary>
		public int XOffset;
		/// <summary>The characters y offset.</summary>
		public int YOffset;
		/// <summary>The amount to advance by when laying out this character.</summary>
		public int Advance;
		/// <summary>A set of additional charcodes.</summary>
		public List<int> AdditionalCharcodes;
		/// <summary>A group of kerning pairs and their offsets.</summary>
		public Dictionary<SPACharacter,int> Kerning;
		
		
		/// <summary>Creates a character map entry, loading the coords from the given reader.</summary>
		public SPACharacter(SPASprite sprite,SPAReader reader):base(sprite,reader){
			
			// Note that the rest of the map entry has been loaded.
			// Now just need the xoffset etc.
			
			// So, offsets are:
			XOffset=(int)reader.ReadCompressedSigned();
			YOffset=(int)reader.ReadCompressedSigned();
			
			// Advance is:
			Advance=(int)reader.ReadCompressedSigned();
			
		}
		
		/// <summary>Adds an additional charcode to this character.</summary>
		public void AddCharcode(int cc){
			
			if(AdditionalCharcodes==null){
				AdditionalCharcodes=new List<int>();
			}
			
			AdditionalCharcodes.Add(cc);
			
		}
		
		/// <summary>Gets the size of the sprite.</summary>
		public int SpriteSize{
			get{
				return Location.Atlas.Dimension;
			}
		}
		
		/// <summary>Convinience mapping for the charcode.</summary>
		public int Charcode{
			get{
				return ID;
			}
		}
		
		/// <summary>Adds the given character as a kerning pair with this.</summary>
		public void AddKerningPair(SPACharacter beforeThis,int value){
			
			// When beforeThis is before this, this is kerned by the given value.
			
			if(Kerning==null){
				
				// Create the dictionary:
				Kerning=new Dictionary<SPACharacter,int>();
				
			}
			
			// Push:
			Kerning[beforeThis]=value;
			
		}
		
	}
	
}