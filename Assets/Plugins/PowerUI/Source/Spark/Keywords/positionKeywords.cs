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
	/// Represents an instance of the static keyword.
	/// </summary>
	
	public class Static:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.Static;
		}
		
		public override string Name{
			get{
				return "static";
			}
		}
		
	}
	
	public class Fixed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.Fixed;
		}
		
		public override string Name{
			get{
				return "fixed";
			}
		}
		
	}
	
	public class Absolute:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.Absolute;
		}
		
		public override string Name{
			get{
				return "absolute";
			}
		}
		
	}
	
	public class SparkAbsolute:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.SparkAbsolute;
		}
		
		public override string Name{
			get{
				return "-spark-absolute";
			}
		}
		
	}
	
	public class SparkAbsoluteFixed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.SparkAbsoluteFixed;
		}
		
		public override string Name{
			get{
				return "-spark-absolute-fixed";
			}
		}
		
	}
	
	public class Relative:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.Relative;
		}
		
		public override string Name{
			get{
				return "relative";
			}
		}
		
	}
	
	public class Sticky:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return PositionMode.Sticky;
		}
		
		public override string Name{
			get{
				return "sticky";
			}
		}
		
	}
	
}