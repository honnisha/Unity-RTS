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
	/// Represents an instance of the thick keyword.
	/// </summary>
	
	public class Thick:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 6f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "thick";
			}
		}
		
	}
	
}