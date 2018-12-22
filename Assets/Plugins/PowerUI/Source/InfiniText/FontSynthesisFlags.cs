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
	/// The flags which represent the synthesis of a font face.
	/// </summary>

	public static class FontSynthesisFlags{
		
		/// <summary>Don't synthesise anything.</summary>
		public const int None=0;
		
		/// <summary>Synthesise the style.</summary>
		public const int Style=1;
		
		/// <summary>Synthesise the weight.</summary>
		public const int Weight=1<<1;
		
		/// <summary>Synthesise the stretch.</summary>
		public const int Stretch=2<<1;
		
		/// <summary>Synthesise all properties.</summary>
		public const int All=Style | Weight | Stretch;
		
	}
	
}