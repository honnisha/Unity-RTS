//--------------------------------------
//          Blaze Rasteriser
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
	/// A glyph vector is broken down into a series of points on its outline.
	/// These points are added to a grid - the distance cache - and used for fast
	/// distance testing during SDF rendering.
	/// </summary>
	
	internal struct DistanceCachePoint{
		
		/// <summary>The x coordinate of this point in pixel space.</summary>
		internal float X;
		/// <summary>The y coordinate of this point in pixel space.</summary>
		internal float Y;
		
		
		internal DistanceCachePoint(float x,float y){
			
			X=x;
			Y=y;
			
		}
		
	}
	
}