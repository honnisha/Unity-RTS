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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Css{
	
	/// <summary>
	/// Used by element.getClientRects as well as the core layout system.
	/// </summary>
	
	public static class LineBoxSegment{
		
		public const int Start=1;
		public const int Middle=2;
		public const int End=4;
		public const int All=1 | 2 | 4;
		
	}
	
}