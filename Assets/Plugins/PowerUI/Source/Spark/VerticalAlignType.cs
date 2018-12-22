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
	/// This represents the value for the vertical-align property.
	/// </summary>
	
	public static class VerticalAlignMode{
		
		public const int Middle=1;
		public const int Bottom=1<<1;
		public const int Baseline=1<<2;
		public const int Sub=1<<3;
		public const int Super=1<<4;
		public const int TextTop=1<<5;
		public const int TextBottom=1<<6;
		public const int Top=1<<7;
		
		// Shortcuts for table-cell vertical align modes
		public const int TableTop=1<<8;
		public const int TableMiddle=1<<9;
		public const int TableBottom=1<<10;
		
		public const int TopMiddleBottom=Top | Middle | Bottom;
		public const int TableMode=TableTop | TableMiddle | TableBottom;
		public const int BaselineRelative=Baseline | Sub | Super | Middle | TextTop | TextBottom;
		
	}
	
}