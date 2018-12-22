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
using Svg;
using Loonim;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the butt keyword.
	/// </summary>
	
	public class Butt:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return StrokeLineMode.Butt;
		}
		
		public override string Name{
			get{
				return "butt";
			}
		}
		
	}
	
	public class Round:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return StrokeLineMode.Round;
		}
		
		public override string Name{
			get{
				return "round";
			}
		}
		
	}
	
	public class Square:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return StrokeLineMode.Square;
		}
		
		public override string Name{
			get{
				return "square";
			}
		}
		
	}
	
	public class Miter:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return StrokeLineMode.Miter;
		}
		
		public override string Name{
			get{
				return "miter";
			}
		}
		
	}
	
	public class Bevel:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return StrokeLineMode.Bevel;
		}
		
		public override string Name{
			get{
				return "bevel";
			}
		}
		
	}
	
}



