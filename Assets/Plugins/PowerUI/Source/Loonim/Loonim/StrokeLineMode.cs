//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------


namespace Loonim{
	
	/// <summary>
	/// Various stroke-line values used by both stroke-linecap and stroke-linejoin.
	/// </summary>
	
	public static class StrokeLineMode{
		
		public const int Miter=1;
		public const int Round=1<<1;
		public const int Bevel=1<<2;
		public const int Butt=1<<3;
		public const int Square=1<<4;
		
		/// <summary>Any of the join values which should be applied.</summary>
		public const int JoinActive=Miter | Round;
		/// <summary>Any of the cap values which should be applied.</summary>
		public const int CapActive=Square | Round;
		
	}
	
}