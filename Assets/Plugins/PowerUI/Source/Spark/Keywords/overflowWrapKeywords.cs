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
	/// Represents an instance of the break-word keyword.
	/// </summary>
	
	public class BreakWord:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 1;
		}
		
		public override string Name{
			get{
				return "break-word";
			}
		}
		
	}
	
}