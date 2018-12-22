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
using PowerUI;


namespace Spa{
	
	public class SPAMapEntry{
		
		/// <summary>The ID of this entry. Usually a Unicode charcode.</summary>
		public int ID;
		/// <summary>The location of this entry.</summary>
		public AtlasLocation Location;
		
		
		/// <summary>Creates a map entry, loading the coords from the given reader.</summary>
		public SPAMapEntry(SPASprite sprite,SPAReader reader){
			
			// Get the sprite as an atlas:
			TextureAtlas atlas=sprite.Atlas;
			
			// ID:
			ID=(int)reader.ReadCompressed();
			
			// Coords are..
			int x=(int)reader.ReadCompressedSigned() + reader.PreviousX;
			int y=(int)reader.ReadCompressedSigned() + reader.PreviousY;
			
			// Update previous:
			reader.PreviousX=x;
			reader.PreviousY=y;
			
			// Read the dimensions:
			int width=(int)reader.ReadCompressed();
			int height=(int)reader.ReadCompressed();
			
			// Create the atlas location now:
			Location=new AtlasLocation(atlas,x,y,width,height);
			
			// Prevent this location from getting recycled:
			Location.PreventDeallocation();
			
		}
		
	}
	
}