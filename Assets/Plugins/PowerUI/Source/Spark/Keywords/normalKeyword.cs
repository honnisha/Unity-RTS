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
	/// Represents an instance of the normal keyword.
	/// </summary>
	
	public class Normal:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			return property.GetNormalValue(context);
			
		}
		
		public override string Name{
			get{
				return "normal";
			}
		}
		
	}
	
}