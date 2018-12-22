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
	/// Represents an instance of the visible keyword.
	/// </summary>
	
	public class Ltr:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DirectionMode.LTR;
		}
		
		public override string Name{
			get{
				return "ltr";
			}
		}
		
	}
	
	public class Rtl:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DirectionMode.RTL;
		}
		
		public override string Name{
			get{
				return "rtl";
			}
		}
		
	}
	
}