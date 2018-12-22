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
	/// Values used by the CompleteLine method.
	/// </summary>
	
	public static class LineBreakMode{
		
		/// <summary>True if the line should be a clean break (usually provide this).</summary>
		public const int Break=1;
		/// <summary>This is the top of stack call (usually provide this).</summary>
		public const int TopOfStack=1<<1;
		/// <summary>This is the last line.</summary>
		public const int Last=1<<2;
		
		/// <summary>The non-breaking mode.</summary>
		public const int NoBreak=TopOfStack;
		/// <summary>Major "normal" mode usually used.</summary>
		public const int Normal=Break | TopOfStack;
		
	}
	
}