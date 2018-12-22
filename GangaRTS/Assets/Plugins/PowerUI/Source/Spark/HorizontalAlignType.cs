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

namespace Css{

	/// <summary>
	/// Values for horizontal alignment. Use text-align for this.
	/// </summary>
	
	public static class HorizontalAlignMode{
		
		public const int Auto=0;
		
		public const int Left=1;
		public const int Right=1<<1;
		public const int Center=1<<2;
		public const int Justify=1<<3;
		public const int SparkCenter=1<<4; //-moz-center
		
		// The following values are specific to text-align-last.
		public const int End=1<<5;
		public const int Start=1<<6;
		
		/// <summary>Either center mode.</summary>
		public const int EitherCenter=Center | SparkCenter;
		
	}
	
}