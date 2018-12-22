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
	/// Represents a scanned vector. Essentially one of these is a midway point between
	/// the raw vector outline and a bitmap. It has the bonus of being extremely compact in memory
	/// yet extremely fast to produce the full bitmap from.
	/// </summary>
	
	public partial class SubRaster{
		
		/// <summary>The width of this semi-rastered vector.</summary>
		public int Width;
		/// <summary>The compact set of intersections of each scan line and the vector.</summary> 
		public SubScanPixel[][] Intersects;
		
		
		/// <summary>The height of this semi-rastered vector.</summary>
		public int Height{
			get{
				return Intersects.Length;
			}
		}
		
	}
	
}