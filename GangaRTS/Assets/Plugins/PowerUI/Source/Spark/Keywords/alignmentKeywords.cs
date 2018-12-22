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
	/// Represents an instance of the right keyword.
	/// </summary>
	
	public class Right:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return HorizontalAlignMode.Right;
		}
		
		public override string Name{
			get{
				return "right";
			}
		}
		
	}
	
	public class Left:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return HorizontalAlignMode.Left;
		}
		
		public override string Name{
			get{
				return "left";
			}
		}
		
	}
	
	public class Justify:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return HorizontalAlignMode.Justify;
		}
		
		public override string Name{
			get{
				return "justify";
			}
		}
		
	}
	
	public class Center:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return HorizontalAlignMode.Center;
		}
		
		public override string Name{
			get{
				return "center";
			}
		}
		
	}
	
	/// <summary>Same as -moz-center. Used by the center element.</summary>
	public class SparkCenter:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return HorizontalAlignMode.SparkCenter;
		}
		
		public override string Name{
			get{
				return "-moz-center";
			}
		}
		
	}
	
	public class Start:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(property is Css.Properties.TextAlignLast){
				
				return HorizontalAlignMode.Start;
			
			}
			
			// As used by transitions
			return 0f;
			
		}
		
		public override string Name{
			get{
				return "start";
			}
		}
		
	}
	
	public class End:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(property is Css.Properties.TextAlignLast){
				
				return HorizontalAlignMode.End;
				
			}
			
			// As used by transitions
			return 1f;
			
		}
		
		public override string Name{
			get{
				return "end";
			}
		}
		
	}
	
	public class Top:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Top;
		}
		
		public override string Name{
			get{
				return "top";
			}
		}
		
	}
	
	public class Bottom:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Bottom;
		}
		
		public override string Name{
			get{
				return "bottom";
			}
		}
		
	}
	
	public class TableBottom:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.TableBottom;
		}
		
		public override string Name{
			get{
				return "table-bottom";
			}
		}
		
	}
	
	public class TableTop:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.TableTop;
		}
		
		public override string Name{
			get{
				return "table-top";
			}
		}
		
	}
	
	public class TableMiddle:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.TableMiddle;
		}
		
		public override string Name{
			get{
				return "table-middle";
			}
		}
		
	}
	
	public class Middle:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Middle;
		}
		
		public override string Name{
			get{
				return "middle";
			}
		}
		
	}
	
	public class Baseline:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Baseline;
		}
		
		public override string Name{
			get{
				return "baseline";
			}
		}
		
	}
	
	public class Sub:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Sub;
		}
		
		public override string Name{
			get{
				return "sub";
			}
		}
		
	}
	
	public class Super:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.Super;
		}
		
		public override string Name{
			get{
				return "super";
			}
		}
		
	}
	
	public class TextTop:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.TextTop;
		}
		
		public override string Name{
			get{
				return "text-top";
			}
		}
		
	}
	
	public class TextBottom:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return VerticalAlignMode.TextBottom;
		}
		
		public override string Name{
			get{
				return "text-bottom";
			}
		}
		
	}
	
}