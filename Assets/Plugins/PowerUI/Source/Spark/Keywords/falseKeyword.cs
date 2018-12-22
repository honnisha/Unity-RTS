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
	/// Represents an instance of the false keyword.
	/// </summary>
	
	public class FalseKeyword:CssKeyword{
		
		public FalseKeyword(){
			Type=ValueType.Number;
		}
		
		public override string Name{
			get{
				return "false";
			}
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return false;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 0f;
		}
		
	}
	
}