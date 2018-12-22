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


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the 'odd' keyword.
	/// </summary>
	
	public class Odd:CssKeyword{
		
		public override string Name{
			get{
				return "odd";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the 'even' keyword.
	/// </summary>
	
	public class Even:CssKeyword{
		
		public override string Name{
			get{
				return "even";
			}
		}
		
	}
	
}