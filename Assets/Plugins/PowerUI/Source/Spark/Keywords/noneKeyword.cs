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
	/// Represents an instance of the none keyword.
	/// </summary>
	
	public class None:CssKeyword{
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return false;
		}
		
		public override string Name{
			get{
				return "none";
			}
		}
		
	}
	
}