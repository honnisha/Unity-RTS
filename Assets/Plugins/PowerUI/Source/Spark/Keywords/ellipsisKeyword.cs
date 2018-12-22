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
	/// Represents an instance of the ellipsis keyword. Used by text-overflow.
	/// </summary>
	
	public class Ellipsis:CssKeyword{
		
		public override string GetText(RenderableData context,CssProperty property){
			return "...";
		}
		
		public override string Name{
			get{
				return "ellipsis";
			}
		}
		
	}
	
}



