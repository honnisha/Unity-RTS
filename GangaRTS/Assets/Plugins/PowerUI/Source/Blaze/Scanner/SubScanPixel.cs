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
	/// Represents a pixel in a scan line.
	/// We scan the vector for each row of pixels.
	/// Whenever our scanner intersects with the vector, we generate one of these pixels.
	/// So the overall count of these for a semi-rastered vector is tiny.
	/// </summary>
	
	public struct SubScanPixel{
		
		/// <summary>The x coordinate of this pixel.</summary>
		public ushort X;
		/// <summary>The fill of this particular scan pixel.</summary>
		public byte Fill;
		
		
		public SubScanPixel(ushort x,byte fill){
			X=x;
			Fill=fill;
		}
		
	}
	
}