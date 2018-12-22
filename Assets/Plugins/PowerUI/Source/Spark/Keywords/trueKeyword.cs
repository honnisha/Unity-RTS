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
	/// Represents an instance of the true keyword.
	/// </summary>
	
	public class TrueKeyword:CssKeyword{
		
		public TrueKeyword(){
			Type=ValueType.Number;
		}
		
		public override string Name{
			get{
				return "true";
			}
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return true;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 1f;
		}
		
	}
	
}