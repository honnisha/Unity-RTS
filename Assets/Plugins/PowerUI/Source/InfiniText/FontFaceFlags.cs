//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace InfiniText{

	/// <summary>
	/// The flags which represent the styling of a font. Combined together.
	/// </summary>

	public static class FontFaceFlags{
		
		// Normal.
		public const int None=0;
		
		// Style (3 bits)
		public const int Italic=1;
		public const int Oblique=2;
		
		// Weight (4 bits)
		public const int Bold100=1<<3; // Thin
		public const int Bold200=2<<3; // Extra Light (Ultra Light)
		public const int Bold300=3<<3; // Light
		public const int Regular=4<<3;// Regular
		public const int Bold500=5<<3; // Medium
		public const int Bold600=6<<3; // Semi Bold (Demi Bold)
		public const int Bold700=7<<3; // Bold
		public const int Bold800=8<<3; // Extra Bold (Ultra Bold)
		public const int Bold900=9<<3; // Black (Heavy) 
		
		// Stretch (4 bits)
		public const int UltraCondensed=1<<7;
		public const int ExtraCondensed=2<<7;
		public const int Condensed=3<<7;
		public const int SemiCondensed=4<<7;
		public const int Medium=5<<7; // Normal. Alternatively don't define anything to get this one.
		public const int SemiExpanded=6<<7;
		public const int Expanded=7<<7;
		public const int ExtraExpanded=8<<7;
		public const int UltraExpanded=9<<7;
		
		// Masks for convenience.
		public const int StyleMask=7;
		public const int WeightMask=15<<3;
		public const int StretchMask=15<<7;
		
	}
	
}