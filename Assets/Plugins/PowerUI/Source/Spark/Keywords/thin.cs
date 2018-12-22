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
	/// Represents an instance of the thin keyword.
	/// </summary>
	
	public class Thin:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 2f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "thin";
			}
		}
		
	}
	
}