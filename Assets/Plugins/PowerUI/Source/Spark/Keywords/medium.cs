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
	/// Represents an instance of the medium keyword.
	/// </summary>
	
	public class Medium:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(property==Css.Properties.FontSize.GlobalProperty){
				// Font size (16):
				return 16f * context.ValueScale;
			}else if(property==Css.Properties.FontWeight.GlobalProperty){
				// Medium weight
				return 400f;
			}
			
			// Border
			return 4f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "medium";
			}
		}
		
	}
	
}