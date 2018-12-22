//--------------------------------------
//                Blaze
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Blaze{

	/// <summary>
	/// Any object that can be written to an atlas.
	/// </summary>

	public interface AtlasEntity{
		
		/// <summary>True if DrawToAtlas can be multithreaded for this object.</summary>
		bool MultiThreadDraw();
		
		/// <summary>Gets the dimensions of this entity on an atlas.</summary>
		void GetDimensionsOnAtlas(out int width,out int height);
		
		/// <summary>Draws this entity to the given atlas now.</summary>
		bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location);
		
		/// <summary>A globally unique ID that can be used to identify the image being held.</summary>
		int GetAtlasID();
		
	}
	
}