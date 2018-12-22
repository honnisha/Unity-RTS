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
	/// Represents an instance of the underline keyword.
	/// </summary>
	
	public class Underline:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextDecorationLineMode.Underline;
		}
		
		public override string Name{
			get{
				return "underline";
			}
		}
		
	}
	
	public class Overline:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextDecorationLineMode.Overline;
		}
		
		public override string Name{
			get{
				return "overline";
			}
		}
		
	}
	
	public class LineThrough:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextDecorationLineMode.LineThrough;
		}
		
		public override string Name{
			get{
				return "line-through";
			}
		}
		
	}
	
	public class CurrentColor:CssKeyword{
		
		public override string Name{
			get{
				return "currentcolor";
			}
		}
		
	}
	
}