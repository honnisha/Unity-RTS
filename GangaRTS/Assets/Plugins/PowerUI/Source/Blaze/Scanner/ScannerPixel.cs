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
	/// Scanner pixels are used during the live raster process.
	/// These objects store information about intersects temporarily and are globally cached.
	/// </summary>
	
	public class ScannerPixel{
		
		/// <summary>The x coordinate of this pixels intersect.</summary>
		public float X;
		/// <summary>The fill of this pixel.</summary>
		public byte Fill;
		/// <summary>The pixel after this one.</summary>
		public ScannerPixel Next;
		/// <summary>The pixel before this one.</summary>
		public ScannerPixel Previous;
		
		
		/// <summary>The x coordinate of the pixel containing this intersect.</summary>
		public int PixelIndex{
			get{
				return (int)X;
			}
		}
		
	}
	
}