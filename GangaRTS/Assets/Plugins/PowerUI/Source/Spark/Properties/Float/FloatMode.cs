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
	
	public static class FloatMode{
		
		public const int None=0;
		public const int Left=HorizontalAlignMode.Left; // 1
		public const int Right=HorizontalAlignMode.Right; // 2
		public const int InlineStart=1<<2;
		public const int InlineEnd=1<<3;
		public const int Both=Left | Right;
		
	}
	
}