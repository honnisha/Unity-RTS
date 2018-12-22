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

#if PowerUI

using System;
using Blaze;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using PowerUI;


namespace Spa{
	
	/// <summary>
	/// Used when an SPA is acting as a bitmap font.
	/// Holds info for a particular character. In this case, ID is charcode.
	/// </summary>
	
	public partial class SPACharacter{
		
		/// <summary>Converts this spa character into an InfiniText glyph. 
		/// Note that it also gets added to the font face.</summary>
		public Glyph ToGlyph(FontFace face){
			
			// Get the size:
			int spriteSize=SpriteSize;
			
			// Create a glyph:
			Glyph glyph=new Glyph(face);
			
			// Apply location:
			glyph.Location=Location;
			
			// Add charcode:
			glyph.AddCharcode(ID);
			
			// Additional ones:
			if(AdditionalCharcodes!=null){
				
				// For each one..
				foreach(int cc in AdditionalCharcodes){
					
					// Add it:
					glyph.AddCharcode(cc);
					
				}
				
			}
			
			// Get the advance width and LSB:
			glyph.AdvanceWidth=(float)Advance / spriteSize;
			glyph.LeftSideBearing=(float)XOffset / spriteSize;
			
			// All done!
			return glyph;
			
		}
		
	}
	
}

#endif