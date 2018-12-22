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
	/// Represents an instance of the auto keyword.
	/// </summary>
	
	public class Auto:CssKeyword{
		
		public override bool IsAuto{
			get{
				return true;
			}
		}
		
		public override string Name{
			get{
				return "auto";
			}
		}
		
	}
	
}



