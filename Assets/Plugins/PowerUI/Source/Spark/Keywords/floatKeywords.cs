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
	/// Represents an instance of the inline-start keyword.
	/// </summary>
	
	// Note: These are an extension to the horizontal align keywords (as HA claims left/right).
	
	public class InlineStart:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FloatMode.InlineStart;
		}
		
		public override string Name{
			get{
				return "inline-start";
			}
		}
		
	}
	
	public class InlineEnd:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FloatMode.InlineEnd;
		}
		
		public override string Name{
			get{
				return "inline-end";
			}
		}
		
	}
	
	public class Both:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FloatMode.Both;
		}
		
		public override string Name{
			get{
				return "both";
			}
		}
		
	}
	
}