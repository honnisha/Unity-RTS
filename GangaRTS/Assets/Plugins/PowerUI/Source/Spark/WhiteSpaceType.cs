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
	/// This represents the white-space css property.
	/// </summary>
	
	public static class WhiteSpaceMode{
		
		/// <summary>Wraps onto new lines.</summary>
		public const int Normal=1;
		
		/// <summary>Doesn't wrap at all.</summary>
		public const int NoWrap=1<<2;
		
		/// <summary>Preserves spaces and potentially newlines too.</summary>
		public const int Pre=1<<3;
		public const int PreLine=1<<4;
		public const int PreWrap=1<<5;
		
		public const int NormalOrNoWrap=Normal | NoWrap;
		
		public const int Wrappable=Normal | PreWrap | PreLine;
		
	}
	
}