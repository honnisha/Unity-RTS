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
	/// Represents an instance of the cover keyword.
	/// </summary>
	
	public class CoverKeyword:CssKeyword{
		
		public CoverKeyword(){
			Type=ValueType.RelativeNumber;
		}
		
		public override string Name{
			get{
				return "cover";
			}
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return true;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			// 100%
			return 1f;
		}
		
	}
	
}