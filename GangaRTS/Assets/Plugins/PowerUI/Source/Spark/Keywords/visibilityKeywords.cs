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
	
	public class Visible:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VisibilityMode.Visible;
		}
		
		public override string Name{
			get{
				return "visible";
			}
		}
		
	}
	
	public class Hidden:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VisibilityMode.Hidden;
		}
		
		public override string Name{
			get{
				return "hidden";
			}
		}
		
	}
	
	public class Collapse:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VisibilityMode.Collapse;
		}
		
		public override string Name{
			get{
				return "collapse";
			}
		}
		
	}
	
	public class Scroll:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VisibilityMode.Scroll;
		}
		
		public override string Name{
			get{
				return "scroll";
			}
		}
		
	}
	
}